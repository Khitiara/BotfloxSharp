using System;
using System.Threading;
using System.Threading.Tasks;
using Botflox.Bot.Utils;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Botflox.Bot
{
    public class BotfloxService : IHostedService, IDisposable
    {
        private readonly ILogger<BotfloxService> _logger;
        private readonly IConfiguration          _configuration;
        private readonly DiscordShardedClient    _discord;
        private readonly GobbieCommandHandler    _commandHandler;

        public BotfloxService(ILogger<BotfloxService> logger, IConfiguration configuration, DiscordShardedClient client,
            GobbieCommandHandler commandHandler) {
            _logger = logger;
            _configuration = configuration;
            _discord = client;
            _commandHandler = commandHandler;
        }

        public void Dispose() {
            _discord.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _discord.LoginAndWaitAsync(TokenType.Bot,
                _configuration.GetSection("Discord")?["Token"] ??
                throw new InvalidOperationException("Null discord api token provided"), cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            async Task StopImpl() {
                await _discord.LogoutAsync();
                await _discord.StopAsync();
            }

            await Task.WhenAny(StopImpl(), Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}