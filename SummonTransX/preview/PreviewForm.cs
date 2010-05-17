//-----------------------------------------------------------------------
// <copyright file="PreviewForm.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-11-14</date>
// <summary>A form for viewing lines as they will appear in-game.</summary>
//-----------------------------------------------------------------------

namespace SummonTransX
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Nitro.Font;

    /// <summary>
    /// Description of PreviewForm.
    /// </summary>
    public partial class PreviewForm : Form
    {
        private static readonly List<char> validDec = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        private Bitmap dialogBuffer;
        private Bitmap backBuffer;
        private ColorMap[] colorMap;
        private ColorMap[] styleMap;
        private NitroFont font;
        private Game game;
        
        /// <summary>
        /// Initializes a new instance of the PreviewForm class.
        /// </summary>
        /// <param name="game">Common properties and methods for the entire game.</param>
        public PreviewForm(Game game)
        {
            this.InitializeComponent();
            this.game = game;
            this.colorMap = new ColorMap[1];
            this.colorMap[0] = new ColorMap();
            this.colorMap[0].OldColor = Color.Black;
            this.colorMap[0].NewColor = Game.ForegroundColor;
            this.styleMap = new ColorMap[1];
            this.styleMap[0] = new ColorMap();
            this.styleMap[0].OldColor = Color.Black;
            this.styleMap[0].NewColor = Game.ShadowColor;
            this.ResizeDialogBox();
            this.comboBoxSize.SelectedIndex = 0;
            this.comboBoxSize.Enabled = false;
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Redirecting to log")]
        private void LoadFont()
        {
            string fileName = @"Font.nftr";
            try
            {
                this.font = new NitroFont(fileName);
                this.font.Load();
            }
            catch (SystemException)
            {
                this.ErrorMessage("The font file could not be loaded.\r\nRun this program from your dslazy folder!");
            }
            finally
            {
                this.RedrawDialogBox();
            }
        }
        
        private void ErrorMessage(string text)
        {
            MessageBoxOptions options = (this.RightToLeft == RightToLeft.Yes)
                    ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
                    : 0;
            
            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
        }
        
        private void ResizeDialogBox()
        {
            // Ignore a resize event triggered by minimizing the window
            if (this.panelDialogBox.Height == 0)
            {
                return;
            }
            
            if (this.backBuffer != null)
            {
                this.backBuffer.Dispose();
            }
            
            this.backBuffer = new Bitmap(this.panelDialogBox.Width, this.panelDialogBox.Height);
            
            if (this.dialogBuffer != null)
            {
                this.dialogBuffer.Dispose();
            }
            
            this.dialogBuffer = new Bitmap(this.panelDialogBox.Width, this.panelDialogBox.Height);
            
            using (Graphics g = Graphics.FromImage(this.backBuffer))
            {
                g.Clear(BackColor);
            }
            
            this.RedrawDialogBox();
        }
        
        private void RedrawDialogBox()
        {
            if (this.font == null)
            {
                return;
            }
            
            string line = this.textBoxLine.Text;
            char[] chars = line.ToCharArray();
            int startX = 3;
            int startY = 3;
            int curX = startX;
            int curY = startY;
            int lineHeight = 16;
            int maxY = (int)Math.Ceiling((this.panelDialogBox.Height * 1.0 / lineHeight) - 1) * lineHeight;
            int maxWidth = Convert.ToInt32(this.textBoxLineLength.Text, CultureInfo.InvariantCulture) - startX;
            int padding = Convert.ToInt32(this.textBoxPadding.Text, CultureInfo.InvariantCulture);
            Bitmap tempBuffer = new Bitmap(this.backBuffer);
            
            using (Graphics g = Graphics.FromImage(tempBuffer))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                ////colorMap[0].NewColor = penColors[0];
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(this.colorMap);
                ImageAttributes shadowAttr = new ImageAttributes();
                shadowAttr.SetRemapTable(this.styleMap);
                Rectangle rect;
                
                g.FillRectangle(new SolidBrush(Game.BackgroundColor), 0, 0, maxWidth, tempBuffer.Height);
                
                for (int i = 0; i < line.Length; i++)
                {
                    string remaining = line.Substring(i);
                    
                    if (curY > maxY)
                    {
                        break;
                    }
                    else if (remaining.StartsWith(@"\n", StringComparison.Ordinal) || remaining.StartsWith("\r\n", StringComparison.Ordinal))
                    {
                        curX = startX;
                        curY += lineHeight;
                        i++;
                        continue;
                    }
                    else if (remaining.StartsWith("<", StringComparison.Ordinal))
                    {
                        if (remaining.StartsWith("<br>", StringComparison.Ordinal))
                        {
                            curX = startX;
                            curY += lineHeight;
                        }
                        
                        int endIndex = remaining.IndexOf(">", StringComparison.Ordinal);
                        if (endIndex != -1)
                        {
                            i += endIndex;
                            continue;
                        }
                    }
                    else if (remaining.StartsWith(@"\r\n", StringComparison.Ordinal))
                    {
                        curX = startX;
                        curY += lineHeight;
                        i += 3;
                        continue;
                    }
                    
                    byte[] charBytes;
                    if (Game.SingleToDual.ContainsKey(remaining[0]))
                    {
                        char[] replacement = new char[] { Game.SingleToDual[remaining[0]] };
                        charBytes = this.game.Encoding.GetBytes(replacement);
                    }
                    else
                    {
                        charBytes = this.game.Encoding.GetBytes(chars, i, 1);
                    }
                    
                    ushort code = 0;
                    if (charBytes.Length == 1)
                    {
                        code = charBytes[0];
                    }
                    else if (charBytes.Length == 2)
                    {
                        code = (ushort)((charBytes[0] << 8) + charBytes[1]);
                    }
                    
                    Character curTile = this.font.GetCharacter(code);
                    if (curTile != null)
                    {
                        int w = curTile.NextOffset;
                        int h = curTile.Height;
                        if (curX + w > maxWidth)
                        {
                            curX = startX;
                            curY += lineHeight;
                        }
                        
                        if (curY > maxY)
                        {
                            break;
                        }
                        
                        rect = new Rectangle(curX + curTile.XOffset + 1, curY, w, h);
                        g.DrawImage(curTile.Glyph, rect, 0, 0, w, h, GraphicsUnit.Pixel, shadowAttr);
                        rect = new Rectangle(curX + curTile.XOffset, curY + 1, w, h);
                        g.DrawImage(curTile.Glyph, rect, 0, 0, w, h, GraphicsUnit.Pixel, shadowAttr);
                        rect = new Rectangle(curX + curTile.XOffset, curY, w, h);
                        g.DrawImage(curTile.Glyph, rect, 0, 0, w, h, GraphicsUnit.Pixel, attr);
                        curX += w + padding;
                    }
                }
            }
            
            using (Graphics g = Graphics.FromImage(this.dialogBuffer))
            {
                g.DrawImageUnscaled(tempBuffer, 0, 0);
            }
            
            this.panelDialogBox.Invalidate();
        }
        
        #region Event Handlers
        private void PanelDialogBoxPaint(object sender, PaintEventArgs e)
        {
            using (Graphics g = this.panelDialogBox.CreateGraphics())
            {
                g.DrawImageUnscaled(this.dialogBuffer, 0, 0);
            }
        }
        
        private void PanelDialogBoxResize(object sender, EventArgs e)
        {
            this.ResizeDialogBox();
        }
        
        private void ComboBoxSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            ////this.LoadFont(this.comboBoxSize.Text);
            this.LoadFont();
        }
        
        private void TextBoxPaddingTextChanged(object sender, EventArgs e)
        {
            if (this.textBoxPadding.Text.Length > 0)
            {
                this.RedrawDialogBox();
            }
        }
        
        private void TextBoxLineLengthTextChanged(object sender, EventArgs e)
        {
            if (this.textBoxLineLength.Text.Length > 0)
            {
                this.RedrawDialogBox();
            }
        }
        
        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            // Reject everything but backspace and decimal numbers
            if (e.KeyChar != '\u0008' && !validDec.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        private void TextBoxLineTextChanged(object sender, EventArgs e)
        {
            this.RedrawDialogBox();
        }
        
        private void PreviewFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        #endregion
    }
}
