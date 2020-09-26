using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Botflox.Bot.Utils
{
    public class ShardChecker : IDisposable
    {
        public readonly DiscordShardedClient Client;
        public HashSet<int> ReadyShards { get; } = new HashSet<int>();

        public bool AllShardsReadyFired { get; private set; }

        public event Func<Task> AllShardsReady {
            add => _allReadyEvent.Add(value);
            remove => _allReadyEvent.Remove(value);
        }

        private readonly AsyncEvent<Func<Task>> _allReadyEvent = new AsyncEvent<Func<Task>>();

        public ShardChecker(DiscordShardedClient client) {
            Client = client;
            client.ShardReady += ShardReadyAsync;
        }

        private async Task ShardReadyAsync(DiscordSocketClient shard) {
            if (AllShardsReadyFired)
                return;
            ReadyShards.Add(shard.ShardId);
            if (Client.Shards.All(x => ReadyShards.Contains(x.ShardId))) {
                AllShardsReadyFired = true;
                await _allReadyEvent.InvokeAsync();
            }
        }

        public void Dispose() {
            Client.ShardReady -= ShardReadyAsync;
        }
    }
}