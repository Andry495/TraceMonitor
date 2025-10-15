using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TraceMonitor
{
    static class Program
    {
        // Windows API for DPI awareness
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable DPI awareness for .NET Framework 4.8
            try
            {
                SetProcessDPIAware();
            }
            catch
            {
                // Fallback if DPI awareness fails
            }

            // Enable modern visual styles for .NET Framework 4.8
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Set high DPI mode for better scaling
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
            }
            catch
            {
                // Fallback for older systems
            }

            Application.Run(new Form1());
        }
    }
}