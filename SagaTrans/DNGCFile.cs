//-----------------------------------------------------------------------
// <copyright file="DNGCFile.cs" company="DarthNemesis">
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
    public class DNGCFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".bin";
        private const string TextFileExtension = @".sjs";
        private const int MagicSignature = 0x43474E44;
        private const int SignatureOffset = 0x00;
        private const int StringCountOffset = 0x04;
        private const int PointerTableOffset = 0x08;
        private string[][] strings;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the DNGCFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public DNGCFile(string fileName, IGame game) : base(fileName, game)
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
                return TextDirectoryPrefix + Path.ChangeExtension(this.FileName, TextFileExtension);
            }
        }
        
        private string GameFileName
        {
            get
            {
                return GameDirectoryPrefix + Path.ChangeExtension(this.FileName, GameFileExtension);
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
            uint fileType = BitConverter.ToUInt32(fileData, SignatureOffset);
            if (fileType != MagicSignature)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, "File \"{0}\" is not DNGC format.", this.GameFileName));
            }
            
            int numStrings = BitConverter.ToInt32(fileData, StringCountOffset);
            this.strings = new string[numStrings][];
            
            int pointerOffset = PointerTableOffset;
            int stringOffset;
            ushort length;
            int pad;
            for (int i = 0; i < this.strings.Length; i++)
            {
                this.strings[i] = new string[2];
                stringOffset = BitConverter.ToInt32(fileData, pointerOffset);
                
                for (int k = 0; k < this.strings[i].Length; k++)
                {
                    length = BitConverter.ToUInt16(fileData, stringOffset);
                    pad = (length > 0 && length % 2 == 1) ? 1 : 0;
                    this.strings[i][k] = this.Game.Encoding.GetString(fileData, stringOffset + 2, (length > 0) ? length - 1 : length);
                    stringOffset += 2 + length + pad;
                }
                
                pointerOffset += Constants.PointerLength;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            int pointerOffset = PointerTableOffset;
            int stringOffset = pointerOffset + (this.strings.Length * Constants.PointerLength);
            MemoryStream pointerTable = new MemoryStream();
            MemoryStream stringData = new MemoryStream();
            BinaryWriter pointerWriter = new BinaryWriter(pointerTable);
            BinaryWriter stringWriter = new BinaryWriter(stringData);
            
            pointerWriter.Write(MagicSignature);
            pointerWriter.Write(this.strings.Length);
            
            byte[] encodedString;
            ushort stringLength;
            int pad;
            for (int i = 0; i < this.strings.Length; i++)
            {
                long oldOffset = stringWriter.BaseStream.Position;
                pointerWriter.Write(stringOffset);
                
                for (int k = 0; k < this.strings[i].Length; k++)
                {
                    encodedString = this.Game.GetBytes(this.strings[i][k]);
                    stringLength = (ushort)encodedString.Length;
                    if (encodedString.Length > 0)
                    {
                        stringLength++;
                    }
                    
                    pad = (stringLength == 0) ? 0 : (stringLength % 2 == 1) ? 2 : 1; // pad if odd
                    stringWriter.Write(stringLength);
                    stringWriter.Write(encodedString);
                    for (int p = 0; p < pad; p++)
                    {
                        stringWriter.Write((byte)0x00);
                    }
                }
                
                long newOffset = stringWriter.BaseStream.Position;
                pointerOffset += Constants.PointerLength;
                stringOffset += (int)(newOffset - oldOffset);
            }
            
            MemoryStream contents = new MemoryStream();
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
            StreamHelper.WriteLinesToFile2D(this.TextFileName, this.strings, this.Game.Encoding);
            return true;
        }
        
        /// <summary>
        /// Reads in the replacement text strings from the text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        protected override bool ImportText()
        {
            StreamHelper.ReadLinesFromFile2D(this.TextFileName, this.strings, this.Game.Encoding);
            return true;
        }
    }
}
