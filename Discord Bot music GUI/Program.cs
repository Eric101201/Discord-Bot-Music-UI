using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Discord.Audio;

namespace Discord_Bot_music_GUI
{
    static class Program
    {
        private static IServiceProvider _services;
        private static CommandService _commands;
        public static DiscordSocketClient _client;
        public static char Prefix = '!';
        public static string Token = "NzM2MDcwODQzOTY0MzI1OTQ4.XxpdZA.vEH51-aiajNrkE0S65kkWYtnjbE";
        public static List<string> Logtext = new List<string> { };
        public static bool switch1 = true;

        [STAThread]
        static async Task  Main()
        {
            var breakfastTasks = new List<Task> { Ran1(),Ran2()};
            await Task.WhenAll(breakfastTasks);
        }
        static async Task Ran1()
        {
            await Task.Run(()=> MainAsync().GetAwaiter().GetResult());
        }
        static async Task Ran2()
        {
             void w()
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
               // var foas = new Form1();
               // Form1.Init();
                //Win32.SetParent(foas.Handle, Form1.programIntPtr);
                //foas.Show();
                Application.Run(new Form1());
                
            }
            await Task.Run(() => w());
        }
        public static void Log2(string Log)
        {
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("HH:mm:ss");
            Logtext.Add(myDateString+ " WriteLine     " + Log + Environment.NewLine);
            
        }
        private static Task Log(LogMessage msg)
        {
            Logtext.Add(msg.ToString() + Environment.NewLine);
            return Task.CompletedTask;
        }
        public static async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.Log += Log;
            _services = new ServiceCollection()
    .AddSingleton(_client)
    .AddSingleton(_commands)
    .BuildServiceProvider();
            await InstallCommandsAsync();
            _client.Ready += async () =>
            {
                await _client.SetGameAsync($"{Prefix.ToString()}help");
                Log2("Bot is connected!");
            };
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            //await Task.Delay(0);
        }
        public static async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }
        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix(Prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;
            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
        public static async Task Stop()
        {
            if (switch1 != false)
            {
                switch1 = false;
                await _client.StopAsync();
            }
            else
            {
                Log2("Bot is turn off");
            }
        }
        public static async Task Start()
        {
            if (switch1 != true)
            {
                switch1 = true;
                await Ran1();
            }
            else
            {
                Log2("Bot is turn on");
            }
        }
    }
}
