using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Mail;
using System.IO;
using System.Diagnostics;
using System.Reflection;


namespace TraceMonitor
{
    public partial class Form1 : Form
    {
        bool auto_check;
        int hc;
        int hc_kl;

        public Form1()
        {
            InitializeComponent();
            button7.Hide();
        }
        private string Timestamp()
        {
            DateTime dt = DateTime.Now;
            return Convert.ToString(dt);
        }

        private void check_trace()
        {
            listBox2.Items.Clear();
            string Host = textBox1.Text;
            bool flag = true;

            // Check if we have any results to compare
            if (listBox1.Items.Count == 0)
            {
                listBox4.Items.Insert(0, Timestamp() + " No tracert results to check");
                listBox4.Update();
                return;
            }

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            if (!File.Exists(@"Data\Tracert_" + Host + ".log"))
            {
                listBox4.Items.Insert(0, Timestamp() + " Old file Traccert Not Found"); listBox4.Update();
                listBox4.Items.Insert(0, Timestamp() + " Create file"); listBox4.Update();
                using (StreamWriter sw = File.CreateText(@"Data\Tracert_" + Host + ".log"))
                {
                    for (int i = 0; i <= listBox1.Items.Count-1; i++)
                    {
                        sw.WriteLine(listBox1.Items[i]);
                        listBox1.Update();
                    }

                }
            }
            else
            {
                listBox4.Items.Insert(0, Timestamp() + " Old file Traccert Found"); listBox4.Update();
                listBox4.Items.Insert(0, Timestamp() + " Start Read Old file Traccert"); listBox4.Update();
                listBox2.Items.Clear();
                using (StreamReader sr = File.OpenText(@"Data\Tracert_" + Host + ".log"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        listBox2.Items.Add(s);
                        listBox2.Update();
                    }
                }
                listBox4.Items.Insert(0, Timestamp() + " Read Old file Traccert Success"); listBox4.Update();
                //--------------------------------   --------------------
                if (!(listBox1.Items.Count == listBox2.Items.Count))
                {
                    flag = false;
                }
                else
                {
                    for (int j = 0; j <= listBox1.Items.Count-1; j++)
                    {
                        string buf1=Convert.ToString(listBox1.Items[j]);
                        string buf2= Convert.ToString(listBox2.Items[j]);
                        if ( buf1!=buf2 )
                        {
                            flag = false;
                        }
                    }

                }

                if (!flag)
                {
                    listBox4.Items.Insert(0, Timestamp() + " WARNING: Paths differ !!!!"); listBox4.Update();
                    if (checkBox2.Checked)
                    {
                        Send_email();
                    }

                    // Show popup notification for route change
                    string changes = Changes("Route changes detected for " + Host);
                    ShowRouteChangeNotification(Host, changes);

                    listBox4.Items.Insert(0, Timestamp() + " We make upgrade of paths."); listBox4.Update();
                    using (StreamWriter sw = File.CreateText(@"Data\Tracert_" + Host + ".log"))
                    {
                        for (int i = 0; i <= listBox1.Items.Count - 1; i++)
                        {
                            sw.WriteLine(listBox1.Items[i]);
                        }

                    }
                }
                else
                {
                    listBox4.Items.Insert(0, Timestamp() + " Paths are identical."); listBox4.Update();
                }

                //---------------------------------------------------------------------
            }
        }

        private string parse_tracer(string lst)
        {
            string Buf = lst;
            int k = 0;
            string result = "";
            int dl = Buf.Length;
            if (dl >= 2)
            {
                for (int i = dl - 1; i >= 1; i--)
                {
                    char ml = Buf[i];
                    char mk = Buf[i - 1];
                    string sl = "" + ml;
                    string sk = "" + mk;

                    if ((sl == " ") && (sk == " "))
                    {
                        if (k == 0)
                        {
                            result = Buf.Substring(i + 1);
                            // Fix encoding issues - replace corrupted text with proper message
                            if (result.Contains("Request timed out") || result.Contains("  "))
                            {
                                result = "Request timed out";
                            }
                            else if (result.Contains("Destination host unreachable") || result.Contains("  "))
                            {
                                result = "Destination host unreachable";
                            }
                            else if (result.Contains("Tracing route") || result.Contains(" "))
                            {
                                result = "Tracing route";
                            }
                        }
                        k = k + 1;
                    }
                }
            }
            return result;
        }

        // ... rest of the file would continue here
    }
}
