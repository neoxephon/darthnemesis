//-----------------------------------------------------------------------
// <copyright file="CharacterMapChunk.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-15</date>
// <summary>Contains a set of character-to-code mappings.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Contains a set of character-to-code mappings.
    /// </summary>
    public class CharacterMapChunk : AbstractChunk
    {
        /* Sample formats
         * size     scod ecod type unk1 nextoff  cnt  zero
         * 18000000 9F82 F182 0000 0000 50160100 6F01 0000
         * C0100000 0000 FFFF 0200 0000 00000000 2A04 A100 5F005201
         * 94000000 A100 DF00 0100 0000 141A0100 5F00 60006100620063006400650066006700680069006A006B006C006D006E006F0070007100720073007400750076007700780079007A007B007C007D007E007F0080008100820083008400850086008700880089008A008B008C008D008E008F0090009100920093009400950096009700980099009A009B009C009D000000
         * CC000000 A200 FC00 0100 0000 60400100 6000 6100FFFFFFFFFFFFFFFFFFFF6200FFFFFFFFFFFFFFFF6300FFFF
         * C0100000 0000 FFFF 0200 0000 00000000 2A04 A1005F005201960053019700152098001C2099001D209A0022209B0026209C0033209D003B209E00AC209F002221A0009021A1009121A2009221A3009321A4001E22A5003422A600A025A700A125
         */
        
        #region Variables
        private const int Signature = 0x434D4150;
        private const int HeaderSize = 0x14;
        private const ushort Padding = 0x0000;
        private const ushort Blank = 0xFFFF;
        private ushort startCode;
        private ushort endCode;
        private CharacterMapType type;
        private ushort unknown;
        private int nextOffset;
        private Dictionary<ushort, ushort> mappings = new Dictionary<ushort, ushort>();
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the CharacterMapChunk class.
        /// </summary>
        public CharacterMapChunk()
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
                switch (this.type)
                {
                    case CharacterMapType.Range:
                        return RoundUp(HeaderSize + 2);
                    case CharacterMapType.List:
                        return RoundUp(HeaderSize + (2 * this.mappings.Count));
                    case CharacterMapType.Map:
                        return RoundUp(HeaderSize + 2 + (4 * this.mappings.Count));
                    default:
                        return -1;
                }
            }
        }
        
        /// <summary>
        /// Gets the type of this character map, which determines how the mappings are encoded.
        /// </summary>
        /// <value>The type of this map.</value>
        public CharacterMapType MapType
        {
            get
            {
                return this.type;
            }
        }
        
        /// <summary>
        /// Gets or sets the offset of the next CMAP chunk in the font file.
        /// It points 8 bytes beyond the CMAP header, so the value must be adjusted accordingly.
        /// The final CMAP chunk should have a next chunk offset of 0000.
        /// </summary>
        /// <value>The adjusted offset of the next CMAP chunk, or zero if this is the last one.</value>
        public int NextChunkOffset
        {
            get
            {
                return this.nextOffset == 0 ? 0 : this.nextOffset - 8;
            }
            
            set
            {
                this.nextOffset = value == 0 ? 0 : value + 8;
            }
        }
        
        /// <summary>
        /// Gets a dictionary representing the mappings between each
        /// character's code and its location in the font.
        /// </summary>
        /// <value>A dictionary of code-to-index mappings for this chunk.</value>
        public Dictionary<int, int> Mappings
        {
            get
            {
                Dictionary<int, int> map = new Dictionary<int, int>(this.mappings.Count);
                foreach (ushort key in this.mappings.Keys)
                {
                    int code = (int)key;
                    int index = (int)this.mappings[key];
                    map.Add(code, index);
                }
                
                return map;
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
                throw new FormatException("Chunk is not CMAP format.");
            }
            
            reader.ReadInt32(); // chunk size; ignore
            this.startCode = reader.ReadUInt16();
            this.endCode = reader.ReadUInt16();
            this.type = (CharacterMapType)reader.ReadUInt16();
            this.unknown = reader.ReadUInt16();
            this.nextOffset = reader.ReadInt32();
            
            // Read body
            this.mappings.Clear();
            switch (this.type)
            {
                    /* Type 0: Assign codes and indices in ascending order
                     * Format: BBBB (B = baseIndex)
                     * Codes = startCode ... endCode
                     * Indices = baseIndex ... (baseIndex + (endCode - startCode))
                     */
                case CharacterMapType.Range:
                    for (ushort code = this.startCode, index = reader.ReadUInt16(); code <= this.endCode; code++, index++)
                    {
                        this.mappings.Add(code, index);
                    }
                    
                    break;
                    
                    /* Type 1: Assign codes in ascending order to specified indices
                     * Format: AAAABBBBCCCCDDDD (A = index1, B = index2, C = index3, etc.
                     *                           Set an index to FFFF to exclude it)
                     * Codes = startCode ... endCode
                     * Indices = index1, index2, index3, ..., indexN
                     */
                case CharacterMapType.List:
                    for (ushort code = this.startCode; code <= this.endCode; code++)
                    {
                        ushort index = reader.ReadUInt16();
                        this.mappings.Add(code, index);
                    }
                    
                    break;
                    
                    /* Type 2: Assign specified codes to specified indices
                     * Format: NNNNAAAABBBBCCCCDDDD (N = number of mappings, A = code1,
                     *                               B = index1, C = code2, D = index2, etc.)
                     * startCode = 0000, endCode = FFFF
                     * Format
                     * Codes = code1, code2 ... endCode
                     * Indices = index1, index2, index3, ..., indexN
                     */
                case CharacterMapType.Map:
                    ushort count = reader.ReadUInt16();
                    for (int i = 0; i < count; i++)
                    {
                        ushort code = reader.ReadUInt16();
                        ushort index = reader.ReadUInt16();
                        this.mappings.Add(code, index);
                    }
                    
                    break;
            }
            
            while (reader.BaseStream.Position < RoundUp(reader.BaseStream.Position))
            {
                reader.ReadUInt16();
            }
        }
        
        /// <summary>
        /// Repopulates the mappings from the given character set.
        /// </summary>
        /// <param name="characterSet">The character set to be read from.</param>
        /// <param name="codes">A list of all numerical encodings in the font, sorted by index.</param>
        public void ReadFrom(CharacterSet characterSet, IList<int> codes)
        {
            ushort code;
            ushort index;
            this.unknown = 0;
            this.mappings.Clear();
            IList<Character> chars = characterSet.Characters;
            this.type = characterSet.MapType;
            switch (this.MapType)
            {
                    // Type 0: Codes and indices are assigned in ascending order
                case CharacterMapType.Range:
                    this.startCode = (ushort)chars[0].Code;
                    this.endCode = (ushort)chars[chars.Count - 1].Code;
                    for (int i = 0; i < chars.Count; i++)
                    {
                        code = (ushort)chars[i].Code;
                        index = (ushort)codes.IndexOf(code);
                        this.mappings.Add(code, index);
                    }
                    
                    break;
                    
                    // Type 1: Codes are assigned in ascending order to the specified indices
                case CharacterMapType.List:
                    this.startCode = (ushort)chars[0].Code;
                    this.endCode = (ushort)chars[chars.Count - 1].Code;
                    code = this.startCode;
                    for (int i = 0; i < chars.Count; i++, code++)
                    {
                        for (; chars[i].Code > code; code++)
                        {
                            this.mappings.Add(code, Blank);
                        }
                        
                        index = (ushort)codes.IndexOf(code);
                        this.mappings.Add(code, index);
                    }
                    
                    break;
                    
                    // Type 2: Codes and indices are assigned individually
                case CharacterMapType.Map:
                    this.startCode = 0x0000;
                    this.endCode = 0xFFFF;
                    for (int i = 0; i < chars.Count; i++)
                    {
                        code = (ushort)chars[i].Code;
                        index = (ushort)codes.IndexOf(code);
                        this.mappings.Add(code, index);
                    }
                    
                    break;
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
            writer.Write(Signature);
            writer.Write(this.Size);
            writer.Write(this.startCode);
            writer.Write(this.endCode);
            writer.Write((ushort)this.type);
            writer.Write(this.unknown);
            writer.Write(this.nextOffset);
            
            // Write body
            switch (this.type)
            {
                    // Type 0: Assign codes and indices in ascending order
                case CharacterMapType.Range:
                    writer.Write(this.mappings[this.startCode]);
                    break;
                    
                    // Type 1: Assign codes in ascending order to specified indices
                case CharacterMapType.List:
                    foreach (ushort index in this.mappings.Values)
                    {
                        writer.Write(index);
                    }
                    
                    break;
                    
                    // Type 2: Assign specified codes to specified indices
                case CharacterMapType.Map:
                    ushort count = (ushort)this.mappings.Count;
                    writer.Write(count);
                    foreach (ushort code in this.mappings.Keys)
                    {
                        ushort index = this.mappings[code];
                        writer.Write(code);
                        writer.Write(index);
                    }
                    
                    break;
            }
            
            // Pad to a multiple of 4
            writer.Flush();
            while (writer.BaseStream.Position < RoundUp(writer.BaseStream.Position))
            {
                writer.Write(Padding);
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
    }
}
