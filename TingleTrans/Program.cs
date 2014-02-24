//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="DarthNemesis">
// Copyright (c) 2014 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2014-02-15</date>
// <summary>Main program execution code.</summary>
//-----------------------------------------------------------------------

namespace TingleTrans
{
    using System;
    using System.Windows.Forms;
    using DarthNemesis;
    
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TranslatorForm(new Game()));
        }
    }
}
