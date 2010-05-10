//-----------------------------------------------------------------------
// <copyright file="ParmFile.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-10-03</date>
// <summary>The primary script file in Ore ga Omae wo Mamoru.</summary>
//-----------------------------------------------------------------------

namespace MamoruTrans
{
    using System;
    using System.IO;
    using System.Text;
    using DarthNemesis;

    /// <summary>
    /// The primary script file in Ore ga Omae wo Mamoru.
    /// </summary>
    public class ParmFile : ITextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".dat";
        private const string TextFileExtension = ".sjs";
        private const int SectionTableOffset = 0x0C;
        private const byte NullCharacter = 0x00;
        private static readonly int[][] PointerOffsets =
        {
            new int[] { 0x04 },
            new int[] { 0x04, 0x48 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { },
            new int[] { 0x04 },
            new int[] { 0x04, 0x10, 0x14 },
            new int[] { 0x04 },
            new int[] { 0x04, 0x0C },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04 },
            new int[] { 0x04, 0x10, 0x14 },
            new int[] { 0x04, 0x14 },
            new int[] { 0x04, 0x24 },
            new int[] { 0x04, 0x08, 0x54, 0x58 },
            new int[] { 0x04, 0x2C },
            new int[] { 0x04, 0x24 },
            new int[] { },
            new int[] { 0x04, 0x14 },
            new int[] { 0x04, 0x0C },
        };
        
        private static readonly int[] TableEntryLengths =
        {
            0x0C,
            0x4C,
            0x10,
            0x2C,
            0x28,
            0x10,
            0x18,
            0x10,
            0x10,
            0x10,
            0x10,
            0x10,
            0x1C,
            0x10,
            0x10,
            0x10,
            0x14,
            0x18,
            0x18,
            0x28,
            0x5C,
            0x30,
            0x28,
            0x10,
            0x18,
            0x10,
        };
        
        private string[][][] strings;
        private byte[][][] tableEntries;
        private int signature;
        private int unknown;
        private string fileName;
        private IGame game;
        private long[] timestamps;
        private bool isModified;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the ParmFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public ParmFile(string fileName, IGame game)
        {
            this.fileName = fileName;
            this.game = game;
        }
        
        #region Properties
        
        /// <summary>
        /// Gets the name of the file relative to the directory where the game was extracted.
        /// </summary>
        /// <value>The file path.</value>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }
        
