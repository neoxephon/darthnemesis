//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-13</date>
// <summary>Common properties and methods for the entire game.</summary>
//-----------------------------------------------------------------------

namespace SummonTransX
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using DarthNemesis;
    
    /// <summary>
    /// Common properties and methods for the entire game.
    /// </summary>
    public class Game : AbstractGame
    {
        private static Dictionary<char, char> singleToDual = InitializeSingleToDual();
        private static Dictionary<char, char> dualToSingle = InitializeDualToSingle();
        private PreviewForm previewForm;
        
        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        public Game() : base()
        {
            this.previewForm = new PreviewForm(this);
        }
        
        /// <summary>
        /// Gets a dictionary that maps single-byte characters to their dual-byte equivalents.
        /// </summary>
        /// <value>A dictionary of single-byte-to-dual-byte mappings.</value>
        public static Dictionary<char, char> SingleToDual
        {
            get
            {
                return singleToDual;
            }
        }
        
        /// <summary>
        /// Gets a dictionary that maps dual-byte characters to their single-byte equivalents.
        /// </summary>
        /// <value>A dictionary of dual-byte-to-single-byte mappings.</value>
        public static Dictionary<char, char> DualToSingle
        {
            get
            {
                return dualToSingle;
            }
        }
        
        /// <summary>
        /// Gets the color to use for the glyphs in the font.
        /// </summary>
        /// <value>The color to use for character glyphs.</value>
        public static Color ForegroundColor
        {
            get
            {
                return Color.FromArgb(81, 56, 0);
            }
        }
        
        /// <summary>
        /// Gets the color to use for drop shadows.
        /// </summary>
        /// <value>The color to use for character drop shadows.</value>
        public static Color ShadowColor
        {
            get
            {
                return Color.FromArgb(232, 200, 72);
            }
        }
        
        /// <summary>
        /// Gets the color to use for the background of the preview window.
        /// </summary>
        /// <value>The color to use for the background.</value>
        public static Color BackgroundColor
        {
            get
            {
                return Color.FromArgb(240, 232, 112);
            }
        }
        
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The application name.</value>
        public override string Name
        {
            get
            {
                return "SummonTransX";
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
                return "v1.3";
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
                return "A text editing utility for the NDS\r\ngame Summon Night X: Tears Crown.";
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
        /// Adds any custom menu options for the game.
        /// </summary>
        /// <param name="menu">The menu strip to update.</param>
        public override void LoadMenu(MenuStrip menu)
        {
            ToolStripMenuItem previewMenuItem = new ToolStripMenuItem();
            previewMenuItem.Name = "textPreviewToolStripMenuItem";
            previewMenuItem.Size = new System.Drawing.Size(152, 22);
            previewMenuItem.Text = "Text &Preview";
            previewMenuItem.Click += new System.EventHandler(this.TextPreviewToolStripMenuItemClick);
            
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add(previewMenuItem);
            
            menu.Items.Insert(1, toolsMenu);
        }
        
        /// <summary>
        /// Populates the list of files in the game.
        /// </summary>
        public override void InitializeFiles()
        {
            IPackFile[] packFiles = new IPackFile[]
            {
                new DPKFile(@"confidence\ConfidenceDataText.dpk", this),
                new DPKFile(@"General\BattleInfText.dpk", this),
                new DPKFile(@"General\VoicePlayDataText.dpk", this),
                new DPKFile(@"map\field\FieldEventText.dpk", this)
            };
            
            foreach (IPackFile file in packFiles)
            {
                this.Files.PackFiles.Add(file);
            }
            
            ITextFile[] textFiles = new ITextFile[]
            {
                new DTXFile(@"battle\UnisonData.dtx", this),
                new DTXFile(@"confidence\ConfidenceList.dtx", this),
                new DTXFile(@"General\CharacterData.dtx", this),
                new DTXFile(@"General\GimmickData.dtx", this),
                new DTXFile(@"General\ItemData.dtx", this),
                new DTXFile(@"General\SkillDataBTL.dtx", this),
                new DTXFile(@"General\SkillDataFLD.dtx", this),
                new DTXFile(@"General\SummonData.dtx", this),
                new DTXFile(@"General\VoicePlayList.dtx", this),
                new DTXFile(@"map\field\ShopInf.dtx", this),
                new DTXFile(@"map\field\teleportData.dtx", this),
                new DTXFile(@"muumuu\MuumuuData.dtx", this),
                new DTXFile(@"progtext\St_Bt_Battle.dtx", this),
                new DTXFile(@"progtext\St_Ev_Ending.dtx", this),
                new DTXFile(@"progtext\St_Ge_General.dtx", this),
                new DTXFile(@"progtext\St_Ge_Option.dtx", this),
                new DTXFile(@"progtext\St_GE_Record.dtx", this),
                new DTXFile(@"progtext\St_Ge_Save.dtx", this),
                new DTXFile(@"progtext\St_Ge_Status.dtx", this),
                new DTXFile(@"progtext\St_Mu_muumuu.dtx", this),
                new DTXFile(@"progtext\St_Op_Voiceplay.dtx", this),
                new DTXFile(@"progtext\St_Pl_panel.dtx", this),
                new DTXFile(@"progtext\St_Save.dtx", this),
                new DTXFile(@"progtext\St_Sk_SkillInfo.dtx", this),
                new DTXFile(@"progtext\St_WM_Field.dtx", this)
            };
            
            foreach (ITextFile file in textFiles)
            {
                this.Files.TextFiles.Add(file);
            }
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data)
        {
            return GetText(data, 0);
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
                // End of string
                if (data[i] == 0x00)
                {
                    break;
                }
                else if (data[i] == 0xFF)
                {
                    switch (data[i + 1])
                    {
                        case 0x20:
                            text += "<br>";
                            break;
                        case 0x21:
                            text += @"\n";
                            break;
                        case 0x60:
                            text += "<neutral>";
                            break;
                        case 0x61:
                            text += "<happy>";
                            break;
                        case 0x62:
                            text += "<stern>";
                            break;
                        case 0x63:
                            text += "<surprised>";
                            break;
                        case 0x64:
                            text += "<upset>";
                            break;
                        case 0x65:
                            text += "<sad>";
                            break;
                        case 0x66:
                            text += "<tender>";
                            break;
                        case 0x67:
                            text += "<cheerful>";
                            break;
                        case 0x6B:
                            text += "<angry>";
                            break;
                        case 0xFB:
                            text += "<gold>";
                            break;
                        case 0xFC:
                            text += "<chars>";
                            break;
                        case 0xFE:
                            text += "<votes>";
                            break;
                        default:
                            throw new FormatException(string.Format(CultureInfo.CurrentCulture, "Unrecognized control code {0:X2}{1:X2} in line: {2}", data[i], data[i + 1], text));
                    }
                    
                    i++;
                }
                else if (data[i] < 0x80 || (data[i] >= 0xA0 && data[i] <= 0xDF))
                {
                    text += Encoding.GetString(data, i, 1);
                }
                else
                {
                    string character = Encoding.GetString(data, i, 2);
                    
                    // Single-byte ASCII characters cannot be displayed correctly in dialogue boxes.
                    // Remap them to equivalent double-byte characters in the custom font.
                    if (dualToSingle.ContainsKey(character[0]))
                    {
                        text += dualToSingle[character[0]];
                    }
                    else
                    {
                        text += character;
                    }
                    
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
                if (text.StartsWith(@"\n", StringComparison.Ordinal))
                {
                    bytes.Add(0xFF);
                    bytes.Add(0x21);
                    text = text.Substring(2);
                }
                else if (text.StartsWith("<", StringComparison.Ordinal))
                {
                    int endIndex = text.IndexOf(">", StringComparison.Ordinal);
                    if (endIndex == -1)
                    {
                        throw new FormatException("Found opening tag with no closing tag: " + text);
                    }
                    
                    string special = text.Substring(1, endIndex - 1);
                    text = text.Substring(endIndex + 1);
                    
                    if ("br".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x20);
                    }
                    else if ("neutral".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x60);
                    }
                    else if ("happy".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x61);
                    }
                    else if ("stern".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x62);
                    }
                    else if ("surprised".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x63);
                    }
                    else if ("upset".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x64);
                    }
                    else if ("sad".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x65);
                    }
                    else if ("tender".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x66);
                    }
                    else if ("cheerful".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x67);
                    }
                    else if ("angry".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0x6B);
                    }
                    else if ("gold".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0xFB);
                    }
                    else if ("chars".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0xFC);
                    }
                    else if ("votes".Equals(special))
                    {
                        bytes.Add(0xFF);
                        bytes.Add(0xFE);
                    }
                    else
                    {
                        throw new FormatException("Unrecognized control code: " + special);
                    }
                }
                else if (singleToDual.ContainsKey(text[0]))
                {
                    char[] replacement = new char[] { singleToDual[text[0]] };
                    byte[] charData = Encoding.GetBytes(replacement);
                    for (int i = 0; i < charData.Length; i++)
                    {
                        bytes.Add(charData[i]);
                    }
                    
                    text = text.Substring(1);
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
        
        private static Dictionary<char, char> InitializeSingleToDual()
        {
            string single = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            string dual = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψωАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклм";
            Dictionary<char, char> dict = new Dictionary<char, char>();
            for (int i = 0; i < dual.Length; i++)
            {
                dict.Add(single[i], dual[i]);
            }
            
            return dict;
        }
        
        private static Dictionary<char, char> InitializeDualToSingle()
        {
            string single = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            string dual = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψωАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклм";
            Dictionary<char, char> dict = new Dictionary<char, char>();
            for (int i = 0; i < dual.Length; i++)
            {
                dict.Add(dual[i], single[i]);
            }
            
            return dict;
        }
        
        private void TextPreviewToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.previewForm.Show();
        }
    }
}
