//-----------------------------------------------------------------------
// <copyright file="AbstractChunk.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-15</date>
// <summary>Manages common elements for file chunks.</summary>
//-----------------------------------------------------------------------

namespace Nitro
{
    using System;
    using System.IO;
    
    /// <summary>
    /// Manages common elements for file chunks.
    /// </summary>
    public abstract class AbstractChunk
    {
        /// <summary>
        /// Gets the size of the chunk in bytes.
        /// </summary>
        /// <value>The size of this chunk.</value>
        public abstract int Size
        {
            get;
        }
        
        /// <summary>
        /// Populates the chunk from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which the chunk will be read.</param>
        public abstract void ReadFrom(Stream stream);
        
        /// <summary>
        /// Populates the chunk from the given byte array.
        /// </summary>
        /// <param name="data">The array from which the chunk will be read.</param>
        /// <param name="offset">The index from which to start reading.</param>
        public void ReadFrom(byte[] data, int offset)
        {
            MemoryStream stream = new MemoryStream(data, offset, data.Length - offset, false);
            this.ReadFrom(stream);
        }
        
        /// <summary>
        /// Writes the section back into the given stream.
        /// </summary>
        /// <param name="stream">The stream to which values will be written.</param>
        public abstract void WriteTo(Stream stream);
    }
}
