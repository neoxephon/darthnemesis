//-----------------------------------------------------------------------
// <copyright file="MusicBoxFile.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-12-17</date>
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
    public class MusicBoxFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".bin";
        private const string TextFileExtension = @".sjs";
        private const int StringCountOffset = 0x08;
        private const int StringSectionOffset = 0x14;
        private const int MaxStringLength = 0x3E;
        private const int TableEntryLength = 0x48;
        private string[] strings;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the MusicBoxFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public MusicBoxFile(string fileName, IGame game) : base(fileName, game)
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
            
            int stringCount = BitConverter.ToInt32(fileData, StringCountOffset);
            this.strings = new string[stringCount];
            
            int stringOffset = 4 + BitConverter.ToInt32(fileData, StringSectionOffset);
            for (int i = 0; i < this.strings.Length; i++)
            {
                int stringLength = CalculateStringLength(fileData, stringOffset);
                this.strings[i] = this.Game.Encoding.GetString(fileData, stringOffset, stringLength);
                stringOffset += TableEntryLength;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            
            int stringOffset = 4 + BitConverter.ToInt32(fileData, StringSectionOffset);
            byte[] encodedString;
            int stringLength;
            for (int i = 0; i < this.strings.Length; i++)
            {
                encodedString = this.Game.Encoding.GetBytes(this.strings[i]);
                stringLength = encodedString.Length;
                if (stringLength > MaxStringLength)
                {
                    throw new FormatException(string.Format(CultureInfo.CurrentCulture, "String length {0} exceeds maximum of {1}: \"{2}\"", stringLength, MaxStringLength, this.strings[i]));
                }
                
                Array.Copy(encodedString, 0, fileData, stringOffset, stringLength);
                for (int p = stringLength; p < MaxStringLength; p++)
                {
                    fileData[stringOffset + p] = 0x00;
                }
                
                stringOffset += TableEntryLength;
            }
            
            StreamHelper.WriteFile(this.GameFileName, fileData);
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
        
        private static int CalculateStringLength(byte[] data, int offset)
        {
            int i = 0;
            for (; i < MaxStringLength && data[offset + i] != 0x00; i++)
            {
            }
            
            return i;
        }
    }
}
