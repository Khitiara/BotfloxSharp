using System;
using Botflox.Bot.Data;
using Botflox.Bot.Utils;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using XivApi;

namespace Botflox.Bot
{
    public static class BotfloxInit
    {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, sc) => ConfigureServices(sc));

        public static void ConfigureServices(IServiceCollection services) {
            services.AddSingleton(sp => sp)
                .AddDbContextPool<BotfloxDatabase>((sp, o) => o
                    .UseMySql(sp.GetRequiredService<IConfiguration>().GetConnectionString("BotfloxDb"), mySqlOpts =>
                        mySqlOpts.ServerVersion(new Version(10, 5, 5), ServerType.MariaDb)))
                .AddSingleton(x => {
                    IConfiguration config = x.GetRequiredService<IConfiguration>();
                    ILogger<DiscordShardedClient> logger = x.GetRequiredService<ILogger<DiscordShardedClient>>();
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
                .AddScoped(x => new CommandService(new CommandServiceConfig {
                    CaseSensitiveCommands = false,
                    ThrowOnError = false,
                    DefaultRunMode = RunMode.Async,
                    IgnoreExtraArgs = false,
                    LogLevel = LogSeverity.Info
                }))
                .AddScoped(x => new XivApiClient(x.GetRequiredService<IConfiguration>()["XivApiKey"]))
                .AddScoped<GobbieCommandHandler>()
                .AddHostedService<BotfloxService>();
        }
    }
}