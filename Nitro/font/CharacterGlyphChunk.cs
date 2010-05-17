//-----------------------------------------------------------------------
// <copyright file="CharacterGlyphChunk.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2008-09-09</date>
// <summary>Contains graphical data for all character glyphs.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Contains graphical data for all character glyphs.
    /// </summary>
    public class CharacterGlyphChunk : AbstractChunk
    {
        #region Variables
        private const int Signature = 0x43474C50;
        private const int HeaderSize = 0x10;
        private const byte Padding = 0x00;
        private int characterCount;
        private byte unknown1;
        private byte unknown2;
        private ushort bitsPerPixel;
        private List<Bitmap> glyphs;
        private Color[] colors;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the CharacterGlyphChunk class.
        /// </summary>
        /// <param name="characterCount">The number of characters in the font.</param>
        public CharacterGlyphChunk(int characterCount)
        {
            this.characterCount = characterCount;
        }
        
        #region Properties
        
        /// <summary>
        /// Gets the size of the chunk in bytes.
        /// </summary>
        /// <value>The size of this chunk.</value>
        public override int Size
        {
            get
            {
                return RoundUp(HeaderSize + (this.glyphs.Count * this.BytesPerGlyph));
            }
        }
        
        /*
        public byte MaxWidth
        {
            get
            {
                return maxGlyphWidth;
            }
        }
        
        public byte MaxHeight
        {
            get
            {
                return maxGlyphHeight;
            }
        }
         */
        
        /// <summary>
        /// Gets the number of bits required to represent each pixel.
        /// </summary>
        /// <value>The number of bits required to represent a pixel.</value>
        public int BitsPerPixel
        {
            get
            {
                return this.bitsPerPixel;
            }
        }
        
        /// <summary>
        /// Gets the list of all graphical glyphs in the font.
        /// </summary>
        /// <value>A list containing the font's graphical glyphs.</value>
        public IList<Bitmap> Glyphs
        {
            get
            {
                return this.glyphs;
            }
        }
        
        private int MaxGlyphWidth
        {
            get
            {
                return this.glyphs[0].Width;
            }
        }
        
        private int MaxGlyphHeight
        {
            get
            {
                return this.glyphs[0].Height;
            }
        }
        
        private ushort BytesPerGlyph
        {
            get
            {
                return (ushort)Math.Ceiling(this.MaxGlyphWidth * this.MaxGlyphHeight * this.bitsPerPixel / 8.0);
            }
        }
        #endregion
        
        /// <summary>
        /// Reads the chunk from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the chunk.</param>
        public override void ReadFrom(Stream stream)
        {
            // Read header
            BinaryReader reader = new BinaryReader(stream);
            int signature = reader.ReadInt32();
            if (signature != Signature)
            {
                throw new FormatException("Chunk is not CGLP format.");
            }
            
            reader.ReadInt32(); // chunk size; ignore
            int maxGlyphWidth = reader.ReadByte();
            int maxGlyphHeight = reader.ReadByte();
            int bytesPerGlyph = reader.ReadUInt16();
            this.unknown1 = reader.ReadByte();
            this.unknown2 = reader.ReadByte();
            this.bitsPerPixel = reader.ReadUInt16();
            
            // Read body
            this.GenerateColors();
            this.glyphs = new List<Bitmap>(this.characterCount);
            byte byteData = reader.ReadByte();
            for (int i = 0; i < this.characterCount; i++)
            {
                // Create blank glyph
                Bitmap glyph = new Bitmap(maxGlyphWidth, maxGlyphHeight);
                using (Graphics g = Graphics.FromImage(glyph))
                {
                    g.Clear(this.colors[0]);
                }
                
                int byteIndex = 0;
                int x = 0;
                int y = 0;
                
                ushort bitMask = 0x80;
                
                while (byteIndex < bytesPerGlyph && y < maxGlyphHeight)
                {
                    byte intensityMask = (byte)Math.Pow(2, this.bitsPerPixel - 1);
                    byte intensity = 0;
                    for (; intensityMask > 0; intensityMask >>= 1)
                    {
                        if ((byteData & bitMask) > 0)
                        {
                            intensity += intensityMask;
                        }
                        
                        bitMask >>= 1;
                        if (bitMask == 0)
                        {
                            bitMask = 0x80;
                            byteIndex++;
                            byteData = reader.ReadByte();
                        }
                    }
                    
                    if (intensity > 0)
                    {
                        glyph.SetPixel(x, y, this.colors[intensity]);
                    }
                    
                    x++;
                    if (x >= maxGlyphWidth)
                    {
                        x = 0;
                        y++;
                    }
                }
                
                while (byteIndex < bytesPerGlyph)
                {
                    byteData = reader.ReadByte();
                    byteIndex++;
                }
                
                this.glyphs.Add(glyph);
            }
            
            reader.BaseStream.Position--;
            while (reader.BaseStream.Position < RoundUp(reader.BaseStream.Position))
            {
                reader.ReadByte();
            }
        }
        
        /// <summary>
        /// Repopulates the graphical glyphs from the given font.
        /// </summary>
        /// <param name="font">The font to be read from.</param>
        public void ReadFrom(NitroFont font)
        {
            IList<int> codes = font.Codes;
            IList<Character> characters = font.Characters;
            this.characterCount = characters.Count;
            
            List<Bitmap> newGlyphs = new List<Bitmap>(this.characterCount);
            
            foreach (int code in codes)
            {
                Character character = font.GetCharacter(code);
                newGlyphs.Add(new Bitmap(character.Glyph));
            }
            
            foreach (Bitmap bitmap in this.glyphs)
            {
                bitmap.Dispose();
            }
            
            this.glyphs = newGlyphs;
        }
        
        /// <summary>
        /// Writes the chunk to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the chunk will be written.</param>
        public override void WriteTo(Stream stream)
        {
            // Write header
            BinaryWriter writer = new BinaryWriter(stream);
            
            byte maxGlyphWidth = (byte)this.glyphs[0].Width;
            byte maxGlyphHeight = (byte)this.glyphs[0].Height;
            
            writer.Write(Signature);
            writer.Write(this.Size);
            writer.Write((byte)this.MaxGlyphWidth);
            writer.Write((byte)this.MaxGlyphHeight);
            writer.Write(this.BytesPerGlyph);
            writer.Write(this.unknown1);
            writer.Write(this.unknown2);
            writer.Write(this.bitsPerPixel);
            
            // Write body
            short bitIndex;
            foreach (Bitmap glyph in this.glyphs)
            {
                bitIndex = (byte)(8 - this.bitsPerPixel);
                byte byteData = 0;
                int x = 0;
                int y = 0;
                
                while (y < maxGlyphHeight)
                {
                    Color c = glyph.GetPixel(x, y);
                    byte colorIndex = this.GetColorIndex(c);
                    if (bitIndex >= 0)
                    {
                        byteData |= (byte)(colorIndex << bitIndex);
                    }
                    else
                    {
                        ushort ushortData = (ushort)(colorIndex << (8 + bitIndex));
                        byteData |= (byte)(ushortData >> 8);
                        writer.Write(byteData);
                        byteData = (byte)(ushortData & 0xFF);
                        bitIndex += 8;
                    }
                    
                    if (bitIndex == 0)
                    {
                        bitIndex = (byte)(8 - this.bitsPerPixel);
                        writer.Write(byteData);
                        byteData = 0;
                    }
                    else
                    {
                        bitIndex -= (byte)this.bitsPerPixel;
                    }
                    
                    x++;
                    if (x >= maxGlyphWidth)
                    {
                        x = 0;
                        y++;
                    }
                }
                
                if (bitIndex != 8 - this.bitsPerPixel)
                {
                    writer.Write(byteData);
                }
            }
            
            writer.Flush();
            while (writer.BaseStream.Position < RoundUp(writer.BaseStream.Position))
            {
                writer.Write(Padding);
            }
        }
        
        /// <summary>
        /// Resizes all glyphs in the font, preserving their existing graphics.
        /// Aligned to the bottom left corner.
        /// </summary>
        /// <param name="width">The new glyph width.</param>
        /// <param name="height">The new glyph height.</param>
        public void Resize(byte width, byte height)
        {
            int y = height - this.MaxGlyphHeight;
            for (int i = 0; i < this.glyphs.Count; i++)
            {
                Bitmap b = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImageUnscaled(this.glyphs[i], 0, y);
                }
                
                this.glyphs[i].Dispose();
                this.glyphs[i] = b;
            }
        }
        
        private static int RoundUp(int offset)
        {
            return (int)(Math.Ceiling((double)offset / 0x04) * 0x04);
        }
        
        private static long RoundUp(long offset)
        {
            return (long)(Math.Ceiling((double)offset / 0x04) * 0x04);
        }
        
        private void GenerateColors()
        {
            int numColors = (int)Math.Pow(2, this.bitsPerPixel);
            this.colors = new Color[numColors];
            for (int i = 0; i < numColors; i++)
            {
                int transparency = 255 * i / (numColors - 1);
                this.colors[i] = Color.FromArgb(transparency, 0, 0, 0);
            }
        }
        
        private byte GetColorIndex(Color c)
        {
            for (byte i = 0; i < this.colors.Length; i++)
            {
                if (c.A == this.colors[i].A)
                {
                    return i;
                }
            }
            
            return 0;
        }
    }
}
