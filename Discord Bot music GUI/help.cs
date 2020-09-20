using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using System.Threading.Tasks;
namespace Discord_Bot_music_GUI
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }
        [Command("help")]
        [Summary("Echoes a message.")]
        public async Task HelpAsync(string HelpName=null)
        {
            if (HelpName != null)
            {
                foreach(var name in _service.Search(Context, HelpName).Commands)
                {
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Title = "help";
                    builder.Description = $"```md\n[必要傳入值](非必要傳入值)\n```\n指令:`{name.Command.Name}`\n權限:`{name.Command.Preconditions.Count}`\n使用方法:```fix\n{Program.Prefix}{name.Command.Summary}\n```";
                    await ReplyAsync("", false, builder.Build());
                }
            }
            else
            {
                string text = $"Prefix:{Program.Prefix}\n指令:";
                EmbedBuilder builder = new EmbedBuilder();
                foreach (var w in _service.Modules)
                {
                    foreach (var name in w.Commands)
                    {
                        text += $"`{name.Name}` ";
                    };
                };
                builder.Title = "help";
                builder.Description = text;
                await ReplyAsync("", false, builder.Build());
            }
        }
    }
}
