using System;
using System.Reflection;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Modules;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Botflox.Bot
{
    public class GobbieCommandHandler
    {
        private readonly CommandService       _commands;
        private readonly IServiceProvider     _serviceProvider;
        private readonly BotfloxDatabase      _database;
        private readonly DiscordShardedClient _client;

        public GobbieCommandHandler(DiscordShardedClient client, CommandService commands,
            IServiceProvider serviceProvider, BotfloxDatabase database) {
            _commands = commands;
            _serviceProvider = serviceProvider;
            _database = database;
            _client = client;
        }

        public async Task InstallCommandsAsync() {
            foreach (IModuleCollection collection in _serviceProvider.GetServices<IModuleCollection>()) {
                await collection.AddModulesAsync(_commands, _serviceProvider);
            }
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage) {
            if (!(socketMessage is SocketUserMessage msg)) {
                return;
            }

            int argPos = 0;
            string prefix = "?";
            // Are we in a guild?
            if (msg.Channel is SocketTextChannel guildChan) {
                ulong guildId = guildChan.Guild.Id;
                GuildSettings? guildSettings = await _database.GuildsSettings
                    .SingleOrDefaultAsync(s => s.GuildId == guildId);
                if (guildSettings == null) {
                    guildSettings = new GuildSettings {
                        GuildId = guildId,
                        CommandPrefix = prefix
                    };
                    await _database.GuildsSettings.AddAsync(guildSettings);
                    await _database.SaveChangesAsync();
                } else prefix = guildSettings.CommandPrefix;
            }

            if (!msg.HasStringPrefix(prefix, ref argPos) && !msg.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                msg.Author.IsBot)
                return;

            ShardedCommandContext ctx = new ShardedCommandContext(_client, msg);
            await _commands.ExecuteAsync(ctx, argPos, _serviceProvider);
        }
    }
}