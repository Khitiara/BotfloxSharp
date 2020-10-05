using System.Threading.Tasks;
using Botflox.Bot;
using Botflox.Bot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Migrator
{
    public class Program
    {
        public static async Task Main(string[] args) {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(BotfloxInit.ConfigureServices).Build();
            BotfloxDatabase database = host.Services.GetRequiredService<BotfloxDatabase>();
            await database.Database.MigrateAsync();
        }
    }
}