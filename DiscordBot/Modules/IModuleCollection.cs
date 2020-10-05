using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Botflox.Bot.Modules
{
    public interface IModuleCollection
    {
        Task AddModulesAsync(CommandService commands, IServiceProvider provider);
    }
}