//-----------------------------------------------------------------------
// <copyright file="NexonArchive.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-04-24</date>
// <summary>A package file containing the data in Maple Story DS.</summary>
//-----------------------------------------------------------------------

namespace MapleTrans
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using DarthNemesis;

    /// <summary>
    /// A package file that contains multiple script files.
    /// </summary>
    public class NexonArchive : AbstractPackFile
    {
        #region Constants
        private const string GameDirectoryPrefix = @"NDS_UNPACK\data\";
        private const string ChildDirectoryPrefix = @"UNPACK\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".NXARC";
        private const string TextFileExtension = @".txt";
        private const int Signature = 0x4341784E;
        private const byte Padding = 0xFF;
        #endregion
        
        #region Variables
        private static readonly IList<string> GMMFiles = GetGMMFileList();
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the NexonArchive class.
        /// </summary>
        /// <param name="fileName">The file path relative to the GameDirectoryPrefix.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public NexonArchive(string fileName, IGame game) : base(fileName, game)
        {
            if (this.Game.IsUnpacked)
            {
                this.InitializeChildren();
            }
        }
        
        private enum FileType
        {
            EMPTY = 0,
            NARC = 1,
            NANR = 2,
            NCER = 3,
            NCGR = 4,
            NCLR = 5,
            NFTR = 6,
            NSCR = 7,
            GMM = 8,
            DAT = 255
        }
        
        #region Properties
        private string GameFileName
        {
            get
            {
                return GameDirectoryPrefix + this.FileName;
            }
        }
        
        private int FileCount
        {
            get
            {
                return this.Game.Cache.Settings[this.GameFileName]["fileCount"].Int32Value;
            }
            
            set
            {
                this.Game.Cache.Settings[this.GameFileName]["fileCount"].Int32Value = value;
            }
        }
        
        private int FolderCount
        {
            get
            {
                return this.Game.Cache.Settings[this.GameFileName]["folderCount"].Int32Value;
            }
            
            set
            {
                this.Game.Cache.Settings[this.GameFileName]["folderCount"].Int32Value = value;
            }
        }
        
        private int HeaderSize
        {
            get
            {
                return this.Game.Cache.Settings[this.GameFileName]["headerSize"].Int32Value;
            }
            
            set
            {
                this.Game.Cache.Settings[this.GameFileName]["headerSize"].Int32Value = value;
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
            uint signature = BitConverter.ToUInt32(fileData, 0x00);
            if (signature != Signature)
            {
                throw new ArgumentException("Unexpected file type");
            }
            
            this.FolderCount = BitConverter.ToUInt16(fileData, 0x04);
            this.FileCount = BitConverter.ToUInt16(fileData, 0x06);
            int fileTableOffset = BitConverter.ToInt32(fileData, 0x08);
            int fileDataOffset = BitConverter.ToInt32(fileData, 0x0C);
            
            this.HeaderSize = fileDataOffset;
            
            int readOffset = 0x20;
            
            ushort[] folderSizes = new ushort[this.FolderCount];
            for (int i = 0; i < this.FolderCount; i++)
            {
                folderSizes[i] = BitConverter.ToUInt16(fileData, readOffset);
                this.Game.Cache.Settings[this.GameFileName]["folders"][i]["count"].Int32Value = folderSizes[i];
                readOffset += 4;
            }
            
            string[] folderNames = new string[this.FolderCount];
            for (int i = 0; i < folderNames.Length; i++)
            {
                int endOffset = readOffset;
                while (fileData[endOffset] != 0x00)
                {
                    endOffset++;
                }
                
                folderNames[i] = Encoding.ASCII.GetString(fileData, readOffset, endOffset - readOffset);
                this.Game.Cache.Settings[this.GameFileName]["folders"][i]["name"].Value = folderNames[i];
                
                readOffset = endOffset + 1;
            }
            
            readOffset = fileTableOffset;
            
            int[] fileOffsets = new int[this.FileCount];
            int[] fileSizes = new int[this.FileCount];
            for (int i = 0; i < this.FileCount; i++)
            {
                fileOffsets[i] = fileDataOffset + BitConverter.ToInt32(fileData, readOffset);
                fileSizes[i] = BitConverter.ToInt32(fileData, readOffset + 4);
                readOffset += 8;
            }
            
            int folderIndex = 0;
            int folderFileIndex = 0;
            
            for (int i = 0; i < this.FileCount; i++)
            {
                while (folderFileIndex >= folderSizes[folderIndex])
                {
                    folderIndex++;
                    folderFileIndex = 0;
                }
                
                FileType fileType = DetermineChildFileType(fileData, fileOffsets, i);
                if (fileType == FileType.DAT)
                {
                    string key = folderNames[folderIndex] + ":" + folderFileIndex;
                    if (GMMFiles.Contains(key))
                    {
                        fileType = FileType.GMM;
                    }
                }
                
                string childFileName = this.DetermineChildFileName(folderNames[folderIndex], fileType, folderFileIndex);
                StreamHelper.WriteFile(ChildDirectoryPrefix + childFileName, fileData, fileOffsets[i], fileSizes[i]);
                this.Game.Cache.Settings[this.GameFileName][i].Int32Value = (int)fileType;
                folderFileIndex++;
            }
            
            this.InitializeChildren();
        }
        
        /// <summary>
        /// Repacks all files into the pack.
        /// </summary>
        public override void Pack()
        {
            MemoryStream packStream = new MemoryStream();
            BinaryWriter packWriter = new BinaryWriter(packStream);
            
            byte[] headerData = StreamHelper.ReadFile(this.GameFileName, this.HeaderSize);
            
            ushort fileCount = BitConverter.ToUInt16(headerData, 0x06);
            int fileTableOffset = BitConverter.ToInt32(headerData, 0x08);
            int[] folderSizes = this.GetFolderSizes();
            string[] folderNames = this.GetFolderNames();
            
            int folderIndex = 0;
            int folderFileIndex = 0;
            int pointerOffset = fileTableOffset;
            int fileOffset = 0;
            for (int i = 0; i < fileCount; i++)
            {
                while (folderFileIndex >= folderSizes[folderIndex])
                {
                    folderIndex++;
                    folderFileIndex = 0;
                }
                
                FileType fileType = this.GetFileType(i);
                string childFileName = this.DetermineChildFileName(folderNames[folderIndex], fileType, folderFileIndex);
                byte[] childFileData = StreamHelper.ReadFile(ChildDirectoryPrefix + childFileName);
                
                StreamHelper.WriteBytes(fileOffset, headerData, pointerOffset);
                StreamHelper.WriteBytes(childFileData.Length, headerData, pointerOffset + 4);
                
                packWriter.Write(childFileData);
                
                int padLength = CalculatePaddingLength(childFileData.Length, 4);
                
                for (int p = 0; p < padLength; p++)
                {
                    packWriter.Write(Padding);
                }
                
                fileOffset += childFileData.Length + padLength;
                pointerOffset += 8;
                folderFileIndex++;
            }
            
            packWriter.Flush();
            
            MemoryStream contents = new MemoryStream();
            contents.Write(headerData, 0, headerData.Length);
            packStream.WriteTo(contents);
            
            StreamHelper.WriteFile(this.GameFileName, contents.ToArray());
        }
        
        private static IList<string> GetGMMFileList()
        {
            string[] gmmFileArray = new string[]
            {
                @"IN/STAGE/GLOBAL:0",
                @"IN/STAGE/GLOBAL:1",
                @"IN/STAGE/GLOBAL:2",
                @"IN/STAGE/GLOBAL:28",
                @"IN/STAGE/GLOBAL:30",
                @"IN/STAGE/GLOBAL:32",
                @"IN/STAGE/GLOBAL:33",
                @"IN/STAGE/GLOBAL:34",
                @"IN/STAGE/GLOBAL:39",
                @"IN/STAGE/GLOBAL:41",
                @"IN/STAGE/GLOBAL:42",
                @"IN/STAGE/GLOBAL:44",
                @"IN/STAGE/GLOBAL:45",
                @"IN/STAGE/GLOBAL:47",
                @"IN/STAGE/GLOBAL:50",
                @"IN/STAGE/GLOBAL:52",
                @"IN/STAGE/GLOBAL:53",
                @"IN/STAGE/GLOBAL:59",
                @"IN/STAGE/GLOBAL:60",
                @"IN/STAGE/GLOBAL:62",
                @"IN/STAGE/GLOBAL:64",
                @"IN/STAGE/GLOBAL:65",
                @"IN/STAGE/GLOBAL:67",
                @"IN/STAGE/GLOBAL:68",
                @"IN/STAGE/GLOBAL:71",
            };
            return new List<string>(gmmFileArray).AsReadOnly();
        }
        
        private static int CalculatePaddingLength(int offset, int boundary)
        {
            return (boundary - 1) - ((offset - 1) % boundary);
        }
        
        private static FileType DetermineChildFileType(byte[] data, int[] offsets, int index)
        {
            FileType type = FileType.EMPTY;
            if (offsets[index] + 4 < data.Length)
            {
                uint signature = BitConverter.ToUInt32(data, offsets[index]);
                switch (signature)
                {
                    case 0x4352414E:
                        type = FileType.NARC;
                        break;
                    case 0x4E414E52:
                        type = FileType.NANR;
                        break;
                    case 0x4E434552:
                        type = FileType.NCER;
                        break;
                    case 0x4E434752:
                        type = FileType.NCGR;
                        break;
                    case 0x4E434C52:
                        type = FileType.NCLR;
                        break;
                    case 0x4E465452:
                        type = FileType.NFTR;
                        break;
                    case 0x4E534352:
                        type = FileType.NSCR;
                        break;
                    default:
                        type = FileType.DAT;
                        break;
                }
            }
            
            return type;
        }
        
        private void InitializeChildren()
        {
            int[] folderSizes = this.GetFolderSizes();
            string[] folderNames = this.GetFolderNames();
            int folderIndex = 0;
            int folderFileIndex = 0;
            for (int i = 0; i < this.FileCount; i++)
            {
                while (folderFileIndex >= folderSizes[folderIndex])
                {
                    folderIndex++;
                    folderFileIndex = 0;
                }
                
                FileType fileType = this.GetFileType(i);
                if (fileType == FileType.GMM)
                {
                    string childFileName = this.DetermineChildFileName(folderNames[folderIndex], fileType, folderFileIndex);
                    this.TextFiles.Add(new GMMFile(childFileName, this.Game));
                }
                else if (fileType == FileType.NARC)
                {
                    string childFileName = this.DetermineChildFileName(folderNames[folderIndex], fileType, folderFileIndex);
                    this.PackFiles.Add(new NitroArchive(childFileName, this.Game));
                }
                
                folderFileIndex++;
            }
        }
        
        private string DetermineChildFileName(string folderName, FileType type, int index)
        {
            string fileName = folderName + Path.DirectorySeparatorChar + index;
            switch (type)
            {
                case FileType.NARC:
                    fileName += ".NARC";
                    break;
                case FileType.NANR:
                    fileName += ".NANR";
                    break;
                case FileType.NCER:
                    fileName += ".NCER";
                    break;
                case FileType.NCGR:
                    fileName += ".NCGR";
                    break;
                case FileType.NCLR:
                    fileName += ".NCLR";
                    break;
                case FileType.NFTR:
                    fileName += ".NFTR";
                    break;
                case FileType.NSCR:
                    fileName += ".NSCR";
                    break;
                case FileType.GMM:
                    fileName += ".GMM.KOREAN";
                    break;
                case FileType.EMPTY:
                    break;
                default:
                    fileName += ".dat";
                    break;
            }
            
            string prefix = Path.GetDirectoryName(this.FileName);
            if (prefix.Length > 0)
            {
                prefix += Path.DirectorySeparatorChar;
            }
            
            return fileName;
        }
        
        private FileType GetFileType(int index)
        {
            return (FileType) this.Game.Cache.Settings[this.GameFileName][index].Int32Value;
        }
        
        private int[] GetFolderSizes()
        {
            int[] folderSizes = new int[this.FolderCount];
            for (int i = 0; i < this.FolderCount; i++)
            {
                folderSizes[i] = this.Game.Cache.Settings[this.GameFileName]["folders"][i]["count"].Int32Value;
            }
            
            return folderSizes;
        }
        
        private string[] GetFolderNames()
        {
            string[] folderNames = new string[this.FolderCount];
            for (int i = 0; i < this.FolderCount; i++)
            {
                folderNames[i] = this.Game.Cache.Settings[this.GameFileName]["folders"][i]["name"].Value;
            }
            
            return folderNames;
        }
    }
}
