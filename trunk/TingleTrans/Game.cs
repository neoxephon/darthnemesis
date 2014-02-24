//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="DarthNemesis">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2014-02-15</date>
// <summary>Common properties and methods for the entire game.</summary>
//-----------------------------------------------------------------------

namespace TingleTrans
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
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
                return "TingleTrans";
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
                return "v1.0";
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
                return "A text editing utility for the Nintendo\r\nDS game Tingle no Koi no Balloon Trip.";
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
                return Encoding.GetEncoding("Shift_JIS");
            }
        }
        
        /// <summary>
        /// Populates the list of files in the game.
        /// </summary>
        public override void InitializeFiles()
        {
            this.Files.TextFiles.Add(new LanguageDatabase(@"db\lang.bin", this));
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data)
        {
            throw new NotSupportedException("Unused");
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset)
        {
            throw new NotSupportedException("Unused");
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <param name="length">The maximum number of bytes to convert.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset, int length)
        {
            throw new NotSupportedException("Unused");
        }
        
        /// <summary>
        /// Converts a text string into an array of bytes using the project's Encoding.
        /// </summary>
        /// <param name="text">The text string to be converted.</param>
        /// <returns>The converted byte array.</returns>
        public override byte[] GetBytes(string text)
        {
            throw new NotSupportedException("Unused");
        }
    }
}
