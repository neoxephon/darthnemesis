//-----------------------------------------------------------------------
// <copyright file="LanguageDatabase.cs" company="DarthNemesis">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2014-02-15</date>
// <summary>A language database file.</summary>
//-----------------------------------------------------------------------

namespace TingleTrans
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// Description of LanguageDatabase.
    /// </summary>
    public class LanguageDatabase : CachedTextFile
    {
        #region Variables
        
        /// <summary>
        /// The relative path of the base directory where all game files are stored.
        /// </summary>
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\";
        
        /// <summary>
        /// The relative path of the base directory where exported text files should be stored.
        /// </summary>
        private const string TextDirectoryPrefix = @"text\";
        
        /// <summary>
        /// The file extension for this type of file.
        /// </summary>
        private const string GameFileExtension = @".bin";
        
        /// <summary>
        /// The file extension to use for exported text files.
        /// </summary>
        private const string TextFileExtension = @".txt";
        
        /// <summary>
        /// The non-standard character mappings used to decode re-encode game strings.
        /// </summary>
        private Encoding encoding = new CustomEncoding();
        
        /// <summary>
        /// The file header.
        /// </summary>
        private byte[] headerData;
        
        /// <summary>
        /// The headers for each section containing the tables of string lengths and other data.
        /// </summary>
        private byte[][] tableData;
        
        /// <summary>
        /// The bodies of each string section.
        /// </summary>
        private byte[][] textData;
        
        /// <summary>
        /// The unique identifiers of each table.
        /// </summary>
        private int[] tableNumbers;
        
        /// <summary>
        /// The starting offsets of each table in the file.
        /// </summary>
        private int[] tableOffsets;
        
        /// <summary>
        /// The starting offsets of each text block in the file.
        /// </summary>
        private int[] textOffsets;
        
        /// <summary>
        /// The combined lengths of each table + text block. 
        /// </summary>
        private int[] blockLengths;
        
        /// <summary>
        /// The decoded text strings.
        /// </summary>
        private string[] strings;
        
        /// <summary>
        /// The index of the single empty string that does not have a 'FFFF' terminator.
        /// This index will be skipped when writing terminators during saving.
        /// </summary>
        private int skipIndex;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the LanguageDatabase class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public LanguageDatabase(string fileName, IGame game) : base(fileName, game)
        {
        }
        
        /// <summary>
        /// Gets the name of the text file relative to the directory where the game was extracted.
        /// </summary>
        /// <value>The name of the text file.</value>
        protected override string TextFileName
        {
            get
            {
                return TextDirectoryPrefix + Path.ChangeExtension(this.FileName, TextFileExtension);
            }
        }
        
        /// <summary>
        /// Gets the name of the game file relative to the directory where the game was extracted.
        /// </summary>
        /// <value>The name of the game file.</value>
        private string GameFileName
        {
            get
            {
                return GameDirectoryPrefix + Path.ChangeExtension(this.FileName, GameFileExtension);
            }
        }
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        protected override bool LoadText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            this.headerData = new byte[0x80];
            Array.Copy(fileData, 0, this.headerData, 0, 0x80);
            
            int numTables = BitConverter.ToInt16(this.headerData, 0);
            int numStrings = BitConverter.ToInt32(this.headerData, 2);
            this.tableNumbers = new int[numTables];
            this.tableOffsets = new int[numTables];
            this.textOffsets = new int[numTables];
            this.blockLengths = new int[numTables];
            this.tableData = new byte[6][];
            this.textData = new byte[6][];
            
            BinaryReader reader = new BinaryReader(new MemoryStream(fileData, 0x10, numTables * 0x10));
            for (int i = 0; i < numTables; i++)
            {
                this.tableNumbers[i] = reader.ReadInt32();
                this.tableOffsets[i] = reader.ReadInt32();
                this.textOffsets[i] = reader.ReadInt32();
                this.blockLengths[i] = reader.ReadInt32();
                this.tableData[i] = new byte[this.textOffsets[i] - this.tableOffsets[i]];
                this.textData[i] = new byte[this.blockLengths[i] - (this.textOffsets[i] - this.tableOffsets[i])];
                Array.Copy(fileData, this.tableOffsets[i], this.tableData[i], 0, this.tableData[i].Length);
                Array.Copy(fileData, this.textOffsets[i], this.textData[i], 0, this.textData[i].Length);
                
                // Only the first table contains any meaningful strings. The rest are blank, so don't waste time decoding them.
                if (i == 0)
                {
                    int textOffset = this.textOffsets[i];
                    this.strings = new string[numStrings];
                    BinaryReader tableReader = new BinaryReader(new MemoryStream(fileData, this.tableOffsets[i], this.blockLengths[i]));
                    for (int j = 0; j < numStrings; j++)
                    {
                        // The first three shorts appear to be ids or some other data unrelated to the location/length of the strings.
                        // They don't need to be modified in order to replace the existing strings, so ignore them for now.
                        // They might need to be identified in order to support adding more lines to a conversation, though. 
                        tableReader.ReadUInt16();
                        tableReader.ReadUInt16();
                        tableReader.ReadUInt16();
                        int textLength = tableReader.ReadInt16();
                        if (textLength == 0)
                        {
                            this.skipIndex = j;
                        }
                        
                        this.strings[j] = this.encoding.GetString(fileData, textOffset, textLength);
                        textOffset += textLength;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            this.tableOffsets[0] = this.headerData.Length;
            MemoryStream textStream = new MemoryStream();
            BinaryWriter textWriter = new BinaryWriter(textStream);
            
            int tableIndex = 0x06;
            for (int i = 0; i < this.strings.Length; i++)
            {
                // Skip the one string that's empty and contains no terminator.
                if (i != this.skipIndex)
                {
                    byte[] encodedBytes = this.encoding.GetBytes(this.strings[i]);
                    ushort length = (ushort)encodedBytes.Length;
                    byte[] lengthBytes = BitConverter.GetBytes(length);
                    Array.Copy(lengthBytes, 0, this.tableData[0], tableIndex, 2);
                    textWriter.Write(encodedBytes);
                }
                
                tableIndex += 0x08;
            }
            
            // Pad the text block with 00 until its size is a multiple of 16 bytes.
            textWriter.Flush();
            int padding = (int)((0x10 - (textStream.Length % 0x10)) % 0x10);
            for (int i = 0; i < padding; i++)
            {
                textWriter.Write((byte)0x00);
            }
            
            textWriter.Flush();
            this.textData[0] = textStream.ToArray();
            
            int headerIndex = 0x14;
            for (int i = 0; i < this.tableData.Length; i++)
            {
                this.tableOffsets[i] = (i == 0) ? this.headerData.Length : this.textOffsets[i - 1] + this.textData[i - 1].Length;
                this.textOffsets[i] = this.tableOffsets[i] + this.tableData[i].Length;
                this.blockLengths[i] = this.tableData[i].Length + this.textData[i].Length;
                StreamHelper.WriteBytes(this.tableOffsets[i], this.headerData, headerIndex);
                StreamHelper.WriteBytes(this.textOffsets[i], this.headerData, headerIndex + 0x04);
                StreamHelper.WriteBytes(this.blockLengths[i], this.headerData, headerIndex + 0x08);
                headerIndex += 0x10;
            }
            
            MemoryStream contents = new MemoryStream();
            contents.Write(this.headerData, 0, this.headerData.Length);
            for (int i = 0; i < this.tableData.Length; i++)
            {
                contents.Write(this.tableData[i], 0, this.tableData[i].Length);
                contents.Write(this.textData[i], 0, this.textData[i].Length);
            }
            
            StreamHelper.WriteFile(this.GameFileName, contents.ToArray());
            
            return true;
        }
        
        /// <summary>
        /// Writes the text strings into an easily editable text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully exported.</returns>
        protected override bool ExportText()
        {
            StreamHelper.WriteLinesToFile(this.TextFileName, this.strings, Encoding.Unicode);
            return true;
        }
        
        /// <summary>
        /// Reads in the replacement text strings from the text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        protected override bool ImportText()
        {
            StreamHelper.ReadLinesFromFile(this.TextFileName, this.strings, Encoding.Unicode);
            return true;
        }
    }
}
