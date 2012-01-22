//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-06</date>
// <summary>Common properties and methods for the entire game.</summary>
//-----------------------------------------------------------------------

namespace MamoruTrans
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
                return "MamoruTrans";
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
                return "v0.2";
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
                return "A text editing utility for the Nintendo\r\nDS game Ore ga Omae wo Mamoru.";
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
            this.Files.TextFiles.Add(new ParmFile(@"data\Proj60.dat", this));
            this.Files.TextFiles.Add(new Arm9File(this));
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data)
        {
            return this.GetText(data, 0);
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset)
        {
            string text = string.Empty;
            for (int i = offset; i < data.Length; i++)
            {
                if (i < 0)
                {
                    return null;
                }
                else if (data[i] == 0x00)
                {
                    break;
                }
                else if (data[i] == 0x0D)
                {
                    text += @"\r";
                }
                else if (data[i] == 0x0A)
                {
                    text += @"\n";
                }
                else if (data[i] < 0x20)
                {
                    if (data[i] == 0x1B && data[i + 1] == 0x43)
                    {
                        text += string.Format(CultureInfo.InvariantCulture, "<C{0:x2}>", data[i + 2]);
                        i += 2;
                    }
                    else
                    {
                        text += string.Format(CultureInfo.InvariantCulture, "<${0:x2}>", data[i]);
                    }
                }
                else if (data[i] == 0x81 && data[i + 1] == 0xE1)
                {
                    text += "<[>";
                    i++;
                }
                else if (data[i] == 0x81 && data[i + 1] == 0xE2)
                {
                    text += "<]>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x54)
                {
                    text += "<I>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x55)
                {
                    text += "<II>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x56)
                {
                    text += "<III>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x85)
                {
                    text += "<icon:consumable>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x86)
                {
                    text += "<icon:ingredient>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x87)
                {
                    text += "<icon:keyitem>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x88)
                {
                    text += "<icon:sword1>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x89)
                {
                    text += "<icon:sword2>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x8A)
                {
                    text += "<icon:sword3>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x8B)
                {
                    text += "<icon:sword4>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x8C)
                {
                    text += "<icon:circle>";
                    i++;
                }
                else if (data[i] == 0x87 && data[i + 1] == 0x8D)
                {
                    text += "<icon:chest>";
                    i++;
                }
                else if (data[i] < 0x80 || (data[i] >= 0xA0 && data[i] <= 0xDF))
                {
                    text += Encoding.GetString(data, i, 1);
                }
                else
                {
                    text += Encoding.GetString(data, i, 2);
                    i++;
                }
            }
            
            return text;
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
            List<byte> bytes = new List<byte>();
            while (text.Length > 0)
            {
                if (text.StartsWith(@"\r", StringComparison.Ordinal))
                {
                    bytes.Add(0x0D);
                    text = text.Substring(2);
                }
                else if (text.StartsWith(@"\n", StringComparison.Ordinal))
                {
                    bytes.Add(0x0A);
                    text = text.Substring(2);
                }
                else if (text.StartsWith("<", StringComparison.Ordinal))
                {
                    int endIndex = text.IndexOf(">", StringComparison.Ordinal);
                    string special = text.Substring(1, endIndex - 1);
                    text = text.Substring(endIndex + 1);
                    if (special.StartsWith("C", StringComparison.Ordinal))
                    {
                        bytes.Add(0x1B);
                        bytes.Add(0x43);
                        bytes.Add(Convert.ToByte(special.Substring(1, special.Length - 1), 16));
                    }
                    else if (special.StartsWith("$", StringComparison.Ordinal))
                    {
                        bytes.Add(Convert.ToByte(special.Substring(1, special.Length - 1), 16));
                    }
                    else if (special.Equals("["))
                    {
                        bytes.Add(0x81);
                        bytes.Add(0xE1);
                    }
                    else if (special.Equals("]"))
                    {
                        bytes.Add(0x81);
                        bytes.Add(0xE2);
                    }
                    else if (special.Equals("I"))
                    {
                        bytes.Add(0x87);
                        bytes.Add(0x54);
                    }
                    else if (special.Equals("II"))
                    {
                        bytes.Add(0x87);
                        bytes.Add(0x55);
                    }
                    else if (special.Equals("III"))
                    {
                        bytes.Add(0x87);
                        bytes.Add(0x56);
                    }
                    else
                    {
                        throw new FormatException("Unrecognized control code \"" + special + "\"");
                    }
                }
                else
                {
                    byte[] charData = Encoding.GetBytes(text.Substring(0, 1));
                    for (int i = 0; i < charData.Length; i++)
                    {
                        bytes.Add(charData[i]);
                    }
                    
                    text = text.Substring(1);
                }
            }
            
            return bytes.ToArray();
        }
    }
}
