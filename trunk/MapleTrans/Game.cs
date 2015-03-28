//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-04-24</date>
// <summary>Common properties and methods for the entire game.</summary>
//-----------------------------------------------------------------------

namespace MapleTrans
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// Common properties and methods for the entire game.
    /// </summary>
    public class Game : AbstractGame
    {
        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        public Game() : base()
        {
        }
        
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The application name.</value>
        public override string Name
        {
            get
            {
                return "MapleTrans";
            }
        }
        
        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public override string Version
        {
            get
            {
                return "v1.1";
            }
        }
        
        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>The application description.</value>
        public override string Description
        {
            get
            {
                return "A text editing utility for the Nintendo\r\nDS game Maple Story DS.";
            }
        }
        
        /// <summary>
        /// Gets the text encoding used by script files in the project.
        /// </summary>
        /// <value>The text encoding used by script files in the project.</value>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }
        
        /// <summary>
        /// Populates the list of files in the game.
        /// </summary>
        public override void InitializeFiles()
        {
            this.Files.PackFiles.Add(new NexonArchive("RESOURCE.NXARC", this));
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data)
        {
            return this.GetText(data, 0, data.Length);
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset)
        {
            int i;
            for (i = offset; i < data.Length; i++)
            {
                // End of string
                if (data[i] == 0x00)
                {
                    break;
                }
            }
            
            return this.GetText(data, offset, i - offset);
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <param name="length">The number of bytes to read from the array.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset, int length)
        {
            string text = Encoding.GetString(data, offset, length);
            return text.Replace("\t", @"[\t]");
        }
        
        /// <summary>
        /// Converts a text string into an array of bytes using the project's Encoding.
        /// </summary>
        /// <param name="text">The text string to be converted.</param>
        /// <returns>The converted byte array.</returns>
        public override byte[] GetBytes(string text)
        {
            return Encoding.GetBytes(text.Replace(@"[\t]", "\t"));
        }
    }
}
