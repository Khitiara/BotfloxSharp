using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;

namespace Botflox.Bot.Modules
{
    public class MainModuleCollection : IModuleCollection
    {
        public async Task AddModulesAsync(CommandService commands, IServiceProvider provider) {
            await commands.AddModulesAsync(Assembly.GetAssembly(typeof(MainModuleCollection)), provider);
        }
    }
}