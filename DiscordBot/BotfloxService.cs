using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Botflox.Bot.Data;
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
        private readonly BotfloxDatabase         _database;
        private readonly ShardChecker            _shardChecker;

        private const GuildPermission ExpectedPerms = GuildPermission.ViewChannel | GuildPermission.SendMessages |
                                                      GuildPermission.AttachFiles | GuildPermission.AddReactions;

        public BotfloxService(ILogger<BotfloxService> logger, IConfiguration configuration, DiscordShardedClient client,
            GobbieCommandHandler commandHandler, BotfloxDatabase database) {
            _logger = logger;
            _configuration = configuration;
            _discord = client;
            _commandHandler = commandHandler;
            _database = database;
            _shardChecker = new ShardChecker(_discord);
            _shardChecker.AllShardsReady += ShardCheckerOnAllShardsReadyAsync;
        }

        private async Task ShardCheckerOnAllShardsReadyAsync() {
            _logger.LogInformation($"Botflox Discord bot started with user @{_discord.CurrentUser}" +
                                   $"<{_discord.CurrentUser.Id}>");
        }

        public void Dispose() {
            _discord.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _commandHandler.InstallCommandsAsync();
            await _discord.LoginAsync(TokenType.Bot,
                _configuration["DiscordToken"] ??
                throw new InvalidOperationException("Null discord api token provided"));
            await _discord.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            async Task StopImpl() {
                await _discord.LogoutAsync();
                await _discord.StopAsync();
            }

            await Task.WhenAny(StopImpl(), Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public Task<Uri> GetInviteUriAsync() {
            return _discord.GetInviteUriAsync(ExpectedPerms);
        }
    }
}