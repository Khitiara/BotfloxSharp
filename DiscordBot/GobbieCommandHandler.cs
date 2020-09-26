using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Botflox.Bot
{
    public class GobbieCommandHandler
    {
        private          CommandService      _commands;
        private readonly IServiceProvider    _serviceProvider;
        private          DiscordSocketClient _client;

        public GobbieCommandHandler(DiscordSocketClient client, CommandService commands,
            IServiceProvider serviceProvider) {
            _commands = commands;
            _serviceProvider = serviceProvider;
            _client = client;
        }

        public async Task InstallCommandsAsync() {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg) {
            throw new NotImplementedException();
        }
    }
}