        private string GameFileName
        {
            get
            {
                return GameDirectoryPrefix + Path.ChangeExtension(this.fileName, GameFileExtension);
            }
        }
        #endregion
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        public bool Load()
        {
            if (!this.LoadText())
            {
                return false;
            }
            
            this.isModified = false;
            this.timestamps = new long[this.tableEntries.Length];
            for (int i = 0; i < this.timestamps.Length; i++)
            {
                this.timestamps[i] = this.game.GetTimestamp(this.fileName + i);
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        public bool Save()
        {
            if (!this.isModified)
            {
                return false;
            }
            
            if (!this.SaveText())
            {
                return false;
            }
            
            this.isModified = false;
            for (int i = 0; i < this.timestamps.Length; i++)
            {
                this.game.SetTimestamp(this.fileName + i, this.timestamps[i]);
            }
            
            return true;
        }
        
        /// <summary>
        /// Writes the text strings into an easily editable text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully exported.</returns>
        public bool Export()
        {
            bool result = false;
            
            for (int i = 0; i < this.timestamps.Length; i++)
            {
                if (!this.ExportText(i))
                {
                    continue;
                }
                
                this.timestamps[i] = StreamHelper.GetTimestamp(this.TextFileName(i));
                if (!this.isModified)
                {
                    this.game.SetTimestamp(this.fileName + i, this.timestamps[i]);
                }
                
                result = true;
            }
            
            return result;
        }
        
        /// <summary>
        /// Reads in the replacement text strings from the text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        public bool Import()
        {
            bool result = false;
            for (int i = 0; i < this.timestamps.Length; i++)
            {
                long newTimestamp = StreamHelper.GetTimestamp(this.TextFileName(i));
                if (newTimestamp == this.timestamps[i])
                {
                    continue;
                }
                
                if (!this.ImportText(i))
                {
                    continue;
                }
                
                this.timestamps[i] = newTimestamp;
                this.isModified = true;
                result = true;
            }
            
            return result;
        }
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        private bool LoadText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            
            this.signature = BitConverter.ToInt32(fileData, 0x00);
            this.unknown = BitConverter.ToInt32(fileData, 0x04);
            int numSections = BitConverter.ToInt32(fileData, 0x08);
            
            this.tableEntries = new byte[numSections][][];
            this.strings = new string[numSections][][];
            for (int i = 0; i < numSections; i++)
            {
                int numTableEntries = BitConverter.ToInt32(fileData, SectionTableOffset + ((2 * i) * Constants.PointerLength));
                int tableOffset = BitConverter.ToInt32(fileData, SectionTableOffset + (((2 * i) + 1) * Constants.PointerLength));
                
                this.tableEntries[i] = new byte[numTableEntries][];
                this.strings[i] = new string[numTableEntries][];
                for (int j = 0; j < numTableEntries; j++)
                {
                    int entryOffset = tableOffset + (j * TableEntryLengths[i]);
                    this.tableEntries[i][j] = new byte[TableEntryLengths[i]];
                    Array.Copy(fileData, entryOffset, this.tableEntries[i][j], 0, TableEntryLengths[i]);
                    
                    this.strings[i][j] = new string[PointerOffsets[i].Length];
                    for (int k = 0; k < PointerOffsets[i].Length; k++)
                    {
                        int stringOffset = BitConverter.ToInt32(fileData, entryOffset + PointerOffsets[i][k]);
                        this.strings[i][j][k] = this.game.GetText(fileData, stringOffset);
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        private bool SaveText()
        {
            MemoryStream headerStream = new MemoryStream();
            BinaryWriter header = new BinaryWriter(headerStream);
            MemoryStream dataStream = new MemoryStream();
            BinaryWriter data = new BinaryWriter(dataStream);
            
            header.Write(this.signature);
            header.Write(this.unknown);
            header.Write(this.strings.Length);
            
            int headerSize = SectionTableOffset + (8 * this.strings.Length);
            
            byte[] pointerBytes;
            int pointerOffset = headerSize;
            for (int k = 0; k < this.strings.Length; k++)
            {
                int[][] pointerTable = new int[this.strings[k].Length][];
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    pointerTable[i] = new int[this.strings[k][i].Length];
                    for (int p = 0; p < this.strings[k][i].Length; p++)
                    {
                        pointerTable[i][p] = pointerOffset;
                        byte[] encodedString = this.game.GetBytes(this.strings[k][i][p]);
                        data.Write(encodedString, 0, encodedString.Length);
                        data.Write(NullCharacter);
                        pointerOffset += encodedString.Length + 1;
                    }
                }
                
                for (; pointerOffset % 4 > 0; pointerOffset++)
                {
                    data.Write(NullCharacter);
                }
                
                header.Write(this.strings[k].Length);
                header.Write(pointerOffset);
                
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    for (int p = 0; p < PointerOffsets[k].Length; p++)
                    {
                        pointerBytes = BitConverter.GetBytes(pointerTable[i][p]);
                        Array.Copy(pointerBytes, 0, this.tableEntries[k][i], PointerOffsets[k][p], Constants.PointerLength);
                    }
                    
                    data.Write(this.tableEntries[k][i]);
                    pointerOffset += TableEntryLengths[k];
                }
            }
            
            dataStream.WriteTo(headerStream);
            StreamHelper.WriteFile(this.GameFileName, headerStream.ToArray());
            
            return true;
        }
        
        /// <summary>
        /// Writes the text strings from the given section into an easily editable text file.
        /// </summary>
        /// <param name="i">The index of the section to export.</param>
        /// <returns>A value indicating whether the file was successfully exported.</returns>
        private bool ExportText(int i)
        {
            StreamHelper.WriteLinesToFile2D(this.TextFileName(i), this.strings[i], this.game.Encoding);
            
            return true;
        }
        
        /// <summary>
        /// Reads in the replacement text strings for the given section from its text file.
        /// </summary>
        /// <param name="i">The index of the section to import.</param>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        private bool ImportText(int i)
        {
            StreamHelper.ReadLinesFromFile2D(this.TextFileName(i), this.strings[i], this.game.Encoding);
            
            return true;
        }
        
        private string TextFileName(int index)
        {
            return TextDirectoryPrefix + Path.ChangeExtension(this.fileName, index + TextFileExtension);
        }
    }
}
