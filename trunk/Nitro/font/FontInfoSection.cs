//-----------------------------------------------------------------------
// <copyright file="FontInfoSection.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2008-09-09</date>
// <summary>Contains properties that apply to the entire font.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Globalization;
    using System.IO;
    
    /// <summary>
    /// Contains properties that apply to the entire font.
    /// </summary>
    public class FontInfoSection : AbstractChunk
    {
        #region Variables
        private const int Signature = 0x46494E46; // FINF
        private int unknown1;
        private int unknown2;
        private int glyphSectionOffset;
        private int widthSectionOffset;
        private int mapSectionOffset;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the FontInfoSection class.
        /// Reads the section from the given stream.
        /// </summary>
        public FontInfoSection()
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
                return 0x1C;
            }
        }
        
        /// <summary>
        /// Gets or sets the offset of the CGLP section in the file.
        /// </summary>
        /// <value>The offset of the CGLP section relative to the start of the file.</value>
        public int GlyphSectionOffset
        {
            get
            {
                return this.glyphSectionOffset - 8;
            }
            
            set
            {
                this.glyphSectionOffset = value + 8;
            }
        }
        
        /// <summary>
        /// Gets or sets the offset of the first CWDH section in the file.
        /// </summary>
        /// <value>The offset of the first CWDH section relative to the start of the file.</value>
        public int WidthSectionOffset
        {
            get
            {
                return this.widthSectionOffset - 8;
            }
            
            set
            {
                this.widthSectionOffset = value + 8;
            }
        }
        
        /// <summary>
        /// Gets or sets the offset of the first CMAP section in the file.
        /// </summary>
        /// <value>The offset of the first CMAP section relative to the start of the file.</value>
        public int MapSectionOffset
        {
            get
            {
                return this.mapSectionOffset - 8;
            }
            
            set
            {
                this.mapSectionOffset = value + 8;
            }
        }
        #endregion
        
        /// <summary>
        /// Reads the chunk from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the chunk.</param>
        public override void ReadFrom(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            
            int signature = reader.ReadInt32();
            if (signature != Signature)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, "Section at offset 0x{0:x8} with signature \"0x{1:x4}\" does not match expected value \"0x{2:x4}\"", stream.Position - 4, signature, Signature);
                throw new FormatException(msg);
            }
            
            int sectionSize = reader.ReadInt32();
            if (sectionSize != this.Size)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, "Section size \"{0}\" does not match expected value \"{1}\"", sectionSize, this.Size);
                throw new FormatException(msg);
            }
            
            this.unknown1 = reader.ReadInt32();
            this.unknown2 = reader.ReadInt32();
            this.glyphSectionOffset = reader.ReadInt32();
            this.widthSectionOffset = reader.ReadInt32();
            this.mapSectionOffset = reader.ReadInt32();
        }
        
        /// <summary>
        /// Writes the chunk to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the chunk will be written.</param>
        public override void WriteTo(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Signature);
            writer.Write(this.Size);
            writer.Write(this.unknown1);
            writer.Write(this.unknown2);
            writer.Write(this.glyphSectionOffset);
            writer.Write(this.widthSectionOffset);
            writer.Write(this.mapSectionOffset);
        }
    }
}
