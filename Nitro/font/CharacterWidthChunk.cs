//-----------------------------------------------------------------------
// <copyright file="CharacterWidthChunk.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-15</date>
// <summary>Contains width/offset information for each character in a font.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    
    /// <summary>
    /// Contains width/offset information for each character in a font.
    /// </summary>
    public class CharacterWidthChunk : AbstractChunk
    {
        #region Variables
        private const int Signature = 0x43574448; // CWDH
        private const int HeaderSize = 0x10;
        private const byte Padding = 0x00;
        private int unknown;
        private byte[] widths;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Valid")]
        private byte[] xOffsets;
        private byte[] nextOffsets;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the CharacterWidthChunk class.
        /// </summary>
        public CharacterWidthChunk()
        {
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
                return this.PaddedSize;
            }
        }
        
        /// <summary>
        /// Gets a list of widths for each character in the font, not including margins.
        /// </summary>
        /// <value>A list of character widths.</value>
        public IList<byte> Widths
        {
            get
            {
                return new List<byte>(this.widths);
            }
        }
        
        /// <summary>
        /// Gets a list of x-offsets for each character in the font.
        /// </summary>
        /// <value>A list of character offsets.</value>
        public IList<byte> XOffsets
        {
            get
            {
                return new List<byte>(this.xOffsets);
            }
        }
        
        /// <summary>
        /// Gets a list values indicating how much total space each character occupies on the screen.
        /// </summary>
        /// <value>A list of overall character widths.</value>
        public IList<byte> NextOffsets
        {
            get
            {
                return new List<byte>(this.nextOffsets);
            }
        }
        
        private int ActualSize
        {
            get
            {
                return HeaderSize + (3 * this.widths.Length);
            }
        }
        
        private int PaddedSize
        {
            get
            {
                return RoundUp(this.ActualSize);
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
                throw new FormatException("Chunk is not CWDH format.");
            }
            
            reader.ReadInt32(); // chunk size; ignore
            ushort minIndex = reader.ReadUInt16();
            ushort maxIndex = reader.ReadUInt16();
            this.unknown = reader.ReadInt32();
            if (this.unknown != 0x00000000)
            {
                throw new FormatException("Unrecognized CWDH unk1 value: " + this.unknown);
            }
            
            // Read body
            int characterCount = 1 + maxIndex - minIndex;
            this.xOffsets = new byte[characterCount];
            this.widths = new byte[characterCount];
            this.nextOffsets = new byte[characterCount];
            
            for (int i = 0; i < characterCount; i++)
            {
                this.xOffsets[i] = reader.ReadByte();
                this.widths[i] = reader.ReadByte();
                this.nextOffsets[i] = reader.ReadByte();
            }
        }
        
        /// <summary>
        /// Repopulates the character widths from the given font.
        /// </summary>
        /// <param name="font">The font to be read from.</param>
        public void ReadFrom(NitroFont font)
        {
            IList<int> codes = font.Codes;
            IList<Character> characters = font.Characters;
            this.xOffsets = new byte[characters.Count];
            this.widths = new byte[characters.Count];
            this.nextOffsets = new byte[characters.Count];
            
            for (int i = 0; i < codes.Count; i++)
            {
                Character character = font.GetCharacter(codes[i]);
                this.xOffsets[i] = character.XOffset;
                this.widths[i] = character.Width;
                this.nextOffsets[i] = character.NextOffset;
            }
        }
        
        /// <summary>
        /// Writes the chunk to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the chunk will be written.</param>
        public override void WriteTo(Stream stream)
        {
            // Write header
            BinaryWriter writer = new BinaryWriter(stream);
            int characterCount = this.widths.Length;
            ushort minIndex = 0x00;
            ushort maxIndex = (ushort)(characterCount - 1);
            
            writer.Write(Signature);
            writer.Write(this.Size);
            writer.Write(minIndex);
            writer.Write(maxIndex);
            writer.Write(this.unknown);
            
            // Write body
            for (int i = 0; i < characterCount; i++)
            {
                writer.Write(this.xOffsets[i]);
                writer.Write(this.widths[i]);
                writer.Write(this.nextOffsets[i]);
            }
            
            for (int i = this.ActualSize; i < this.PaddedSize; i++)
            {
                writer.Write(Padding);
            }
            
            writer.Flush();
        }
        
        private static int RoundUp(int offset)
        {
            return (int)(Math.Ceiling((double)offset / 0x04) * 0x04);
        }
    }
}
