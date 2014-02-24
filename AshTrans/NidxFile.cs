//-----------------------------------------------------------------------
// <copyright file="NidxFile.cs" company="DarthNemesis">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2014-02-24</date>
// <summary>A script file.</summary>
//-----------------------------------------------------------------------

namespace AshTrans
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// A NIDX file.
    /// </summary>
    public class NidxFile : CachedTextFile
    {
        #region Variables
        
        /// <summary>
        /// The file type identifier for NIDX files.
        /// </summary>
        private const uint Magic = 0x5844494E;
        
        /// <summary>
        /// The relative path of the base directory where all game files are stored.
        /// </summary>
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        
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
        /// Shift_JIS character mappings.
        /// </summary>
        private static Encoding shiftJis = Encoding.GetEncoding("Shift_JIS");
        
        /// <summary>
        /// The decoded text strings.
        /// </summary>
        private string[] strings;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the NidxFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public NidxFile(string fileName, IGame game) : base(fileName, game)
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
            uint fileType = BitConverter.ToUInt32(fileData, 0);
            if (fileType != Magic)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "File \"{0}\" is not NIDX format.", this.GameFileName));
            }
            
            int numStrings = BitConverter.ToInt32(fileData, 0x04);
            this.strings = new string[2 * numStrings];
            
            int pointerOffset = 0x08;
            ushort length;
            int pad;
            for (int i = 0; i < numStrings; i++)
            {
                int stringOffset = BitConverter.ToInt32(fileData, pointerOffset);
                
                for (int j = 0; j < 2; j++)
                {
                    length = BitConverter.ToUInt16(fileData, stringOffset);
                    pad = (length > 0 && length % 2 == 1) ? 1 : 0;
                    this.strings[(2 * i) + j] = shiftJis.GetString(fileData, stringOffset + 2, (length > 0) ? length - 1 : length);
                    stringOffset += 2 + length + pad;
                }
                
                pointerOffset += 0x04;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            int numStrings = this.strings.Length / 2;
            int pointerOffset = 0x08;
            int stringOffset = pointerOffset + (0x04 * numStrings);
            MemoryStream pointerTable = new MemoryStream();
            MemoryStream stringData = new MemoryStream();
            
            using (BinaryWriter pointerWriter = new BinaryWriter(pointerTable), stringWriter = new BinaryWriter(stringData))
            {
                byte[] encodedString;
                ushort length;
                int pad;
                for (int i = 0; i < numStrings; i++)
                {
                    long oldOffset = stringWriter.BaseStream.Position;
                    pointerWriter.Write(stringOffset);
                    
                    for (int j = 0; j < 2; j++)
                    {
                        encodedString = shiftJis.GetBytes(this.strings[(2 * i) + j]);
                        length = (ushort)encodedString.Length;
                        if (encodedString.Length > 0)
                        {
                            length++;
                        }
                        
                        pad = (length == 0) ? 0 : (length % 2 == 1) ? 2 : 1; // pad if odd
                        stringWriter.Write(length);
                        stringWriter.Write(encodedString);
                        for (int k = 0; k < pad; k++)
                        {
                            stringWriter.Write((byte)0x00);
                        }
                    }
                    
                    // Align to 4-byte boundary
                    long pos = stringWriter.BaseStream.Position;
                    pad = (int)(3 - ((pos - 1) % 4));
                    for (int k = 0; k < pad; k++)
                    {
                        stringWriter.Write((byte)0x00);
                    }
                    
                    long newOffset = stringWriter.BaseStream.Position;
                    pointerOffset += 0x04;
                    stringOffset += (int)(newOffset - oldOffset);
                }
            }
            
            MemoryStream contents = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(contents))
            {
                writer.Write(Magic);
                writer.Write(numStrings);
                writer.Write(pointerTable.ToArray());
                writer.Write(stringData.ToArray());
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
