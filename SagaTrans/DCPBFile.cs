//-----------------------------------------------------------------------
// <copyright file="DCPBFile.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-09-17</date>
// <summary>One type of script file in SaGa 2.</summary>
//-----------------------------------------------------------------------

namespace SagaTrans
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// One type of script file in SaGa 2.
    /// </summary>
    public class DCPBFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".scripb";
        private const string TextFileExtension = @".sjs";
        private const int MagicSignature = 0x42504344;
        private const int SignatureOffset = 0x00;
        private const int DataSizeOffset = 0x04;
        private const int DataOffset = 0x08;
        private string[] strings;
        private int headerSize;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the DCPBFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public DCPBFile(string fileName, IGame game) : base(fileName, game)
        {
        }
        
        #region Properties
        
        /// <summary>
        /// Gets the name of the text file relative to the directory where the game was extracated.
        /// </summary>
        /// <value>The name of the text file.</value>
        protected override string TextFileName
        {
            get
            {
                return Path.ChangeExtension(TextDirectoryPrefix + this.FileName, TextFileExtension);
            }
        }
        
        private string GameFileName
        {
            get
            {
                return Path.ChangeExtension(GameDirectoryPrefix + this.FileName, GameFileExtension);
            }
        }
        #endregion
        
        /// <summary>
        /// Extracts the text strings from the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully loaded.</returns>
        protected override bool LoadText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            int fileType = BitConverter.ToInt32(fileData, SignatureOffset);
            if (fileType != MagicSignature)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, "File \"{0}\" is not DCPB format.", this.GameFileName));
            }
            
            int dataSize = BitConverter.ToInt32(fileData, DataSizeOffset);
            
            int stringCountOffset = DataOffset + dataSize;
            
            int stringCount = BitConverter.ToInt32(fileData, stringCountOffset);
            this.strings = new string[stringCount];
            
            int tableOffset = stringCountOffset + 4;
            this.headerSize = tableOffset;
            int pointerOffset = tableOffset;
            
            int stringOffset;
            int stringLength;
            for (int i = 0; i < this.strings.Length; i++)
            {
                stringOffset = BitConverter.ToInt32(fileData, pointerOffset);
                stringLength = BitConverter.ToInt32(fileData, pointerOffset + Constants.PointerLength);
                
                this.strings[i] = this.Game.Encoding.GetString(fileData, tableOffset + stringOffset, stringLength);
                
                pointerOffset += 2 * Constants.PointerLength;
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
            
            int stringOffset = this.strings.Length * 2 * Constants.PointerLength;
            MemoryStream pointerTable = new MemoryStream();
            MemoryStream stringData = new MemoryStream();
            BinaryWriter pointerWriter = new BinaryWriter(pointerTable);
            BinaryWriter stringWriter = new BinaryWriter(stringData);
            
            byte[] encodedString;
            int length;
            for (int i = 0; i < this.strings.Length; i++)
            {
                long oldOffset = stringWriter.BaseStream.Position;
                encodedString = this.Game.Encoding.GetBytes(this.strings[i]);
                length = encodedString.Length;
                
                pointerWriter.Write(stringOffset);
                pointerWriter.Write(length);
                stringWriter.Write(encodedString);
                
                long newOffset = stringWriter.BaseStream.Position;
                stringOffset += (int)(newOffset - oldOffset);
            }
            
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
