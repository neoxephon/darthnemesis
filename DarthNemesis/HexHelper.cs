//-----------------------------------------------------------------------
// <copyright file="HexHelper.cs" company="DarthNemesis">
// Copyright (c) 2012 All Right Reserved
// Based on byte manipulation code by PZahra
// </copyright>
// <author>DarthNemesis</author>
// <date>2012-01-28</date>
// <summary>Helper methods for hex conversion.</summary>
//-----------------------------------------------------------------------

namespace DarthNemesis
{
    using System;
    
    /// <summary>
    /// Helper methods for hex conversion.
    /// </summary>
    public static class HexHelper
    {
        /// <summary>
        /// A helper method for converting an array of bytes to a hexadecimal string.
        /// </summary>
        /// <param name="array">The bytes to be converted.</param>
        /// <returns>An uppercase hexadecimal string representing the contents of the array.</returns>
        public static string BytesToHexString(byte[] array)
        {
            char[] hex = new char[array.Length * 2];
            byte b;
            int r;
            for (int i = 0; i < array.Length; i++)
            {
                r = 2 * i;
                b = (byte)(array[i] >> 4);
                hex[r] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = (byte)(array[i] & 0xF);
                hex[r + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            
            return new string(hex);
        }
        
        /// <summary>
        /// A helper method for converting an array of bytes to a hexadecimal string.
        /// </summary>
        /// <param name="array">The bytes to be converted.</param>
        /// <param name="offset">The starting index at which to begin the conversion.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <returns>An uppercase hexadecimal string representing the contents of the array.</returns>
        public static string BytesToHexString(byte[] array, int offset, int length)
        {
            char[] hex = new char[length * 2];
            byte b;
            int r;
            for (int i = offset; i < offset + length; i++)
            {
                r = 2 * i;
                b = (byte)(array[i] >> 4);
                hex[r] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = (byte)(array[i] & 0xF);
                hex[r + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            
            return new string(hex);
        }
        
        /// <summary>
        /// A helper method for converting a hexadecimal string to an array of bytes.
        /// </summary>
        /// <param name="hex">The uppercase string to be converted.</param>
        /// <returns>An array of bytes representing the contents of the string.</returns>
        public static byte[] HexStringToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            int r, hi, lo;
            for (int i = 0; i < bytes.Length; i++)
            {
                r = 2 * i;
                hi = (hex[r] > 0x40 ? hex[r] - 0x37 : hex[r] - 0x30) << 4;
                lo = hex[r + 1] > 0x40 ? hex[r + 1] - 0x37 : hex[r + 1] - 0x30;
                bytes[i] = (byte)(hi + lo);
            }
            
            return bytes;
        }
    }
}
