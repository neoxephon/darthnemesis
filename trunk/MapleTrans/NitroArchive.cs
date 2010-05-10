//-----------------------------------------------------------------------
// <copyright file="NitroArchive.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-05-01</date>
// <summary>A standard Nintendo archive file.</summary>
//-----------------------------------------------------------------------

namespace MapleTrans
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using DarthNemesis;

    /// <summary>
    /// A package file that contains multiple script files.
    /// </summary>
    public class NitroArchive : AbstractPackFile
    {
        #region Constants
        private const string GameDirectoryPrefix = @"UNPACK\";
        private const string ChildDirectoryPrefix = @"UNPACK\";
        private const string TextDirectoryPrefix = @"text\";
        private const string GameFileExtension = @".NXARC";
        private const string TextFileExtension = @".txt";
        private const int Signature = 0x4341784E;
        private const byte Padding = 0xFF;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the NitroArchive class.
        /// </summary>
        /// <param name="fileName">The file path relative to the GameDirectoryPrefix.</param>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public NitroArchive(string fileName, IGame game) : base(fileName, game)
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
        
        private string ChildDirectoryName
        {
            get
            {
                return Path.GetDirectoryName(this.FileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(this.FileName) + Path.DirectorySeparatorChar;
            }
        }
        
        #endregion
        
        /// <summary>
        /// Unpacks and initializes all files from the pack.
        /// </summary>
        public override void Unpack()
        {
            this.TextFiles.Clear();
            
            this.UnpackWithNarctool();
            this.InitializeChildren();
        }
        
        /// <summary>
        /// Repacks all files into the pack.
        /// </summary>
        public override void Pack()
        {
            this.PackWithNarctool();
        }
        
        private void UnpackWithNarctool()
        {
            string childFolderName = ChildDirectoryPrefix + this.ChildDirectoryName;
            
            Process command = new Process();
            command.StartInfo.UseShellExecute = false;
            command.StartInfo.CreateNoWindow = true;
            command.StartInfo.FileName = "narctool";
            command.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "u {0} {1}", this.GameFileName, childFolderName);
            command.Start();
            command.WaitForExit(20000);
            if (!command.HasExited)
            {
                throw new ArgumentException("Unable to unpack file");
            }
        }
        
        private void PackWithNarctool()
        {
            string childFolderName = ChildDirectoryPrefix + this.ChildDirectoryName;
            
            Process command = new Process();
            command.StartInfo.UseShellExecute = false;
            command.StartInfo.CreateNoWindow = true;
            command.StartInfo.FileName = "narctool";
            command.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "p {1} {0}", this.GameFileName, childFolderName);
            command.Start();
            command.WaitForExit(20000);
            if (!command.HasExited)
            {
                throw new ArgumentException("Unable to pack file");
            }
        }
        
        private void InitializeChildren()
        {
            string[] fileNames = this.GetFileNames();
            
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i].EndsWith(".GMM.KOREAN", StringComparison.Ordinal))
                {
                    string fileName = fileNames[i].Replace(ChildDirectoryPrefix, string.Empty);
                    this.TextFiles.Add(new GMMFile(fileName, this.Game));
                }
            }
        }
        
        private string[] GetFileNames()
        {
            string folderName = ChildDirectoryPrefix + this.ChildDirectoryName;
            return Directory.GetFiles(folderName);
        }
    }
}
