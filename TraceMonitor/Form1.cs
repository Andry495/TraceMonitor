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
                //-------------------------------- ������ �������� --------------------
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
            int k =0;
            string result = "";
            int dl = Buf.Length;   
            if (dl>=2)
            {
            for (int i = dl-1; i >= 1; i--)
            {
                char ml = Buf[i];
                char mk = Buf[i-1];
                string sl = "" + ml;
                string sk = "" + mk;

                if ((sl == " ") && (sk == " "))
                {

                    if (k == 0)
                    {
                        result = Buf.Substring(i+1);
                        if (result == "�ॢ�襭 ���ࢠ� �������� ��� �����.") result = "�������� �������� ��������";
                    }
                    k = k+1;
                }
            }
            }
            return result;
        }

        private void tracerr(string hosts)
        {
            listBox1.Items.Clear();
            string command = "tracert";
            string param = " -d " + hosts;
            string buffer = "";
            char ch;
            bool n, r;
            n = false;
            r = false;
            int k = 0;
            if (command.Trim().Length > 0)
            {
                try
                {
                    if (!command.EndsWith(".exe"))
                        command = command + ".exe";
                    ProcessStartInfo inf = new ProcessStartInfo(command, param);
                    inf.CreateNoWindow = true;
                    inf.UseShellExecute = false;
                    inf.RedirectStandardOutput = true;
                    inf.RedirectStandardInput = true;
                    Process prc = new Process();
                    prc.StartInfo = inf;
                    prc.Start();
                    int character;
                    while (true)
                    {
                        character = prc.StandardOutput.Read();
                        if (character == -1)
                            break;

                        ch = (char)character;
                        if (character == 13)
                        {
                            r = true;
                        }
                        else
                            if (character == 10)
                            {
                                n = true;
                            }

                        if (r && n)
                        {                   
                            n = false;
                            r = false;
                            k = k + 1;
                            if (k > 4)
                            {
                                listBox1.Items.Add(parse_tracer(buffer));
                                listBox1.Update();         
                            }
                            buffer = "";
                        }
                        if (!(character == 13) && !(character == 10))
                        {
                            buffer = buffer + ch;
                        }
                                           
                        //System.Console.Out.Write((char)character);
                    }
                }
                catch (Exception ex)
                {
                    listBox4.Items.Insert(0, "Can't run program. " + ex.Message + "\n" + command + "\n" + param);
                    //System.Console.Out.WriteLine("Can't run program. " + ex.Message + "\n" + command + "\n" + param);
                }
            }
            int kl = listBox1.Items.Count;
            if (kl >= 2)
            {
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                listBox1.Update();
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                listBox1.Update();
            }
        }

        private void check_doublecate()
        {         
            // Check if listBox1 has items
            if (listBox1.Items.Count == 0)
            {
                return;
            }

            bool flag = false;
          while (flag == false)
          {
              flag = true;
              int kl = listBox1.Items.Count;
              
              // Ensure we have at least 2 items to compare
              if (kl < 2)
              {
                  break;
              }
              
              for (int i = 0; i < kl - 1; i++)
              {
                  // Double-check bounds before accessing items
                  if (i >= listBox1.Items.Count || (i + 1) >= listBox1.Items.Count)
                  {
                      break;
                  }
                  
                  string buf1 = Convert.ToString(listBox1.Items[i]);
                  string buf2 = Convert.ToString(listBox1.Items[i + 1]);
                  if (buf1 == buf2)
                  {
                      flag = false;
                      listBox1.Items.RemoveAt(i + 1);
                      listBox1.Update();
                      listBox4.Items.Insert(0, Timestamp() + " Found duplicate host - corrected "); 
                      listBox4.Update();
                      
                      // Break after removal to restart the loop with updated count
                      break;
                  }

              }
          }
          


        }

        private void hosts_startup()
        {
            hc = 0;
            hc_kl = listBox3.Items.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                listBox4.Items.Insert(0, Timestamp() + " Tracert is already running, please wait...");
                listBox4.Update();
                return;
            }

            ClearOldLogs();
            textBox1.Enabled = false;
            button1.Enabled = false;
            button1.Text = "Running...";
            listBox1.Items.Clear();
            listBox4.Items.Insert(0, Timestamp()+" Start tracert to:" + textBox1.Text); 
            listBox4.Update();
            
            // Start background worker for tracert
            backgroundWorker1.RunWorkerAsync(textBox1.Text);
        }

        private string ChangeHost(string lst)
        {
            string Buf = lst;
            Buf=Buf.Replace("%HOST%", textBox1.Text);
            return Buf;
        }

        private string Changes(string ls)
        {
            string Buf = ls;
            string change_listing="\r\n";
            int k = 0;
            if (listBox1.Items.Count > listBox2.Items.Count)
            {
                k = listBox1.Items.Count;
            }
            else
            {
                k = listBox2.Items.Count;
            }
            string ff = "";
            for (int m = 0; m <= k - 1; m++)
            {
                if (m < listBox1.Items.Count)
                {
                    ff = ff + " " + listBox1.Items[m] + " |   ";
                }
                else
                {
                    ff = ff + " " + "[---.---.---.---]" + " |   ";
                }

                if (m < listBox2.Items.Count)
                {
                    ff = ff + " " + listBox2.Items[m] + "\r\n";
                }
                else
                {
                    ff = ff + " " + "[---.---.---.---]" + "\r\n";
                }
            }
            change_listing = change_listing + ff+"\r\n";
            Buf = Buf.Replace("%CHANGE%", change_listing);
            return Buf;
        }

        private void Send_email()
        {
            //����� SMTP-�������
            String smtpHost = textBox2.Text;
            //���� SMTP-�������
            int smtpPort = Convert.ToInt32(maskedTextBox1.Text);
            //�����
            String smtpUserName = "";
            if (textBox4.Text == "")
            {
                smtpUserName = "LOGIN";
            }
            else
            {
                smtpUserName = textBox4.Text;
            }
            //������
            String smtpUserPass = "";
            if (textBox5.Text == "")
            {
                smtpUserPass = "PASSWORD";
            }
            else
            {
                smtpUserPass = textBox5.Text;
            }             

            //�������� �����������
            SmtpClient client = new SmtpClient(smtpHost, smtpPort);
            client.Credentials = new NetworkCredential(smtpUserName, smtpUserPass);
 
            //����� ��� ���� "��"
            String msgFrom = "TraceMonitor@Server.Ru";
            if (textBox3.Text == "")
            {
                msgFrom = "TraceMonitor@Server.Ru";
            }
            else
            {
                msgFrom = textBox3.Text;
            } 

            //����� ��� ���� "����" (����� ����������)
            String msgTo = "TraceMonitor@Server.Ru";
            if (textBox6.Text == "")
            {
                msgTo = "TraceMonitor@Server.Ru";
            }
            else
            {
                msgTo = textBox6.Text;
            } 

            //���� ������
            String msgSubject = "����� �������� ��� %HOST%";
            if (textBox7.Text == "")
            {
                msgSubject = "����� �������� ��� %HOST%";
            }
            else
            {
                msgSubject = textBox7.Text;
            }             
            msgSubject=ChangeHost(msgSubject);
            //����� ������
            String msgBody = "��������\r\n\r\n ��������� ������� ��� %HOST% \r\n\r\n%CHANGE%\r\n\r\n----------\r\nTraceMonitor@serv.ru";
            if (textBox8.Text == "")
            {
                msgBody = "��������\r\n\r\n ��������� ������� ��� %HOST% \r\n\r\n%CHANGE%\r\n\r\n----------\r\nTraceMonitor@serv.ru";
            }
            else
            {
                msgBody = textBox8.Text;
            }
            msgBody = ChangeHost(msgBody);
            msgBody = Changes(msgBody);

            //�������� ��� ������
            //���� ����� ������ ��������, ��� ������� �������� ������� ���� ������ Attachment � ������ ����� � �����
            //Attachment attachData = new Attachment("D:\�������� ��������.zip");
            //�������� ���������
            //MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
            MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
            //������ � ��������� �������������� ������� ��������
            //message.Attachments.Add(attachData);
 
            try
            {
             //�������� ���������
            client.Send(message);
            }
            catch (SmtpException ex)
            {
            //� ������ ������ ��� ������� ��������� ����� �������, � ��� ��������
            listBox4.Items.Insert(0, "ERROR Email Send:" + ex.InnerException.Message.ToString()); listBox4.Update();
            //Console.WriteLine(ex.InnerException.Message.ToString());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox4.Enabled = true;
                textBox5.Enabled = true;
            }
            else
            {
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.smtpHost = textBox2.Text;
            Properties.Settings.Default.smtpPort = Convert.ToInt32(maskedTextBox1.Text);
            Properties.Settings.Default.UseAutorisation = checkBox1.Checked;
            Properties.Settings.Default.smtpUserName = textBox4.Text;
            Properties.Settings.Default.smtpUserPass = textBox5.Text;
            Properties.Settings.Default.DefaultHosts = textBox1.Text;
            Properties.Settings.Default.msgFrom = textBox3.Text;
            Properties.Settings.Default.msgTo = textBox6.Text;
            Properties.Settings.Default.msgSubject = textBox7.Text;
            Properties.Settings.Default.Message = textBox8.Text;
            Properties.Settings.Default.SendEmail = checkBox2.Checked;
            Properties.Settings.Default.StartAutoCheck = checkBox3.Checked;
            Properties.Settings.Default.ShowPopupNotifications = checkBox4.Checked;
            Properties.Settings.Default.Timer= Convert.ToInt32(textBox9.Text);
            timer2.Interval = Convert.ToInt32(textBox9.Text);
            Properties.Settings.Default.Save();
        }

        private void EnableSettings()
        {
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            textBox7.Enabled = true;
            textBox8.Enabled = true;
            maskedTextBox1.Enabled = true;
            checkBox1.Enabled = true;
            textBox9.Enabled = true;

            if (checkBox1.Checked)
            {
                textBox4.Enabled = true;
                textBox5.Enabled = true;
            }
            else
            {
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }                                
        }

        private void DisableSettings()
        {
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            maskedTextBox1.Enabled = false;
            checkBox1.Enabled = false;
            textBox9.Enabled = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                maskedTextBox1.Enabled = true;
                checkBox1.Enabled = true;

                if (checkBox1.Checked)
                {
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                }
                else
                {
                    textBox4.Enabled = false;
                    textBox5.Enabled = false;
                }
            }
            else
            {
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                maskedTextBox1.Enabled = false;
                checkBox1.Enabled = false;
            }
        }

        private void autocheck()
        {
            if (auto_check)
            {
                listBox4.Items.Insert(0, Timestamp() + " Stop auto check tracert to host"); listBox4.Update();
                button2.Text = "Start auto";
                timer2.Enabled = false;
                
                // Cancel current tracert if running
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                    listBox4.Items.Insert(0, Timestamp() + " Cancelling current tracert..."); listBox4.Update();
                }
                
                EnableSettings();
                textBox1.Enabled = true;
                button1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                button3.Enabled = true;
                auto_check = false;
            }
            else
            {
                listBox4.Items.Insert(0, Timestamp() + " Start auto check tracert to host"); listBox4.Update();
                button2.Text = "Stop auto";
                timer2.Interval = Convert.ToInt32(textBox9.Text);
                timer2.Enabled = true;
                DisableSettings();
                textBox1.Enabled = false;
                button1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                button3.Enabled = false;
                auto_check = true;
            }                        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearOldLogs();
            hosts_startup();
            autocheck();
        }

        private string parser_param(string s,int param)
        {
            string[] parameters;
            string buf;
            buf = s;
            parameters = buf.Split(';');
            return parameters[param];
        }

        private int Count_param(string s)
        {
            return 1;
        }

        private void ClearOldLogs()
        {
            if (listBox4.Items.Count > 200)
            {
                int lst = listBox4.Items.Count;
                for (int h = lst - 1; h >=200 ; h--)
                {
                    listBox4.Items.RemoveAt(h);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(@"ListHosts.cfg"))
            {
                using (StreamWriter sw = File.CreateText(@"ListHosts.cfg"))
                {
                    sw.WriteLine("www.yandex.ru");
                }
            }
            listBox3.Items.Clear();
            using (StreamReader sr = File.OpenText(@"ListHosts.cfg"))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    listBox3.Items.Add(s);
                }
            }

            checkBox2.Checked = Properties.Settings.Default.SendEmail;
            textBox1.Text = Properties.Settings.Default.DefaultHosts;
            textBox2.Text = Properties.Settings.Default.smtpHost;
            maskedTextBox1.Text = Convert.ToString(Properties.Settings.Default.smtpPort);
            checkBox1.Checked = Properties.Settings.Default.UseAutorisation;
            textBox4.Text = Properties.Settings.Default.smtpUserName;
            textBox5.Text = Properties.Settings.Default.smtpUserPass;
            textBox3.Text = Properties.Settings.Default.msgFrom;
            textBox6.Text = Properties.Settings.Default.msgTo;
            textBox7.Text = Properties.Settings.Default.msgSubject;
            textBox8.Text = Properties.Settings.Default.Message;
            checkBox3.Checked = Properties.Settings.Default.StartAutoCheck;
            checkBox4.Checked = Properties.Settings.Default.ShowPopupNotifications;
            textBox9.Text = Convert.ToString(Properties.Settings.Default.Timer);
            timer2.Interval = Convert.ToInt32(textBox9.Text);

            if (checkBox2.Checked)
            {
                EnableSettings();
            }
            else
            {
                DisableSettings();
            }

            if (checkBox3.Checked)
            {
                auto_check = false;
                hosts_startup();
                autocheck();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:"+linkLabel1.Text);
        }

        private void edit_open()
        {
            listBox3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();

            button7.Show();
        }

        private void edit_save()
        {
            listBox3.Show();
            button4.Show();
            button5.Show();
            button6.Show();

            button7.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddNewHost();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
                Show();
                WindowState = FormWindowState.Normal;
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex >= 0)
            {
                string currentHost = parser_param(Convert.ToString(listBox3.Items[listBox3.SelectedIndex]), 0);
                string newHost = Microsoft.VisualBasic.Interaction.InputBox(
                    "Edit host name or IP address:", 
                    "Edit Host", 
                    currentHost);
                
                if (!string.IsNullOrEmpty(newHost) && newHost.Trim().Length > 0 && newHost != currentHost)
                {
                    // Check if new host already exists
                    bool hostExists = false;
                    for (int i = 0; i < listBox3.Items.Count; i++)
                    {
                        if (i != listBox3.SelectedIndex)
                        {
                            string existingHost = parser_param(Convert.ToString(listBox3.Items[i]), 0);
                            if (existingHost.Equals(newHost, StringComparison.OrdinalIgnoreCase))
                            {
                                hostExists = true;
                                break;
                            }
                        }
                    }
                    
                    if (!hostExists)
                    {
                        string hostEntry = newHost + ";Yes;" + newHost + ".state.cfg";
                        listBox3.Items[listBox3.SelectedIndex] = hostEntry;
                        SaveHostsList();
                        listBox4.Items.Insert(0, Timestamp() + " Host edited: " + currentHost + " -> " + newHost);
                        listBox4.Update();
                    }
                    else
                    {
                        listBox4.Items.Insert(0, Timestamp() + " Host already exists: " + newHost);
                        listBox4.Update();
                    }
                }
            }
            else
            {
                listBox4.Items.Insert(0, Timestamp() + " Please select a host to edit");
                listBox4.Update();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            edit_save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex >= 0)
            {
                listBox3.Items.RemoveAt(listBox3.SelectedIndex);
                SaveHostsList();
                listBox4.Items.Insert(0, Timestamp() + " Host removed from list"); 
                listBox4.Update();
            }
            else
            {
                listBox4.Items.Insert(0, Timestamp() + " Please select a host to remove"); 
                listBox4.Update();
            }
        }

        private void SaveHostsList()
        {
            try
            {
                using (StreamWriter sw = File.CreateText(@"ListHosts.cfg"))
                {
                    for (int i = 0; i < listBox3.Items.Count; i++)
                    {
                        sw.WriteLine(listBox3.Items[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                listBox4.Items.Insert(0, Timestamp() + " Error saving hosts list: " + ex.Message);
                listBox4.Update();
            }
        }

        private void AddNewHost()
        {
            string newHost = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter host name or IP address:", 
                "Add New Host", 
                "www.example.com");
            
            if (!string.IsNullOrEmpty(newHost) && newHost.Trim().Length > 0)
            {
                // Check if host already exists
                bool hostExists = false;
                for (int i = 0; i < listBox3.Items.Count; i++)
                {
                    string existingHost = parser_param(Convert.ToString(listBox3.Items[i]), 0);
                    if (existingHost.Equals(newHost, StringComparison.OrdinalIgnoreCase))
                    {
                        hostExists = true;
                        break;
                    }
                }
                
                if (!hostExists)
                {
                    string hostEntry = newHost + ";Yes;" + newHost + ".state.cfg";
                    listBox3.Items.Add(hostEntry);
                    SaveHostsList();
                    listBox4.Items.Insert(0, Timestamp() + " Host added: " + newHost);
                    listBox4.Update();
                }
                else
                {
                    listBox4.Items.Insert(0, Timestamp() + " Host already exists: " + newHost);
                    listBox4.Update();
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                // Skip this tick if tracert is still running
                return;
            }

            ClearOldLogs();
            timer2.Enabled = false;
            textBox1.Text = parser_param(Convert.ToString(listBox3.Items[hc]), 0);
            listBox1.Items.Clear();
            listBox4.Items.Insert(0, Timestamp() + " Start auto tracert to:" + textBox1.Text); 
            listBox4.Update();
            
            // Start background worker for auto tracert
            backgroundWorker1.RunWorkerAsync(textBox1.Text);
            
            hc++;
            if (hc > hc_kl - 1)
            {
                hc = 0;
            }
        }

        // BackgroundWorker event handlers for async tracert
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string host = (string)e.Argument;
            List<string> routeResults = new List<string>();
            
            try
            {
                string command = "tracert";
                string param = " -d " + host;
                string buffer = "";
                char ch;
                bool n, r;
                n = false;
                r = false;
                int k = 0;
                
                if (command.Trim().Length > 0)
                {
                    if (!command.EndsWith(".exe"))
                        command = command + ".exe";
                        
                    ProcessStartInfo inf = new ProcessStartInfo(command, param);
                    inf.CreateNoWindow = true;
                    inf.UseShellExecute = false;
                    inf.RedirectStandardOutput = true;
                    inf.RedirectStandardInput = true;
                    
                    Process prc = new Process();
                    prc.StartInfo = inf;
                    prc.Start();
                    
                    int character;
                    while (true)
                    {
                        // Check for cancellation
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            prc.Kill();
                            return;
                        }
                        
                        character = prc.StandardOutput.Read();
                        if (character == -1)
                            break;

                        ch = (char)character;
                        if (character == 13)
                        {
                            r = true;
                        }
                        else if (character == 10)
                        {
                            n = true;
                        }

                        if (r && n)
                        {                   
                            n = false;
                            r = false;
                            k = k + 1;
                            if (k > 4)
                            {
                                string result = parse_tracer(buffer);
                                if (!string.IsNullOrEmpty(result))
                                {
                                    routeResults.Add(result);
                                    // Report progress
                                    backgroundWorker1.ReportProgress(routeResults.Count, result);
                                }
                            }
                            buffer = "";
                        }
                        if (!(character == 13) && !(character == 10))
                        {
                            buffer = buffer + ch;
                        }
                    }
                    
                    prc.WaitForExit();
                }
                
                // Remove last 2 items (usually empty or summary)
                if (routeResults.Count >= 2)
                {
                    routeResults.RemoveAt(routeResults.Count - 1);
                    routeResults.RemoveAt(routeResults.Count - 1);
                }
                
                e.Result = routeResults;
            }
            catch (Exception ex)
            {
                e.Result = new List<string>();
                backgroundWorker1.ReportProgress(-1, "Error: " + ex.Message);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                // Error message
                listBox4.Items.Insert(0, Timestamp() + " " + e.UserState.ToString());
                listBox4.Update();
            }
            else
            {
                // Add route hop to list
                listBox1.Items.Add(e.UserState.ToString());
                listBox1.Update();
                
                // Update progress in log
                listBox4.Items.Insert(0, Timestamp() + " Hop " + e.ProgressPercentage + ": " + e.UserState.ToString());
                listBox4.Update();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Restore UI state
            textBox1.Enabled = true;
            button1.Enabled = true;
            button1.Text = "Start manual.";
            
            if (e.Cancelled)
            {
                listBox4.Items.Insert(0, Timestamp() + " Tracert cancelled by user");
                listBox4.Update();
            }
            else if (e.Error != null)
            {
                listBox4.Items.Insert(0, Timestamp() + " Tracert error: " + e.Error.Message);
                listBox4.Update();
            }
            else
            {
                List<string> results = (List<string>)e.Result;
                listBox4.Items.Insert(0, Timestamp() + " End tracert to:" + textBox1.Text + " (" + results.Count + " hops)");
                listBox4.Update();
                
                // Check for duplicates only if we have results
                if (listBox1.Items.Count > 0)
                {
                    check_doublecate();
                }
                
                // Start check trace
                listBox4.Items.Insert(0, Timestamp() + " Start check tracert to:" + textBox1.Text);
                listBox4.Update();
                
                // Run check_trace in UI thread (it's fast)
                check_trace();
                
                listBox4.Items.Insert(0, Timestamp() + " End check tracert to:" + textBox1.Text);
                listBox4.Update();
            }
            
            // Restart timer if auto mode is enabled
            if (auto_check)
            {
                timer2.Enabled = true;
            }
        }

        private void ShowRouteChangeNotification(string host, string changes)
        {
            if (Properties.Settings.Default.ShowPopupNotifications)
            {
                string title = "Route Change Detected";
                string message = string.Format("Route changed for {0}\n\n{1}", host, changes);
                
                notifyIcon1.BalloonTipTitle = title;
                notifyIcon1.BalloonTipText = message;
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                notifyIcon1.ShowBalloonTip(5000); // Show for 5 seconds
                
                listBox4.Items.Insert(0, Timestamp() + " Popup notification shown for route change");
                listBox4.Update();
            }
        }
    }
}