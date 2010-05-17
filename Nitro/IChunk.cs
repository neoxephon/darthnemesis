//-----------------------------------------------------------------------
// <copyright file="IChunk.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-05-13</date>
// <summary>Standard interface for Nitro file chunks.</summary>
//-----------------------------------------------------------------------

namespace Nitro
{
    using System;
    using System.IO;
    
    /// <summary>
    /// Standard interface for Nitro file chunks.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Gets the size of the chunk in bytes.
        /// </summary>
        /// <value>The size of this chunk.</value>
        int Size
        {
            get;
        }
        
        /// <summary>
        /// Populates the chunk from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which the chunk will be read.</param>
        void ReadFrom(Stream stream);
        
        /// <summary>
        /// Populates the chunk from the given byte array.
        /// </summary>
        /// <param name="data">The array from which the chunk will be read.</param>
        /// <param name="offset">The index from which to start reading.</param>
        void ReadFrom(byte[] data, int offset);
        
        /// <summary>
        /// Writes the section back into the given stream.
        /// </summary>
        /// <param name="stream">The stream to which values will be written.</param>
        void WriteTo(Stream stream);
    }
}
