using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Botflox.Bot.Utils
{
    public class ReactionAwaiter
    {
        public class Token : IAsyncDisposable, IDisposable
        {
            private readonly ReactionAwaiter                      _awaiter;
            private readonly ulong                                _msgId;
            private readonly Channel<SocketReaction>              _channel;
            private readonly CancellationToken                    _cancellationToken;

            public Token(ReactionAwaiter awaiter, ulong msgId, Channel<SocketReaction> channel, 
                CancellationToken cancellationToken) {
                _awaiter = awaiter;
                _msgId = msgId;
                _channel = channel;
                _cancellationToken = cancellationToken;
            }

            public async void Post(SocketReaction reaction) {
                await _channel.Writer.WriteAsync(reaction, _cancellationToken);
            }

            public async ValueTask DisposeAsync() {
                lock (_awaiter._tokensLock) {
                    _awaiter._tokens.Remove(_msgId);
                }
            }

            public void Dispose() {
                lock (_awaiter._tokensLock) {
                    _awaiter._tokens.Remove(_msgId);
                }
            }
        }

        private readonly BaseSocketClient         _client;
        private readonly object                   _tokensLock = new object();
        private readonly Dictionary<ulong, Token> _tokens;

        public ReactionAwaiter(BaseSocketClient client) {
            _client = client;
            _tokens = new Dictionary<ulong, Token>();
            _client.ReactionAdded += this.ClientOnReactionAdded;
        }

        private Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> msg,
            ISocketMessageChannel channel, SocketReaction reaction) {
            if (reaction.UserId != _client.CurrentUser.Id) {
                lock (_tokensLock) {
                    if (!_tokens.ContainsKey(msg.Id)) return Task.CompletedTask;
                    _tokens[msg.Id].Post(reaction);
                }
            }

            return Task.CompletedTask;
        }

        public Token WaitForReaction(IUserMessage message, Channel<SocketReaction> channel, 
            CancellationToken cancellationToken = default) =>
            WaitForReaction(channel, message.Id, cancellationToken);

        public Token WaitForReaction(Channel<SocketReaction> channel, ulong id, 
            CancellationToken cancellationToken = default) {
            lock (_tokensLock) {
                if (_tokens.ContainsKey(id)) {
                    throw new InvalidOperationException(
                        "Cannot await reactions on the same message from multiple places");
                }

                Token token = new Token(this, id, channel, cancellationToken);
                _tokens[id] = token;
                return token;
            }
        }

        public void Dispose() {
            _client.ReactionAdded -= this.ClientOnReactionAdded;
        }
    }
}