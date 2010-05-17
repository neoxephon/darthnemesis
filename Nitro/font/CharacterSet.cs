//-----------------------------------------------------------------------
// <copyright file="CharacterSet.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-01-01</date>
// <summary>A collection of characters encompassing one map of a font.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// A collection of characters encompassing one map of a font.
    /// </summary>
    public class CharacterSet
    {
        #region Variables
        private const int Blank = 0xFFFF;
        private List<Character> characters;
        private CharacterMapType type;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the CharacterSet class with the given characters and map type.
        /// </summary>
        /// <param name="characters">The initial characters in the set.</param>
        /// <param name="type">The character map type, which determines how the mappings are encoded in the saved file.</param>
        public CharacterSet(IList<Character> characters, CharacterMapType type)
        {
            this.characters = new List<Character>(characters);
            this.type = type;
        }
        
        /// <summary>
        /// Initializes a new instance of the CharacterSet class with the given map type and an empty character list.
        /// </summary>
        /// <param name="type">The character map type, which determines how the mappings are encoded in the saved file.</param>
        public CharacterSet(CharacterMapType type)
        {
            this.characters = new List<Character>();
            this.type = type;
        }
        
        /// <summary>
        /// Initializes a new instance of the CharacterSet class by reading the appropriate chunks from the font.
        /// </summary>
        /// <param name="map">The map between characters and their numerical codes.</param>
        /// <param name="widths">The width and offset data for all characters in the font.</param>
        /// <param name="glyphs">The graphical glyphs for all characters in the font.</param>
        public CharacterSet(CharacterMapChunk map, CharacterWidthChunk widths, CharacterGlyphChunk glyphs)
        {
            this.Load(map, widths, glyphs);
        }
        
        #region Properties
        
        /// <summary>
        /// Gets a list of the characters contained in this set.
        /// </summary>
        /// <value>A list of the characters contained in this set.</value>
        public IList<Character> Characters
        {
            get
            {
                return this.characters;
            }
        }
        
        /// <summary>
        /// Gets or sets the type of this character set, which determines how the mappings are encoded.
        /// </summary>
        /// <value>The type of this map.</value>
        public CharacterMapType MapType
        {
            get
            {
                return this.type;
            }
            
            set
            {
                this.type = value;
            }
        }
        #endregion
        
        /// <summary>
        /// Inserts the given character into the set at the appropriate index.
        /// </summary>
        /// <param name="character">The character to add to the set.</param>
        public void Add(Character character)
        {
            this.characters.Add(character);
            this.characters.Sort();
        }
        
        private void Load(CharacterMapChunk map, CharacterWidthChunk widths, CharacterGlyphChunk glyphs)
        {
            this.characters = new List<Character>(map.Mappings.Count);
            this.type = map.MapType;
            
            foreach (int code in map.Mappings.Keys)
            {
                int index = map.Mappings[code];
                if (index != Blank)
                {
                    Character c = new Character(code, glyphs.Glyphs[index], widths.XOffsets[index], widths.Widths[index], widths.NextOffsets[index]);
                    this.characters.Add(c);
                }
            }
        }
    }
}
