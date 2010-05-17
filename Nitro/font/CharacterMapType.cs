//-----------------------------------------------------------------------
// <copyright file="CharacterMapType.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-15</date>
// <summary>An enumeration of the different character map types.</summary>
//-----------------------------------------------------------------------

namespace Nitro.Font
{
    using System;
    
    /// <summary>
    /// An enumeration of the different character map types.
    /// </summary>
    public enum CharacterMapType
    {
        /// <summary>
        /// Only the starting code and index are stored.
        /// Both mappings are assigned in ascending order from the starting values.
        /// </summary>
        Range = 0,
        
        /// <summary>
        /// A list of indices is stored.
        /// Codes are assigned in ascending order to each index in the list.
        /// An index of FFFF indicates that the code should be left unmapped.
        /// </summary>
        List = 1,
        
        /// <summary>
        /// Both characters and indices are stored.
        /// They are assigned to each other in pairs.
        /// </summary>
        Map = 2
    }
}
