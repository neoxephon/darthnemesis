//-----------------------------------------------------------------------
// <copyright file="GMMFile.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-04-27</date>
// <summary>A text file in Maple Story DS.</summary>
//-----------------------------------------------------------------------

namespace MapleTrans
{
    using System;
    using System.IO;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// A text file in Maple Story DS.
    /// </summary>
    public class GMMFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string CacheDirectoryPrefix = @"UNPACK\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".GMM.KOREAN";
        private const string TextFileExtension = @".txt";
        private const int StringCountOffset = 0x02;
        private const int TableEntryLength = 0x0C;
        private string[] strings;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the GMMFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public GMMFile(string fileName, IGame game) : base(fileName, game)
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
                return TextDirectoryPrefix + this.FileName.Replace(".GMM.KOREAN", TextFileExtension);
            }
        }
        
        private string GameFileName
        {
            get
            {
                return CacheDirectoryPrefix + this.FileName;
            }
        }
        
        private int TextSectionOffset
        {
            get
            {
                return 0x04 + (this.strings.Length * TableEntryLength);
            }
        }
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        protected override bool LoadText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            
            int numStrings = BitConverter.ToUInt16(fileData, StringCountOffset);
            
            this.strings = new string[numStrings];
            
            int pointerOffset = 0x0C;
            for (int i = 0; i < numStrings; i++)
            {
                int textOffset = this.TextSectionOffset + BitConverter.ToInt32(fileData, pointerOffset);
                ushort textLength = BitConverter.ToUInt16(fileData, textOffset);
                string text = this.Game.GetText(fileData, textOffset + 2, textLength);
                this.strings[i] = text;
                
                pointerOffset += TableEntryLength;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            byte[] headerData = StreamHelper.ReadFile(this.GameFileName, this.TextSectionOffset);
            
            MemoryStream textStream = new MemoryStream();
            BinaryWriter textWriter = new BinaryWriter(textStream);
            
            // Placeholder for text length
            textWriter.Write((ushort)0x0000);
            
            byte[] encodedString;
            int pointerOffset = 0x0C;
            int textOffset = 0x02;
            for (int i = 0; i < this.strings.Length; i++)
            {
                encodedString = this.Game.GetBytes(this.strings[i]);
                ushort stringLength = (ushort)encodedString.Length;
                
                StreamHelper.WriteBytes(textOffset, headerData, pointerOffset);
                textWriter.Write(stringLength);
                textWriter.Write(encodedString);
                
                pointerOffset += TableEntryLength;
                textOffset += 0x02 + stringLength;
            }
            
            textWriter.Flush();
            
            byte[] textData = textStream.ToArray();
            ushort textLength = (ushort)textData.Length;
            ushort totalLength = (ushort)(this.TextSectionOffset + textLength);
            
            WriteBytes(textLength, textData, 0x00);
            WriteBytes(totalLength, headerData, 0x00);
            
            MemoryStream contents = new MemoryStream();
            contents.Write(headerData, 0, headerData.Length);
            contents.Write(textData, 0, textData.Length);
            
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
        
        private static bool WriteBytes(ushort value, byte[] array, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (offset < 0 || offset + bytes.Length > array.Length)
            {
                return false;
            }
            
            Array.Copy(bytes, 0, array, offset, bytes.Length);
            return true;
        }
    }
}
