//-----------------------------------------------------------------------
// <copyright file="CustomEncoding.cs" company="DarthNemesis">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2014-02-15</date>
// <summary>A game-specific custom character set.</summary>
//-----------------------------------------------------------------------

namespace TingleTrans
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    
    /// <summary>
    /// A custom Encoding for the character mapping used by Tingle's Balloon Trip.
    /// </summary>
    public class CustomEncoding : Encoding
    {
        /// <summary>
        /// A mapping of game character codes to Unicode characters. Used to decode game characters into readable text files.
        /// </summary>
        private Dictionary<ushort, char> codeToChar = new Dictionary<ushort, char>();
        
        /// <summary>
        /// A mapping of Unicode characters to game character codes. Used to encode replacement text lines into game format.
        /// </summary>
        private Dictionary<char, ushort> charToCode = new Dictionary<char, ushort>();
        
        /// <summary>
        /// Initializes a new instance of the CustomEncoding class.
        /// </summary>
        public CustomEncoding() : base()
        {
            // UTF-16 punctuation, symbols, and latin alphabet
            // Skip 0x3C and 0x3E (< and >) since they're's used as special characters in the text files.
            // They will be represented by <lt> and <gt> instead.
            this.GenerateUTF16Range(0x0000, 0x0020, 28);
            this.GenerateUTF16Range(0x001D, 0x003D, 1);
            this.GenerateUTF16Range(0x001F, 0x003F, 64);
            
            // Shift_JIS punctuation and symbols
            this.GenerateShiftJISRange(0x00E0, 0x8140, 63);
            this.GenerateShiftJISRange(0x011F, 0x8180, 45);
            
            // Shift_JIS hiragana
            this.GenerateShiftJISRange(0x0151, 0x829F, 83);
            
            // Shift_JIS katakana
            this.GenerateShiftJISRange(0x01B1, 0x8340, 63);
            this.GenerateShiftJISRange(0x01F0, 0x8380, 23);
        }
        
        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <returns>The maximum number of characters produced by decoding the specified number of bytes.</returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * 9;
        }
        
        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <returns>The maximum number of bytes produced by encoding the specified number of characters.</returns>
        public override int GetMaxByteCount(int charCount)
        {
            return 2 * (charCount + 1);
        }
        
        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="byteIndex">The index of the first byte to decode.</param>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
        /// <returns>The actual number of characters written into chars.</returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int originalIndex = charIndex;
            int charCount = 0;
            string stringToWrite;
            for (int i = byteIndex; i < byteIndex + byteCount; i += 2)
            {
                ushort code = BitConverter.ToUInt16(bytes, i);
                if (this.codeToChar.ContainsKey(code))
                {
                    chars[charIndex++] = this.codeToChar[code];
                    charCount++;
                    continue;
                }
                else if (code == 0xFFFF)
                {
                    continue;
                }
                else if (code >= 0xF000 && code <= 0xF00F)
                {
                    stringToWrite = "<" + "0123456789ABCDEF"[code & 0xF] + ">";
                }
                else if (code >= 0xF100 && code <= 0xF10F)
                {
                    i += 2;
                    string val = BitConverter.ToUInt16(bytes, i).ToString(CultureInfo.InvariantCulture);
                    stringToWrite = "<" + "0123456789ABCDEF"[code & 0xF] + ":" + val + ">";
                }
                else if (code == 0x001C)
                {
                    stringToWrite = "<lt>";
                }
                else if (code == 0x001E)
                {
                    stringToWrite = "<gt>";
                }
                else if (code == 0x0251 || code == 0x02D4 || code == 0x03BE || code == 0x0500)
                {
                    string val = string.Format(CultureInfo.InvariantCulture, "{0:X4}", code);
                    stringToWrite = "<h" + val + ">";
                }
                else
                {
                    throw new InvalidDataException("Unsupported character [" + code + "]");
                }
                
                charIndex += WriteString(chars, charIndex, stringToWrite);
            }
            
            return charIndex - originalIndex;
        }
        
        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>The number of characters produced by decoding the specified sequence of bytes.</returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            int charCount = 0;
            for (int i = index; i < index + count; i += 2)
            {
                ushort code = BitConverter.ToUInt16(bytes, i);
                if (this.codeToChar.ContainsKey(code))
                {
                    charCount++;
                }
                else if (code == 0xFFFF)
                {
                    continue;
                }
                else if (code >= 0xF000 && code <= 0xF00F)
                {
                    charCount += 3;
                }
                else if (code >= 0xF100 && code <= 0xF10F)
                {
                    i += 2;
                    string val = bytes[i].ToString(CultureInfo.InvariantCulture);
                    charCount += 4 + val.Length;
                }
                else if (code == 0x001C || code == 0x001E)
                {
                    charCount += 4;
                }
                else if (code == 0x0251 || code == 0x02D4 || code == 0x03BE || code == 0x0500)
                {
                    charCount += 7;
                }
                else
                {
                    throw new InvalidDataException("Unsupported character [" + code + "]");
                }
            }
            
            return charCount;
        }
        
        /// <summary>
        /// Encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="charIndex">The index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
        /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
        /// <returns>The actual number of bytes written into bytes.</returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            int byteCount = 2;
            for (int i = charIndex; i < charIndex + charCount; i++)
            {
                if (chars[i] == '<')
                {
                    i++;
                    if (chars[i] == 'l' && chars[i + 1] == 't')
                    {
                        bytes[byteIndex++] = 0x1C;
                        bytes[byteIndex++] = 0x00;
                        byteCount += 2;
                        i += 2;
                    }
                    else if (chars[i] == 'g' && chars[i + 1] == 't')
                    {
                        bytes[byteIndex++] = 0x1E;
                        bytes[byteIndex++] = 0x00;
                        byteCount += 2;
                        i += 2;
                    }
                    else if (chars[i] == 'h')
                    {
                        bytes[byteIndex++] = System.Convert.ToByte(string.Empty + chars[i + 3] + chars[i + 4], 16);
                        bytes[byteIndex++] = System.Convert.ToByte(string.Empty + chars[i + 1] + chars[i + 2], 16);
                        byteCount += 2;
                        i += 5;
                    }
                    else if ("012345CDE".IndexOf(chars[i]) != -1)
                    {
                        bytes[byteIndex++] = (byte)"0123456789ABCDEF".IndexOf(chars[i]);
                        bytes[byteIndex++] = 0xF0;
                        byteCount += 2;
                        i++;
                    }
                    else if ("6789ABF".IndexOf(chars[i]) != -1)
                    {
                        bytes[byteIndex++] = (byte)"0123456789ABCDEF".IndexOf(chars[i]);
                        bytes[byteIndex++] = 0xF1;
                        string val = string.Empty;
                        i += 2;
                        while (chars[i] != '>')
                        {
                            val += chars[i++];
                        }
                        
                        ushort code = ushort.Parse(val, CultureInfo.InvariantCulture);
                        byte[] data = BitConverter.GetBytes(code);
                        Array.Copy(data, 0, bytes, byteIndex, 2);
                        byteIndex += 2;
                        byteCount += 4;
                    }
                }
                else if (this.charToCode.ContainsKey(chars[i]))
                {
                    ushort code = this.charToCode[chars[i]];
                    byte[] data = BitConverter.GetBytes(code);
                    Array.Copy(data, 0, bytes, byteIndex, 2);
                    byteIndex += 2;
                    byteCount += 2;
                }
                else
                {
                    throw new InvalidDataException("Unsupported character [" + chars[i] + "]");
                }
            }
            
            bytes[byteIndex++] = 0xFF;
            bytes[byteIndex++] = 0xFF;
            
            return byteCount;
        }
        
        /// <summary>
        /// When overridden in a derived class, calculates the number of bytes produced by encoding a set of characters from the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="index">The index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            int byteCount = 2;
            for (int i = index; i < index + count; i++)
            {
                if (chars[i] == '<')
                {
                    i++;
                    if (chars[i] == 'l' && chars[i + 1] == 't')
                    {
                        byteCount += 2;
                        i += 2;
                    }
                    else if (chars[i] == 'g' && chars[i + 1] == 't')
                    {
                        byteCount += 2;
                        i += 2;
                    }
                    else if (chars[i] == 'h')
                    {
                        byteCount += 2;
                        i += 5;
                    }
                    else if ("012345CDE".IndexOf(chars[i]) != -1)
                    {
                        byteCount += 2;
                        i++;
                    }
                    else if ("6789ABF".IndexOf(chars[i]) != -1)
                    {
                        i += 2;
                        while (chars[i] != '>')
                        {
                            i++;
                        }
                        
                        byteCount += 4;
                    }
                }
                else if (this.charToCode.ContainsKey(chars[i]))
                {
                    byteCount += 2;
                }
                else
                {
                    throw new InvalidDataException("Unsupported character [" + chars[i] + "]");
                }
            }
            
            return byteCount;
        }
        
        /// <summary>
        /// Writes a string into the character array at the given position and returns the number of characters written.
        /// </summary>
        /// <param name="chars">The character array into which the string will be written.</param>
        /// <param name="charIndex">The starting position in the array at which the characters will be written.</param>
        /// <param name="stringToWrite">The string to write into the character array.</param>
        /// <returns>The number of characters written.</returns>
        private static int WriteString(char[] chars, int charIndex, string stringToWrite)
        {
            char[] charsToWrite = stringToWrite.ToCharArray();
            charsToWrite.CopyTo(chars, charIndex);
            return charsToWrite.Length;
        }
        
        /// <summary>
        /// Adds a mapping between a character code and a Unicode character.
        /// </summary>
        /// <param name="code">The game character code.</param>
        /// <param name="character">The Unicode character.</param>
        private void AddCharacter(ushort code, char character)
        {
            this.codeToChar.Add(code, character);
            this.charToCode.Add(character, code);
        }
        
        /// <summary>
        /// Generates a range of mappings between game character codes and UTF-16 code points.
        /// </summary>
        /// <param name="codeStart">The starting game code point.</param>
        /// <param name="utfStart">The starting UTF-16 code point.</param>
        /// <param name="length">The number of character mappings to generate.</param>
        private void GenerateUTF16Range(ushort codeStart, ushort utfStart, int length)
        {
            ushort code = codeStart;
            char utfChar = (char)utfStart;
            for (int i = 0; i < length; i++)
            {
                this.AddCharacter(code, utfChar);
                code++;
                utfChar++;
            }
        }
        
        /// <summary>
        /// Generates a range of mappings between game character codes and Shift_JIS code points.
        /// </summary>
        /// <param name="codeStart">The starting game code point.</param>
        /// <param name="shiftJisStart">The starting Shift_JIS code point.</param>
        /// <param name="length">The number of character mappings to generate.</param>
        private void GenerateShiftJISRange(ushort codeStart, ushort shiftJisStart, int length)
        {
            byte[] jisCode = new byte[2];
            jisCode[0] = (byte)(shiftJisStart >> 8);
            jisCode[1] = (byte)(shiftJisStart & 0xFF);
            byte[] jisBytes = new byte[length * 2];
            for (int i = 0; i < length * 2; i += 2)
            {
                jisBytes[i] = jisCode[0];
                jisBytes[i + 1] = jisCode[1];
                jisCode[1]++;
            }
            
            ushort code = codeStart;
            string chars = Encoding.GetEncoding("Shift_JIS").GetString(jisBytes);
            for (int i = 0; i < length; i++)
            {
                this.AddCharacter(code++, chars[i]);
            }
        }
    }
}
