//-----------------------------------------------------------------------
// <copyright file="NitroHeader.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-12-31</date>
// <summary>The standard header for all Nitro files.</summary>
//-----------------------------------------------------------------------

namespace Nitro
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    
    /// <summary>
    /// The standard header for all Nitro files.
    /// </summary>
    public class NitroHeader : AbstractChunk
    {
        #region Variables
        ////private const int ByteOrderMark = 0x0101FEFF;
        private const ushort HeaderSize = 0x10;
        private int signature;
        private int byteOrderMark;
        private uint fileSize;
        private ushort sectionCount;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the NitroHeader class.
        /// Sets the expected file signature to the given 4-byte value.
        /// </summary>
        /// <param name="signature">The 4-byte file signature to expect.</param>
        public NitroHeader(int signature)
        {
            this.signature = signature;
        }
        
        /// <summary>
        /// Initializes a new instance of the NitroHeader class.
        /// Sets the expected file signature to the given 4-character value.
        /// </summary>
        /// <param name="signature">The 4-character file signature to expect.</param>
        public NitroHeader(string signature)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(signature);
            if (bytes.Length != 4)
            {
                throw new FormatException("Signature must be 4 characters.");
            }
            
            this.signature = BitConverter.ToInt32(bytes, 0);
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
                return HeaderSize;
            }
        }
        
        /// <summary>
        /// Gets or sets the size of the entire file.
        /// </summary>
        /// <value>The overall file size.</value>
        public long FileSize
        {
            get
            {
                return this.fileSize;
            }
            
            set
            {
                this.fileSize = (uint)value;
            }
        }
        
        /// <summary>
        /// Gets or sets the number of sections in the file.
        /// </summary>
        /// <value>The number of sections, not including the header.</value>
        public int SectionCount
        {
            get
            {
                return this.sectionCount;
            }
            
            set
            {
                this.sectionCount = (ushort)value;
            }
        }
        #endregion
        
        /// <summary>
        /// Populates the header values from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which values will be read.</param>
        public override void ReadFrom(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            
            int signature = reader.ReadInt32();
            if (signature != this.signature)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, "File signature \"{0}\" does not match expected value \"{1}\"", signature, this.signature);
                throw new FormatException(msg);
            }
            
            this.byteOrderMark = reader.ReadInt32();
            /*
            int byteOrderMark = reader.ReadInt32();
            if (byteOrderMark != ByteOrderMark)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, "Byte order mark \"0x{0:x4}\" does not match expected value \"0x{1:x4}\"", byteOrderMark, ByteOrderMark);
                throw new FormatException(msg);
            }
            */
            
            this.fileSize = reader.ReadUInt32();
            
            int headerSize = reader.ReadUInt16();
            if (headerSize != HeaderSize)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, "Header size \"{0}\" does not match expected value \"{1}\"", headerSize, this.Size);
                throw new FormatException(msg);
            }
            
            this.sectionCount = reader.ReadUInt16();
        }
        
        /// <summary>
        /// Writes the header back into the given stream.
        /// </summary>
        /// <param name="stream">The stream to which values will be written.</param>
        public override void WriteTo(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(this.signature);
            ////writer.Write(ByteOrderMark);
            writer.Write(this.byteOrderMark);
            writer.Write(this.fileSize);
            writer.Write(HeaderSize);
            writer.Write(this.sectionCount);
        }
    }
}
