using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PascalCase
{
    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }
    class Music
    {
        public static string ph;
        public static Dictionary<ulong,bool> record = new Dictionary<ulong,bool>();
        public static Dictionary<ulong,bool> random=  new  Dictionary<ulong, bool> ();
        public static Dictionary<ulong, string> stop = new Dictionary<ulong, string>();
        public static Dictionary<ulong, bool> skip = new Dictionary<ulong, bool>();
        public static Dictionary<ulong, bool> break1 = new Dictionary<ulong, bool>();
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        public static Process CreateStream(string path,int volume =1)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2  -f s16le -ar 48000 -ab 1000 -af volume={volume} pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //CreateNoWindow = true,
            });
        }
        public static async Task SendAsync(IAudioClient audioClient,Process ffmpeg)
        {
            var output = ffmpeg.StandardOutput.BaseStream;
            var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
        }
        public static async Task Play(ulong GuildId,ulong ChannelId)
        {
            var guild = Program._client.GetGuild(GuildId);
            var channel = guild.GetVoiceChannel(ChannelId);
            var audioClient = await channel.ConnectAsync();
            var Description = audioClient.ConnectionState;
            bool IsPlay = false;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var list1 = new List<int>();
            Process ffmpeg= CreateStream("",1);
            random.Add(ChannelId, false);
            stop.Add(ChannelId, "false");
            skip.Add(ChannelId, false);
            record.Add(ChannelId, false);
            break1.Add(ChannelId, false);
            int w = 0;
            while (true)
            {
                try
                {
                    if (break1[ChannelId])
                    {
                        ffmpeg.CloseMainWindow();
                        token.ThrowIfCancellationRequested();
                        await channel.DisconnectAsync();
                        break1.Remove(ChannelId);
                        random.Remove(ChannelId);
                        skip.Remove(ChannelId);
                        record.Remove(ChannelId);
                        stop.Remove(ChannelId);
                        IsPlay = false;
                        break;
                    };
                    DirectoryInfo di = new DirectoryInfo(ph);
                    var files = di.GetFiles();
                    if (Description.ToString() != "Disconnected" || Description.ToString() != "Disconnecting")
                    {
                        if (record[ChannelId])
                        {
                            ffmpeg.CloseMainWindow();
                            token.ThrowIfCancellationRequested();
                            stop[ChannelId] = "false";
                        };
                        if (IsPlay && skip[ChannelId])
                        {
                            ffmpeg.CloseMainWindow();
                            token.ThrowIfCancellationRequested();
                            skip[ChannelId] = false;
                            stop[ChannelId] = "false";
                        };
                        if (IsPlay && stop[ChannelId] == "Resume")
                        {
                            foreach (ProcessThread thread in ffmpeg.Threads)
                            {
                                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                                if (pOpenThread == IntPtr.Zero)
                                {
                                    break;
                                }
                                ResumeThread(pOpenThread);
                            }
                            stop[ChannelId] = "false";
                        };
                        if (IsPlay == false && stop[ChannelId] != "Suspend")
                        {
                            if (record[ChannelId])
                            {
                                w = list1[list1.Count - 2];
                                list1.Remove(list1.Count - 1);
                                record[ChannelId] = false;
                            }
                            else
                            {
                                if (random[ChannelId])
                                {
                                    Random crandom = new Random();
                                    w = crandom.Next(0, files.Length);
                                }
                                else
                                {
                                    if (w >= files.Length - 1)
                                    {
                                        w = 0;
                                    }
                                };
                                list1.Add(w);
                            }
                            IsPlay = true;
                            Program.Log2(files[w].Name);
                            ffmpeg = CreateStream( ph+ "\\" + files[w].Name, 1);
                            var taskA = Task.Run(() => SendAsync(audioClient, ffmpeg), token);
                            taskA.ContinueWith(r => { IsPlay = false; w += 1; });
                        };
                        if (IsPlay && stop[ChannelId] == "true")
                        {
                            foreach (ProcessThread thread in ffmpeg.Threads)
                            {
                                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                                if (pOpenThread == IntPtr.Zero)
                                {
                                    break;
                                }
                                SuspendThread(pOpenThread);
                            }
                            stop[ChannelId] = "Suspend";
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (Exception)
                {
                    ffmpeg.CloseMainWindow();
                    token.ThrowIfCancellationRequested();
                    await channel.DisconnectAsync();
                    random.Remove(ChannelId);
                    skip.Remove(ChannelId);
                    record.Remove(ChannelId);
                    stop.Remove(ChannelId);
                    IsPlay = false;
                    break;
                }
            }
        }
    }
}
