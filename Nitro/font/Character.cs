//-----------------------------------------------------------------------
// <copyright file="Character.cs" company="DarthNemesis">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-01-01</date>
// <summary>A single character in a font.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    using System.Drawing;
    
    /// <summary>
    /// A single character in a font.
    /// </summary>
    public class Character : IComparable
    {
        #region Variables
        private int code;
        private Bitmap glyph;
        private byte width;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Valid")]
        private byte xOffset;
        private byte nextOffset;
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the Character class.
        /// </summary>
        /// <param name="code">The numerical encoding index assigned to the character.</param>
        /// <param name="glyph">The character graphic.</param>
        /// <param name="xOffset">The amount of whitespace to add to the left of the glyph when drawing the character.</param>
        /// <param name="width">The number of columns in the glyph that actually comprise the character image.</param>
        /// <param name="nextOffset">The overall space taken up by the character, encompassing both its width and offset.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Valid")]
        public Character(int code, Bitmap glyph, byte xOffset, byte width, byte nextOffset)
        {
            this.code = code;
            this.glyph = (Bitmap)glyph.Clone();
            this.width = width;
            this.xOffset = xOffset;
            this.nextOffset = nextOffset;
        }
        
        #region Properties
        
        /// <summary>
        /// Gets the encoded textual representation of the character.
        /// </summary>
        /// <value>The character encoding.</value>
        public int Code
        {
            get
            {
                return this.code;
            }
        }
        
        /// <summary>
        /// Gets the graphical representation of the character.
        /// </summary>
        /// <value>The character graphic.</value>
        public Bitmap Glyph
        {
            get
            {
                return this.glyph;
            }
        }
        
        /// <summary>
        /// Gets the number of blank pixels to leave before drawing the glyph.
        /// </summary>
        /// <value>The horizontal offset to apply to the glyph when drawing the character.</value>
        public byte XOffset
        {
            get
            {
                return this.xOffset;
            }
        }
        
        /// <summary>
        /// Gets the actual width of the glyph graphic.
        /// </summary>
        /// <value>The width of the graphic itself.</value>
        public byte Width
        {
            get
            {
                return this.width;
            }
        }
        
        /// <summary>
        /// Gets the height of the glyph graphic.
        /// </summary>
        /// <value>The height of the graphic itself.</value>
        public int Height
        {
            get
            {
                return this.glyph.Height;
            }
        }
        
        /// <summary>
        /// Gets the offset at which to draw the next character.
        /// Generally represents the "total width" of the character, encompassing offset + width.
        /// </summary>
        /// <value>The total width of the character.</value>
        public byte NextOffset
        {
            get
            {
                return this.nextOffset;
            }
        }
        #endregion
        
        /// <summary>
        /// Determines whether the characters are equivalent.
        /// </summary>
        /// <param name="char1">The left-hand operand.</param>
        /// <param name="char2">The right-hand operand.</param>
        /// <returns>True if the characters are equal, false otherwise.</returns>
        public static bool operator ==(Character char1, Character char2)
        {
            if ((object)char1 == null)
            {
                return (object)char2 == null;
            }
            
            return char1.Equals(char2);
        }
        
        /// <summary>
        /// Determines whether the characters are different.
        /// </summary>
        /// <param name="char1">The left-hand operand.</param>
        /// <param name="char2">The right-hand operand.</param>
        /// <returns>True if the characters are unequal, false otherwise.</returns>
        public static bool operator !=(Character char1, Character char2)
        {
            return !(char1 == char2);
        }
        
        /// <summary>
        /// Determines whether the left-hand operand is lower than than the right-hand operand.
        /// </summary>
        /// <param name="char1">The left-hand operand.</param>
        /// <param name="char2">The right-hand operand.</param>
        /// <returns>True if the left-hand operand is lower than the right-hand operand, false otherwise.</returns>
        public static bool operator <(Character char1, Character char2)
        {
            if (char1 == null)
            {
                return true;
            }
            
            return char1.CompareTo(char2) < 0;
        }
        
        /// <summary>
        /// Determines whether the left-hand operand is higher than the right-hand operand.
        /// </summary>
        /// <param name="char1">The left-hand operand.</param>
        /// <param name="char2">The right-hand operand.</param>
        /// <returns>True if the left-hand operand is higher than the right-hand operand, false otherwise.</returns>
        public static bool operator >(Character char1, Character char2)
        {
            if (char1 == null)
            {
                return false;
            }
            
            return char1.CompareTo(char2) > 0;
        }
        
        /// <summary>
        /// Resizes the glyph, preserving the existing graphic.
        /// Aligned to the bottom left corner.
        /// </summary>
        /// <param name="width">The new glyph width.</param>
        /// <param name="height">The new glyph height.</param>
        public void Resize(byte width, byte height)
        {
            int y = height - this.glyph.Height;
            Bitmap b = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.Transparent);
                g.DrawImageUnscaled(this.glyph, 0, y);
            }
            
            this.glyph.Dispose();
            this.glyph = b;
        }
        
        /// <summary>
        /// Compares the codes of the two characters.
        /// </summary>
        /// <param name="obj">The character to compare to this.</param>
        /// <returns>Positive if this characters's code is greater than that characters's, negative if it is lower, or zero if they are equal.</returns>
        public int CompareTo(object obj)
        {
            Character that = (Character) obj;
            return this.code.CompareTo(that.code);
        }
        
        /// <summary>
        /// Determines whether the two characters are equal.
        /// </summary>
        /// <param name="obj">The character to compare to this.</param>
        /// <returns>True if the characters are identical, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Character))
            {
                return false;
            }
            
            return this.CompareTo(obj) == 0;
        }
        
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int result = this.code.GetHashCode();
            const int PRIME = 397;
            result = (result * PRIME) ^ this.glyph.GetHashCode();
            result = (result * PRIME) ^ this.width.GetHashCode();
            result = (result * PRIME) ^ this.xOffset.GetHashCode();
            result = (result * PRIME) ^ this.nextOffset.GetHashCode();
            return result;
        }
    }
}
