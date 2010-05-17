//-----------------------------------------------------------------------
// <copyright file="DTXFile.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-13</date>
// <summary>A text file in Summon Night X.</summary>
//-----------------------------------------------------------------------

namespace SummonTransX
{
    using System;
    using System.IO;
    using System.Text;
    using DarthNemesis;

    /// <summary>
    /// A text file in Summon Night X.
    /// </summary>
    public class DTXFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string CacheDirectoryPrefix = @"cache\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".dtx";
        private const string TextFileExtension = @".sjs";
        private const int DataCountOffset = 0x00;
        private const int StringCountOffset = 0x04;
        private int headerSize;
        private string[] strings;
        private bool isPacked;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the DTXFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        /// <param name="isPacked">A value indicating whether the text file originated from a pack file.</param>
        public DTXFile(string fileName, IGame game, bool isPacked) : base(fileName, game)
        {
            this.isPacked = isPacked;
        }
        
        /// <summary>
        /// Initializes a new instance of the DTXFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public DTXFile(string fileName, IGame game) : base(fileName, game)
        {
        }
        
        /// <summary>
        /// Gets the name of the text file relative to the directory where the game was extracated.
        /// </summary>
        /// <value>The name of the text file.</value>
        protected override string TextFileName
        {
            get
            {
                return TextDirectoryPrefix + Path.ChangeExtension(this.FileName, TextFileExtension);
            }
        }
        
        private string GameFileName
        {
            get
            {
                return (this.isPacked ? CacheDirectoryPrefix : GameDirectoryPrefix) + Path.ChangeExtension(this.FileName, GameFileExtension);
            }
        }
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        protected override bool LoadText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            
            int numDatas = BitConverter.ToInt32(fileData, DataCountOffset);
            int numStrings = BitConverter.ToInt32(fileData, StringCountOffset);
            
            int tableOffset = 0x08 + (8 * numDatas);
            int textOffset = tableOffset + (4 * numStrings);
            
            this.headerSize = tableOffset;
            this.strings = new string[numStrings];
            
            int pointerOffset = tableOffset;
            int stringOffset;
            for (int i = 0; i < this.strings.Length; i++)
            {
                stringOffset = BitConverter.ToInt32(fileData, pointerOffset);
                this.strings[i] = this.Game.GetText(fileData, textOffset + stringOffset);
                pointerOffset += 4;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            byte[] headerData = StreamHelper.ReadFile(this.GameFileName, this.headerSize);
            
            MemoryStream pointerTable = new MemoryStream();
            MemoryStream stringData = new MemoryStream();
            BinaryWriter pointerWriter = new BinaryWriter(pointerTable);
            BinaryWriter stringWriter = new BinaryWriter(stringData);
            
            byte[] encodedString;
            int stringOffset = 0;
            for (int i = 0; i < this.strings.Length; i++)
            {
                encodedString = this.Game.GetBytes(this.strings[i]);
                
                pointerWriter.Write(stringOffset);
                stringWriter.Write(encodedString);
                
                stringOffset += encodedString.Length;
                if (encodedString.Length % 2 == 0)
                {
                    stringWriter.Write((short)0x00);
                    stringOffset += 2;
                }
                else
                {
                    stringWriter.Write((byte)0x00);
                    stringOffset += 1;
                }
            }
            
            pointerWriter.Flush();
            stringWriter.Flush();
            
            MemoryStream contents = new MemoryStream();
            contents.Write(headerData, 0, headerData.Length);
            pointerTable.WriteTo(contents);
            stringData.WriteTo(contents);
            
            StreamHelper.WriteFile(this.GameFileName, contents.ToArray());
            return true;
        }
        
        /// <summary>
        /// Writes the text strings into an easily editable text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully exported.</returns>
        protected override bool ExportText()
        {
            StreamHelper.WriteLinesToFile(this.TextFileName, this.strings, this.Game.Encoding);
            return true;
        }
        
        /// <summary>
        /// Reads in the replacement text strings from the text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        protected override bool ImportText()
        {
            StreamHelper.ReadLinesFromFile(this.TextFileName, this.strings, this.Game.Encoding);
            return true;
        }
    }
}
