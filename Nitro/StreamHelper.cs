//-----------------------------------------------------------------------
// <copyright file="StreamHelper.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2009-12-18</date>
// <summary>Helper methods for stream I/O.</summary>
//-----------------------------------------------------------------------

namespace Nitro
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    
    /// <summary>
    /// Description of StreamHelper.
    /// </summary>
    internal sealed class StreamHelper
    {
        private StreamHelper()
        {
        }
        
        /// <summary>
        /// Reads as much of the stream as will fit into the provided buffer.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="data">The buffer into which the stream is read.</param>
        public static void ReadStream(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException(String.Format(CultureInfo.CurrentCulture, "End of stream reached with {0} bytes left to read", remaining));
                }
                
                remaining -= read;
                offset += read;
            }
        }
        
        /// <summary>
        /// Reads the specified number of bytes from the stream into memory.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="length">The maximum number of bytes to read.</param>
        /// <returns>A buffer containing the contents read from the stream.</returns>
        public static byte[] ReadStream(Stream stream, int length)
        {
            byte[] streamData = new byte[length];
            ReadStream(stream, streamData);
            return streamData;
        }
        
        /// <summary>
        /// Reads the entire stream into memory.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <returns>A buffer containing the contents of the entire stream.</returns>
        public static byte[] ReadStream(Stream stream)
        {
            return ReadStream(stream, (int)stream.Length);
        }
        
        /// <summary>
        /// Returns the entire contents of the given file.
        /// </summary>
        /// <param name="fileName">The path of the file to be read.</param>
        /// <returns>A byte array containing the file contents.</returns>
        public static byte[] ReadFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] fileData = ReadStream(file);
            file.Close();
            return fileData;
        }
        
        /// <summary>
        /// Writes the data to the given file, creating or overwriting it.
        /// </summary>
        /// <param name="fileName">The path of the file to be written to.</param>
        /// <param name="data">A byte array containing the new contents of the file.</param>
        public static void WriteFile(string fileName, byte[] data)
        {
            string directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName) && directoryName.Length > 0)
            {
                Directory.CreateDirectory(directoryName);
            }
            
            FileStream file = null;
            try
            {
                file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                file.Write(data, 0, data.Length);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
    }
}
