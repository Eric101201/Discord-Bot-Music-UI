using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Discord_Bot_music_GUI
{
    public partial class Form1 : Form
    {
        public static IntPtr programIntPtr = IntPtr.Zero;
        public static Dictionary<string,ulong> input1 = new Dictionary<string, ulong>();
        public static Dictionary<ulong,ulong> guilds = new Dictionary<ulong, ulong>();
        public static void Init()
        {
            // 通过类名查找一个窗口，返回窗口句柄。
            programIntPtr = Win32.FindWindow("Progman", null);

            // 窗口句柄有效
            if (programIntPtr != IntPtr.Zero)
            {

                IntPtr result = IntPtr.Zero;

                // 向 Program Manager 窗口发送 0x52c 的一个消息，超时设置为0x3e8（1秒）。
                Win32.SendMessageTimeout(programIntPtr, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, result);

                // 遍历顶级窗口
                Win32.EnumWindows((hwnd, lParam) =>
                {
                    // 找到包含 SHELLDLL_DefView 这个窗口句柄的 WorkerW
                    if (Win32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                    {
                        // 找到当前 WorkerW 窗口的，后一个 WorkerW 窗口。
                        IntPtr tempHwnd = Win32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                        // 隐藏这个窗口
                        Win32.ShowWindow(tempHwnd, 0);
                    }
                    return true;
                }, IntPtr.Zero);
            }
        }
        private readonly String PATH = String.Format(@"test.json", Directory.GetCurrentDirectory());
        private void re(string Token,char Pr,string Ph)
        {
            Dictionary<string, string> jsontext = new Dictionary<string, string>();
            jsontext.Add("Token", Token);
            jsontext.Add("Pr", Pr.ToString());
            jsontext.Add("Ph", Ph);
            string j = JsonConvert.SerializeObject(jsontext);
            File.WriteAllText(PATH, j, Encoding.UTF8);


        }
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            try
            {
                StreamReader sr = File.OpenText(PATH);
                string jsonWordTemplate = sr.ReadToEnd();
                var dTemlate = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonWordTemplate);
                textBox5.Text = dTemlate["Token"];
                textBox3.Text = dTemlate["Pr"];
                textBox12.Text = dTemlate["Ph"];
                sr.Close();
            }
            catch (Exception)
            {
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Program.Logtext.Count>20) { Program.Logtext.RemoveAt(0); }
            
            textBox2.Text =  String.Join("", Program.Logtext);
        }
        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = System.Drawing.Color.Black;
            button1.ForeColor = System.Drawing.Color.White;
        }
        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor= System.Drawing.Color.FromArgb(0,0,0,0);
            button1.ForeColor= System.Drawing.Color.Black;
        }
        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.BackColor = System.Drawing.Color.Black;
            button2.ForeColor = System.Drawing.Color.White;
        }
        private void butto3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.BackColor = System.Drawing.Color.Black;
            button3.ForeColor = System.Drawing.Color.White;
        }
        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            button3.ForeColor = System.Drawing.Color.Black;
        }
        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            button2.ForeColor = System.Drawing.Color.Black;
        }
        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button8.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            button8.ForeColor = System.Drawing.Color.Black;
        }
        private void button_stop(object sender , EventArgs e)
        {
            Program.Stop();
            foreach(var i in Music.break1.Keys.ToList())
            {
                Music.break1[i] = true;
            }
        }
        private void button_Start(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox5.Text == "")
            {
                Program.Log2("Token or Prefix not Fill in");
            }
            else if(Program.switch1==false)
            {
                Program.Prefix = textBox3.Text.ToCharArray()[0];
                Program.Token = textBox5.Text;
                Music.ph = textBox12.Text;
                re(textBox5.Text, textBox3.Text.ToCharArray()[0], textBox12.Text);
                Program.Start();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.switch1 == true)
            {
                Program.Stop();
                Environment.Exit(0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            foreach(var i in Program._client.Guilds)
            {
                comboBox1.Items.Remove(i.Name);
                comboBox1.Items.Add(i.Name);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                foreach (var i in Program._client.Guilds)
                {
                    if (i.Name == comboBox1.SelectedItem.ToString())
                    {
                        comboBox2.Items.Clear();
                        var guild = Program._client.GetGuild(i.Id);
                        foreach (var w in guild.VoiceChannels)
                        {
                            comboBox2.Items.Add(w.Name);
                        }
                    }
                }
            }

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                foreach (var i in Program._client.Guilds)
                {
                    if (i.Name == comboBox1.SelectedItem.ToString())
                    {
                        input1["guild"] = i.Id;
                        var guild = Program._client.GetGuild(i.Id);
                        foreach (var w in guild.VoiceChannels)
                        {
                            if (w.Name == comboBox2.SelectedItem.ToString())
                            {
                                input1["channel"] = w.Id;
                                if (Music.break1.Keys.ToList().Contains(w.Id))
                                {
                                    if (Music.random[w.Id])
                                    {
                                        button7.Text = "🔀";
                                    }
                                    else
                                    {
                                        button7.Text = "🔁";
                                    };
                                    if (Music.stop[w.Id] == "false")
                                    {
                                        button5.Text = "⏸";
                                    }
                                    else if (Music.stop[w.Id] == "Suspend")
                                    {
                                        button5.Text = "▶";
                                    }
                                }
                                else
                                {
                                    button5.Text = "⏸";
                                    button7.Text = "🔁";
                                }
                            }
                        }
                    }
                }
            }
        }
        private void dd(object sender, EventArgs e)
        {
            if (guilds.Keys.ToList().Contains(input1["guild"]))
            {
                Music.break1[guilds[input1["guild"]]] = true;
                guilds.Remove(input1["guild"]);
                Music.break1.Remove(input1["channel"]);
            }
            else if (!Music.break1.Keys.ToList().Contains(input1["channel"]))
            {
                Music.Play(input1["guild"], input1["channel"]);
                Program.Log2("已連線");
                guilds.Add(input1["guild"],input1["channel"]);
            }
        }
        private void dd2(object sender, EventArgs e)
        {
            if (Music.break1.Keys.ToList().Contains(input1["channel"]))
            {
                Music.break1[input1["channel"]] = true;
                Music.break1.Remove(input1["channel"]);
                Program.Log2("已斷開連線");
            }
        }

        private void butto4_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = System.Drawing.Color.Black;
            button8.ForeColor = System.Drawing.Color.White;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Music.stop[input1["channel"]] == "false")
            {
                Music.stop[input1["channel"]] = "true";
                button5.Text = "▶";
            }else if (Music.stop[input1["channel"]] == "Suspend")
            {
                Music.stop[input1["channel"]] = "Resume";
                button5.Text = "⏸";
            }
            //暫停播放
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Music.skip[input1["channel"]] = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Music.record[input1["channel"]] = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Music.random[input1["channel"]])
            {
                Music.random[input1["channel"]] = false;
                button7.Text = "🔁";
            }
            else
            {
                Music.random[input1["channel"]] = true;
                button7.Text = "🔀";
            }
        }


        private void wwwe(object sender, EventArgs e)
        {
        }
    }
}
