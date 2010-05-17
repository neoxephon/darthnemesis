//-----------------------------------------------------------------------
// <copyright file="NitroFont.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-01-01</date>
// <summary>The standard Nintendo DS font format.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    
    /// <summary>
    /// The standard Nintendo DS font format.
    /// </summary>
    public class NitroFont
    {
        #region Variables
        private string fileName;
        private NitroHeader header;
        private FontInfoSection finfChunk;
        private CharacterWidthChunk cwdhChunk;
        private CharacterGlyphChunk cglpChunk;
        private List<CharacterMapChunk> cmapChunks;
        private List<CharacterSet> characterSets;
        private List<int> codes = new List<int>();
        private List<Character> characters = new List<Character>();
        private Dictionary<int, Character> codeToChar = new Dictionary<int, Character>();
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the NitroFont class.
        /// </summary>
        /// <param name="fileName">The name of the file associated with the font.</param>
        public NitroFont(string fileName)
        {
            this.fileName = fileName;
        }
        
        #region Properties
        
        /// <summary>
        /// Gets or sets the name of the file associated with the font.
        /// </summary>
        /// <value>The name of the file that the font will be read from or saved to.</value>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            
            set
            {
                this.fileName = value;
            }
        }
        
        /// <summary>
        /// Gets a list of all codes used by characters in the font, in ascending order.
        /// </summary>
        /// <value>A sorted list of all character codes in the font.</value>
        public IList<int> Codes
        {
            get
            {
                return this.codes;
            }
        }
        
        /// <summary>
        /// Gets a list of all characters in the font.
        /// </summary>
        /// <value>A list of all characters in the font.</value>
        public IList<Character> Characters
        {
            get
            {
                return this.characters;
            }
        }
        
        /// <summary>
        /// Gets the maximum width of a character in the font.
        /// </summary>
        /// <value>The maximum character width.</value>
        public int Width
        {
            get
            {
                return this.characters[0].Glyph.Width;
            }
        }
        
        /// <summary>
        /// Gets the maximum height of a character in the font.
        /// </summary>
        /// <value>The maximum character height.</value>
        public int Height
        {
            get
            {
                return this.characters[0].Glyph.Height;
            }
        }
        #endregion
        
        /// <summary>
        /// Reads the font from its associated file.
        /// </summary>
        public void Load()
        {
            byte[] fileData = StreamHelper.ReadFile(this.fileName);
            
            this.header = new NitroHeader("RTFN");
            this.header.ReadFrom(fileData, 0);
            
            this.finfChunk = new FontInfoSection();
            int finfOffset = this.header.Size;
            this.finfChunk.ReadFrom(fileData, finfOffset);
            
            this.cwdhChunk = new CharacterWidthChunk();
            this.cwdhChunk.ReadFrom(fileData, this.finfChunk.WidthSectionOffset);
            
            int characterCount = this.cwdhChunk.Widths.Count;
            this.cglpChunk = new CharacterGlyphChunk(characterCount);
            this.cglpChunk.ReadFrom(fileData, this.finfChunk.GlyphSectionOffset);
            
            int cmapCount = this.header.SectionCount - 3;
            int cmapOffset = this.finfChunk.MapSectionOffset;
            this.cmapChunks = new List<CharacterMapChunk>();
            this.characterSets = new List<CharacterSet>();
            for (int i = 0; i < cmapCount; i++)
            {
                CharacterMapChunk cmapChunk = new CharacterMapChunk();
                cmapChunk.ReadFrom(fileData, cmapOffset);
                cmapOffset = cmapChunk.NextChunkOffset;
                this.cmapChunks.Add(cmapChunk);
                this.characterSets.Add(new CharacterSet(cmapChunk, this.cwdhChunk, this.cglpChunk));
            }
            
            this.RebuildIndices();
        }
        
        /// <summary>
        /// Writes the font to its associated file.
        /// </summary>
        public void Save()
        {
            this.cglpChunk.ReadFrom(this);
            this.cwdhChunk.ReadFrom(this);
            
            this.finfChunk.GlyphSectionOffset = this.header.Size + this.finfChunk.Size;
            this.finfChunk.WidthSectionOffset = this.finfChunk.GlyphSectionOffset + this.cglpChunk.Size;
            this.finfChunk.MapSectionOffset = this.finfChunk.WidthSectionOffset + this.cwdhChunk.Size;
            
            this.cmapChunks.Clear();
            int cmapOffset = this.finfChunk.MapSectionOffset;
            foreach (CharacterSet characterSet in this.characterSets)
            {
                CharacterMapChunk cmapChunk = new CharacterMapChunk();
                cmapChunk.ReadFrom(characterSet, this.codes);
                cmapOffset += cmapChunk.Size;
                cmapChunk.NextChunkOffset = cmapOffset;
                this.cmapChunks.Add(cmapChunk);
            }
            
            this.cmapChunks[this.cmapChunks.Count - 1].NextChunkOffset = 0;
            
            this.header.SectionCount = 3 + this.cmapChunks.Count;
            this.header.FileSize = cmapOffset;
            
            MemoryStream contents = new MemoryStream();
            this.header.WriteTo(contents);
            this.finfChunk.WriteTo(contents);
            this.cglpChunk.WriteTo(contents);
            this.cwdhChunk.WriteTo(contents);
            for (int i = 0; i < this.cmapChunks.Count; i++)
            {
                this.cmapChunks[i].WriteTo(contents);
            }
            
            StreamHelper.WriteFile(this.fileName, contents.ToArray());
        }
        
        /// <summary>
        /// Retrieves the character from the font with the given code, or null
        /// if no such character exists.
        /// </summary>
        /// <param name="code">The numerical encoding of the character to retrieve.</param>
        /// <returns>The character with the given code, or null if none exists.</returns>
        public Character GetCharacter(int code)
        {
            Character c;
            return this.codeToChar.TryGetValue(code, out c) ? c : null;
        }
        
        /// <summary>
        /// Resizes all glyphs in the font, preserving their existing graphics.
        /// Aligned to the bottom left corner.
        /// </summary>
        /// <param name="width">The new glyph width.</param>
        /// <param name="height">The new glyph height.</param>
        public void Resize(byte width, byte height)
        {
            foreach (Character c in this.Characters)
            {
                c.Resize(width, height);
            }
        }
        
        /// <summary>
        /// Adds the given character set to the font.
        /// The new set must not contain any characters with codes that already exist in the font.
        /// </summary>
        /// <param name="characterSet">The character set to add to the font.</param>
        public void AddCharacterSet(CharacterSet characterSet)
        {
            foreach (Character character in characterSet.Characters)
            {
                if (this.codeToChar.ContainsKey(character.Code))
                {
                    throw new ArgumentException("A character with code " + character.Code + " already exists in the font.");
                }
            }
            
            this.characterSets.Add(characterSet);
            this.RebuildIndices();
        }
        
        /// <summary>
        /// Creates a new glyph graphic of the same size as the other glyphs in the font.
        /// </summary>
        /// <returns>A new blank glyph.</returns>
        public Bitmap CreateBlankGlyph()
        {
            Bitmap glyph = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(glyph))
            {
                g.Clear(Color.Transparent);
            }
            
            return glyph;
        }
        
        /// <summary>
        /// Rebuilds the cached values used to quickly access characters.
        /// Needs to be called after modifying character indices or adding/removing characters.
        /// </summary>
        internal void RebuildIndices()
        {
            this.characters.Clear();
            this.codes.Clear();
            this.codeToChar.Clear();
            foreach (CharacterSet characterSet in this.characterSets)
            {
                this.characters.AddRange(characterSet.Characters);
                foreach (Character character in characterSet.Characters)
                {
                    this.codes.Add(character.Code);
                    this.codeToChar.Add(character.Code, character);
                }
            }
            
            this.codes.Sort();
        }
    }
}
