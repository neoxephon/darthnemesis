//-----------------------------------------------------------------------
// <copyright file="DPKFile.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-11-06</date>
// <summary>A package file in Summon Night X.</summary>
//-----------------------------------------------------------------------

namespace SummonTransX
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using DarthNemesis;

    /// <summary>
    /// A package file that contains multiple script files.
    /// </summary>
    public class DPKFile : AbstractPackFile
    {
        #region Variables
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\data\";
        private const string ChildDirectoryPrefix = @"cache\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".dpk";
        private const string ChildFileExtension = @".dtx";
        private const string TextFileExtension = @".sjs";
        private const int FileCountOffset = 0x00;
        private const int TableOffset = 0x04;
        private const int TableEntryLength = 0x0C;
        private const byte PaddingByte = 0xEE;
        private string childFilenameFormat;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the DPKFile class.
        /// </summary>
        /// <param name="fileName">The file path relative to the GameDirectoryPrefix.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public DPKFile(string fileName, IGame game) : base(fileName, game)
        {
            if (this.Game.IsUnpacked)
            {
                this.InitializeChildren();
            }
        }
        
        #region Properties
        private string GameFileName
        {
            get
            {
                return GameDirectoryPrefix + Path.ChangeExtension(this.FileName, GameFileExtension);
            }
        }
        
        private string ChildDirectory
        {
            get
            {
                return Path.GetDirectoryName(ChildDirectoryPrefix + this.FileName);
            }
        }
        #endregion
        
        /// <summary>
        /// Unpacks and initializes all files from the pack.
        /// </summary>
        public override void Unpack()
        {
            this.TextFiles.Clear();
            
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            int numChildren = BitConverter.ToInt32(fileData, FileCountOffset);
            
            // int[] fileIDs = new int[numChildren];
            int[] fileOffsets = new int[numChildren];
            int[] fileSizes = new int[numChildren];
            
            int offset = TableOffset;
            for (int i = 0; i < numChildren; i++)
            {
                // fileIDs[i] = BitConverter.ToInt32(fileData, offset);
                fileOffsets[i] = BitConverter.ToInt32(fileData, offset + 0x04);
                fileSizes[i] = BitConverter.ToInt32(fileData, offset + 0x08);
                offset += TableEntryLength;
            }
            
            this.DetermineChildFilenameFormat(numChildren);
            for (int i = 0; i < numChildren; i++)
            {
                this.ExportChild(fileData, i, fileOffsets[i], fileSizes[i]);
                if (fileSizes[i] > 0x10)
                {
                    this.TextFiles.Add(new DTXFile(this.ChildFileName(i), this.Game, true));
                }
            }
        }
        
        /// <summary>
        /// Repacks all files into the pack.
        /// </summary>
        public override void Pack()
        {
            MemoryStream headerData = new MemoryStream();
            MemoryStream packedData = new MemoryStream();
            BinaryWriter headerWriter = new BinaryWriter(headerData);
            BinaryWriter packedWriter = new BinaryWriter(packedData);
            int[] fileIDs = this.GetFileIDs();
            int tableLength = TableOffset + (TableEntryLength * fileIDs.Length);
            int headerLength = RoundUp(tableLength);
            int fileOffset = headerLength;
            
            headerWriter.Write(fileIDs.Length);
            this.DetermineChildFilenameFormat(fileIDs.Length);
            
            for (int i = 0; i < fileIDs.Length; i++)
            {
                byte[] childData = StreamHelper.ReadFile(ChildDirectoryPrefix + this.ChildFileName(i));
                int fileLength = RoundUp(childData.Length);
                
                headerWriter.Write(fileIDs[i]);
                headerWriter.Write(fileOffset);
                headerWriter.Write(fileLength);
                
                packedWriter.Write(childData);
                for (int pad = childData.Length; pad < fileLength; pad++)
                {
                    packedWriter.Write(PaddingByte);
                }
                
                fileOffset += fileLength;
            }
            
            for (int pad = tableLength; pad < headerLength; pad++)
            {
                headerWriter.Write(PaddingByte);
            }
            
            headerWriter.Flush();
            packedWriter.Flush();
            
            MemoryStream contents = new MemoryStream();
            headerData.WriteTo(contents);
            packedData.WriteTo(contents);
            
            StreamHelper.WriteFile(this.GameFileName, contents.ToArray());
        }
        
        private static int RoundUp(int offset)
        {
            return (int)(Math.Ceiling((double)offset / 0x10) * 0x10);
        }
        
        private void InitializeChildren()
        {
            int[] fileSizes = this.GetFileSizes();
            int numChildren = fileSizes.Length;
            this.DetermineChildFilenameFormat(numChildren);
            for (int i = 0; i < numChildren; i++)
            {
                if (fileSizes[i] > 0x10)
                {
                    this.TextFiles.Add(new DTXFile(this.ChildFileName(i), this.Game, true));
                }
            }
        }
        
        private string ChildFileName(int index)
        {
            return Path.ChangeExtension(this.FileName, string.Format(CultureInfo.InvariantCulture, this.childFilenameFormat, index));
        }
        
        private void ExportChild(byte[] fileData, int id, int offset, int length)
        {
            if (!Directory.Exists(this.ChildDirectory))
            {
                Directory.CreateDirectory(this.ChildDirectory);
            }
            
            FileStream file = null;
            try
            {
                file = new FileStream(ChildDirectoryPrefix + this.ChildFileName(id), FileMode.Create, FileAccess.Write);
                file.Write(fileData, offset, length);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
        
        private void DetermineChildFilenameFormat(int max)
        {
            string format = "{0:";
            for (int digits = max; digits > 0; digits /= 10)
            {
                format += "0";
            }
            
            format += "}" + ChildFileExtension;
            this.childFilenameFormat = format;
        }
        
        private int[] GetFileIDs()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            int numChildren = BitConverter.ToInt32(fileData, 0x00);
            
            int[] fileIDs = new int[numChildren];
            
            int offset = 0x04;
            for (int i = 0; i < numChildren; i++)
            {
                fileIDs[i] = BitConverter.ToInt32(fileData, offset);
                offset += TableEntryLength;
            }
            
            return fileIDs;
        }
        
        private int[] GetFileSizes()
        {
            byte[] fileData = StreamHelper.ReadFile(this.GameFileName);
            int numChildren = BitConverter.ToInt32(fileData, 0x00);
            
            int[] fileSizes = new int[numChildren];
            
            int offset = 0x0C;
            for (int i = 0; i < numChildren; i++)
            {
                fileSizes[i] = BitConverter.ToInt32(fileData, offset);
                offset += TableEntryLength;
            }
            
            return fileSizes;
        }
    }
}
