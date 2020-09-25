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

namespace PascalCase
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
    }
}
