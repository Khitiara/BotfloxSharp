using System;
using System.Threading;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Utils;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Botflox.Bot
{
    public class BotfloxService : IHostedService, IDisposable
    {
        private readonly ILogger<BotfloxService> _logger;
        private readonly IConfiguration          _configuration;
        private readonly DiscordShardedClient    _discord;
        private          GobbieCommandHandler?   _commandHandler;
        private          BotfloxDatabase?        _database;
        private readonly IServiceProvider        _provider;
        private          IServiceScope?          _scope;
        public BotfloxService(ILogger<BotfloxService> logger, IConfiguration configuration, DiscordShardedClient client,
            IServiceProvider provider) {
            _logger = logger;
            _configuration = configuration;
            _discord = client;
            _provider = provider;
        }

        public void Dispose() {
            _scope?.Dispose();
            _discord.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            _scope = _provider.CreateScope();
            _database = _scope.ServiceProvider.GetRequiredService<BotfloxDatabase>();
            _commandHandler = _scope.ServiceProvider.GetRequiredService<GobbieCommandHandler>();
            await _commandHandler.InstallCommandsAsync();
            await _discord.LoginAndWaitAsync(TokenType.Bot,
                _configuration.GetSection("Discord")?["Token"] ??
                throw new InvalidOperationException("Null discord api token provided"), cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            async Task StopImpl() {
                _scope?.Dispose();
                await _discord.LogoutAsync();
                await _discord.StopAsync();
            }

            await Task.WhenAny(StopImpl(), Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}