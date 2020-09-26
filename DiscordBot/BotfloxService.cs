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
        private readonly IServiceProvider        _serviceProvider;
        private readonly DiscordSocketClient     _discord;

        public BotfloxService(ILogger<BotfloxService> logger, IConfiguration configuration, IServiceProvider serviceProvider) {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _discord = new DiscordSocketClient();
            _discord.Log += _logger.LogDiscord;
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