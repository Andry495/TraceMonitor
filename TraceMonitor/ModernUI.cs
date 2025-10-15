using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TraceMonitor
{
    /// <summary>
    /// Modern UI styling for Windows 10/11 compatibility
    /// </summary>
    public static class ModernUI
    {
        // Windows API declarations for DPI support
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
        // Modern color scheme
        public static readonly Color PrimaryColor = Color.FromArgb(0, 120, 215); // Windows 10 blue
        public static readonly Color SecondaryColor = Color.FromArgb(240, 240, 240); // Light gray
        public static readonly Color AccentColor = Color.FromArgb(107, 107, 107); // Dark gray
        public static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255); // White
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 200); // Light border

        /// <summary>
        /// Get DPI scaling factor for the current display
        /// </summary>
        public static float GetDpiScalingFactor()
        {
            try
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                if (hdc != IntPtr.Zero)
                {
                    int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                    ReleaseDC(IntPtr.Zero, hdc);
                    return dpiX / 96.0f; // 96 DPI is standard
                }
            }
            catch
            {
                // Fallback to 1.0 if DPI detection fails
            }
            return 1.0f;
        }

        /// <summary>
        /// Apply modern styling to a form
        /// </summary>
        public static void ApplyModernFormStyle(Form form)
        {
            form.BackColor = BackgroundColor;
            
            // Apply DPI-aware font scaling using .NET Framework 4.8 features
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            form.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            
            // Enable double buffering for smoother rendering
            SetDoubleBuffered(form);
            
            // Enable modern visual styles for .NET Framework 4.8
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch
            {
                // Ignore if already set
            }
        }

        /// <summary>
        /// Apply modern styling to a button
        /// </summary>
        public static void ApplyModernButtonStyle(Button button)
        {
            button.BackColor = PrimaryColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 180);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 80, 150);
            
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            button.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Apply modern styling to a text box
        /// </summary>
        public static void ApplyModernTextBoxStyle(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = BackgroundColor;
            textBox.ForeColor = Color.Black;
            
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            textBox.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
        }

        /// <summary>
        /// Apply modern styling to a list box
        /// </summary>
        public static void ApplyModernListBoxStyle(ListBox listBox)
        {
            listBox.BackColor = BackgroundColor;
            listBox.ForeColor = Color.Black;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.SelectionMode = SelectionMode.One;
            
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            listBox.Font = new Font("Consolas", fontSize, FontStyle.Regular);
        }

        /// <summary>
        /// Apply modern styling to a tab control
        /// </summary>
        public static void ApplyModernTabControlStyle(TabControl tabControl)
        {
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            tabControl.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            tabControl.Appearance = TabAppearance.Normal;
        }

        /// <summary>
        /// Apply modern styling to a panel
        /// </summary>
        public static void ApplyModernPanelStyle(Panel panel)
        {
            panel.BackColor = BackgroundColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// Apply modern styling to a label
        /// </summary>
        public static void ApplyModernLabelStyle(Label label)
        {
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            label.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            label.ForeColor = Color.Black;
        }

        /// <summary>
        /// Apply modern styling to a checkbox
        /// </summary>
        public static void ApplyModernCheckBoxStyle(CheckBox checkBox)
        {
            // Apply DPI-aware font scaling
            float dpiScale = GetDpiScalingFactor();
            float fontSize = Math.Max(8.0f, 9.0f * dpiScale);
            checkBox.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            checkBox.ForeColor = Color.Black;
        }

        /// <summary>
        /// Enable double buffering for a control
        /// </summary>
        private static void SetDoubleBuffered(Control control)
        {
            if (SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            aProp?.SetValue(control, true, null);
        }

        /// <summary>
        /// Apply modern styling to all controls in a container
        /// </summary>
        public static void ApplyModernStyleToContainer(Control container)
        {
            foreach (Control control in container.Controls)
            {
                switch (control)
                {
                    case Button button:
                        ApplyModernButtonStyle(button);
                        break;
                    case TextBox textBox:
                        ApplyModernTextBoxStyle(textBox);
                        break;
                    case ListBox listBox:
                        ApplyModernListBoxStyle(listBox);
                        break;
                    case TabControl tabControl:
                        ApplyModernTabControlStyle(tabControl);
                        break;
                    case Panel panel:
                        ApplyModernPanelStyle(panel);
                        break;
                    case Label label:
                        ApplyModernLabelStyle(label);
                        break;
                    case CheckBox checkBox:
                        ApplyModernCheckBoxStyle(checkBox);
                        break;
                }

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyModernStyleToContainer(control);
                }
            }
        }
    }
}
