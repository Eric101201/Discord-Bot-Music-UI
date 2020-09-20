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

namespace Discord_Bot_music_GUI
{
    public partial class Form1 : Form
    {
        public static IntPtr programIntPtr = IntPtr.Zero;
        public static Dictionary<string,ulong> input1 = new Dictionary<string, ulong>();
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
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
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
        }
        private void button_Start(object sender, EventArgs e)
        {
            Program.Start();
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
                        Program.Log2(i.Id.ToString());
                        var guild = Program._client.GetGuild(i.Id);
                        foreach (var w in guild.VoiceChannels)
                        {
                            if (w.Name == comboBox2.SelectedItem.ToString())
                            {
                                input1["channel"] = w.Id;
                                Program.Log2(w.Id.ToString());
                            }
                        }
                    }
                }
            }
        }
        private void dd(object sender, EventArgs e)
        {
            Music.Play(input1["guild"], input1["channel"]);
            Program.Log2("已連線");
        }
        private void dd2(object sender, EventArgs e)
        {
            Music.break1[input1["channel"]] = true;
            Program.Log2("已斷開連線");
        }

        private void butto4_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = System.Drawing.Color.Black;
            button8.ForeColor = System.Drawing.Color.White;
        }
    }
}
