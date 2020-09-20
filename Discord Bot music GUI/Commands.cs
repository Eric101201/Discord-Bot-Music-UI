using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Discord.Audio;
using System.Diagnostics;

namespace Discord_Bot_music_GUI
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("say")]
        [Summary("say")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        // ReplyAsync is a method on ModuleBase 
        [Command("ping")]
        [Summary("ping")]
        private async Task Ping()
        {
            await ReplyAsync("Pong! 🏓 **" + Program._client.Latency + "ms**");
        }
        [Command("cr")]
        private async Task Cr(IVoiceChannel channel=null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            Music.random[channel.Id]=true;
            Music.stop[channel.Id] = "true";
            var guild = Program._client.GetGuild(600363644991176822);
            var channel1 = guild.GetTextChannel(607499417259474965);
            await channel1.SendMessageAsync("cr!rank");
        }
        [Command("ant")]
        [Summary("ant [文字]")]
        private async Task ant([Remainder][Summary("The text to echo")] string text)
        {
            char[] textlist = text.ToCharArray();
            string t = "҉";
            string sendtext = "";
            foreach(char v in textlist)
            {
                if(v!=' ')
                {
                    sendtext += t + v.ToString() + t;
                }
                else
                {
                    sendtext += " ";
                }
            }
            try
            {
                await Context.Channel.SendMessageAsync(sendtext);
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("似乎超過2000字");
            }
        }
        [Command("random")]
        private async Task random(int to , int end, IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            Music.stop[channel.Id] = "Resume";
            Random crandom = new Random();
            int w = crandom.Next(to, end);
            await Context.Channel.SendMessageAsync(w.ToString());
        }
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
            await Music.Play(Context.Guild.Id, channel.Id);
        }
        [Command("w", RunMode = RunMode.Async)]
        private async Task w(string url = "",IVoiceChannel channel = null)
        {
            var w =Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = $"-g \"{url}\" --verbose -f bestaudio/best",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //CreateNoWindow = true, youtube-dl -g "https://www.youtube.com/watch?v=58dNx719j8Q" --verbose
            });
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
            var audioClient = await channel.ConnectAsync();
            var ffmpeg = Music.CreateStream(w.StandardOutput.ReadToEnd().Split("\n")[0], 1);
            await Music.SendAsync(audioClient, ffmpeg);
        }
        [Command("skip")]
        private async Task skip(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            Music.skip[channel.Id]=true;
        }
        [Command("record")]
        private async Task record(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            Music.record[channel.Id]=true;
        }
    }
}
