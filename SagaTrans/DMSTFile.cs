//-----------------------------------------------------------------------
// <copyright file="DMSTFile.cs" company="DarthNemesis">
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
    public class DMSTFile : CachedTextFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".mst";
        private const string TextFileExtension = @".sjs";
        private const int MagicSignature = 0x54534D44;
        private const int SignatureOffset = 0x00;
        private const int SectionCountOffset = 0x04;
        private const int SectionTableOffset = 0x08;
        private string[][] strings;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the DMSTFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the directory where the game was extracted.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public DMSTFile(string fileName, IGame game) : base(fileName, game)
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
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, "File \"{0}\" is not DMST format.", this.GameFileName));
            }
            
            int numSections = BitConverter.ToInt32(fileData, SectionCountOffset);
            this.strings = new string[numSections][];
            
            int offSectionPointer = SectionTableOffset;
            for (int k = 0; k < this.strings.Length; k++)
            {
                int offCount = BitConverter.ToInt32(fileData, offSectionPointer);
                int numStrings = BitConverter.ToInt32(fileData, offCount);
                this.strings[k] = new string[numStrings];
                
                int offTable = offCount + Constants.PointerLength;
                int offPointer = offTable;
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    int offString = BitConverter.ToInt32(fileData, offPointer);
                    int length = BitConverter.ToInt32(fileData, offPointer + 4);
                    
                    this.strings[k][i] = this.Game.Encoding.GetString(fileData, offTable + offString, length);
                    
                    offPointer += 2 * Constants.PointerLength;
                }
                
                offSectionPointer += Constants.PointerLength;
            }
            
            return true;
        }
        
        /// <summary>
        /// Inserts the text strings back into the game file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully saved.</returns>
        protected override bool SaveText()
        {
            MemoryStream sectionPointerTable = new MemoryStream();
            MemoryStream sectionData = new MemoryStream();
            BinaryWriter sectionPointerWriter = new BinaryWriter(sectionPointerTable);
            BinaryWriter sectionWriter = new BinaryWriter(sectionData);
            
            sectionPointerWriter.Write(MagicSignature);
            sectionPointerWriter.Write(this.strings.Length);
            
            int sectionOffset = SectionTableOffset + (this.strings.Length * Constants.PointerLength);
            
            for (int k = 0; k < this.strings.Length; k++)
            {
                int stringOffset = this.strings[k].Length * 2 * Constants.PointerLength;
                MemoryStream pointerTable = new MemoryStream();
                MemoryStream stringData = new MemoryStream();
                BinaryWriter pointerWriter = new BinaryWriter(pointerTable);
                BinaryWriter stringWriter = new BinaryWriter(stringData);
                
                pointerWriter.Write(this.strings[k].Length);
                
                long oldSectionOffset = sectionWriter.BaseStream.Position;
                
                byte[] encodedString;
                int length;
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    long oldOffset = stringWriter.BaseStream.Position;
                    encodedString = this.Game.Encoding.GetBytes(this.strings[k][i]);
                    length = encodedString.Length;
                    
                    pointerWriter.Write(stringOffset);
                    pointerWriter.Write(length);
                    stringWriter.Write(encodedString);
                    
                    long newOffset = stringWriter.BaseStream.Position;
                    stringOffset += (int)(newOffset - oldOffset);
                }
                
                sectionPointerWriter.Write(sectionOffset);
                sectionWriter.Write(pointerTable.ToArray());
                sectionWriter.Write(stringData.ToArray());
                
                long newSectionOffset = sectionWriter.BaseStream.Position;
                sectionOffset += (int)(newSectionOffset - oldSectionOffset);
            }
            
            MemoryStream contents = new MemoryStream();
            sectionPointerTable.WriteTo(contents);
            sectionData.WriteTo(contents);
            
            StreamHelper.WriteFile(this.GameFileName, contents.ToArray());
            return true;
        }
        
        /// <summary>
        /// Writes the text strings into an easily editable text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully exported.</returns>
        protected override bool ExportText()
        {
            string directoryName = Path.GetDirectoryName(this.TextFileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            
            FileStream file = new FileStream(this.TextFileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file, this.Game.Encoding);
            
            for (int k = 0; k < this.strings.Length; k++)
            {
                writer.WriteLine("====================[" + k + "]====================");
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    writer.WriteLine(this.strings[k][i]);
                }
            }
            
            writer.Close();
            return true;
        }
        
        /// <summary>
        /// Reads in the replacement text strings from the text file.
        /// </summary>
        /// <returns>A value indicating whether the file was successfully imported.</returns>
        protected override bool ImportText()
        {
            FileStream file = new FileStream(this.TextFileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, this.Game.Encoding);
            for (int k = 0; k < this.strings.Length; k++)
            {
                reader.ReadLine();
                for (int i = 0; i < this.strings[k].Length; i++)
                {
                    this.strings[k][i] = StreamHelper.ReadNextLine(reader);
                    if (this.strings[k][i].Contains("=========="))
                    {
                        int captionStart = 1 + this.strings[k][i].IndexOf("[", StringComparison.Ordinal);
                        int captionEnd = this.strings[k][i].IndexOf("]", StringComparison.Ordinal);
                        if (captionStart >= 0 && captionEnd > captionStart)
                        {
                            int caption = int.Parse(this.strings[k][i].Substring(captionStart, captionEnd - captionStart), CultureInfo.CurrentCulture) - 1;
                            throw new FormatException("Invalid line break detected in section [" + caption + "].");
                        }
                        else
                        {
                            throw new FormatException("Invalid line break detected above line: " + this.strings[k][i]);
                        }
                    }
                }
            }
            
            reader.Close();
            return true;
        }
    }
}
