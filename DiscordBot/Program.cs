using System;
using Botflox.Bot.Utils;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XivApi;

namespace Botflox.Bot
{
    public static class Program
    {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => services
                    .AddSingleton<IServiceProvider>(sp => sp)
                    .AddSingleton(x => {
                        IConfiguration config = x.GetRequiredService<IConfiguration>();
                        ILogger<DiscordShardedClient>
                            logger = x.GetRequiredService<ILogger<DiscordShardedClient>>();
                        IConfigurationSection discordSection = config.GetSection("Discord");
                        DiscordShardedClient client = new DiscordShardedClient(new DiscordSocketConfig {
                            ExclusiveBulkDelete = true,
                            LogLevel = LogSeverity.Info,
                            AlwaysDownloadUsers = false,
                            MessageCacheSize = discordSection.GetValue("MessageCacheSize", 50),
                            TotalShards = discordSection.GetValue("Shards", 1)
                        });
                        client.Log += logger.LogDiscord;
                        return client;
                    })
                    .AddSingleton<Random>()
                    .AddSingleton<CommandService>(x => new CommandService(new CommandServiceConfig {
                        CaseSensitiveCommands = false,
                        ThrowOnError = false,
                        DefaultRunMode = RunMode.Async,
                        IgnoreExtraArgs = false,
                        LogLevel = LogSeverity.Info
                    }))
                    .AddSingleton<XivApiClient>(x =>
                        new XivApiClient(x.GetRequiredService<IConfiguration>()["XivApiKey"]))
                    .AddSingleton<GobbieCommandHandler>()
                    .AddHostedService<BotfloxService>());
    }
}