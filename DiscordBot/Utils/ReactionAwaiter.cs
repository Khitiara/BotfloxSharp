using System;
using System.Collections.Generic;
using System.Threading;
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
            private readonly TaskCompletionSource<SocketReaction> _taskCompletionSource;
            private readonly CancellationTokenRegistration        _registration;

            public Task<SocketReaction> Task => _taskCompletionSource.Task;

            public Token(ReactionAwaiter awaiter, ulong msgId, CancellationToken cancellationToken) {
                _awaiter = awaiter;
                _msgId = msgId;
                _taskCompletionSource = new TaskCompletionSource<SocketReaction>();
                _registration =
                    cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled(cancellationToken));
            }

            public void Post(SocketReaction reaction) {
                _taskCompletionSource.TrySetResult(reaction);
            }

            public async ValueTask DisposeAsync() {
                lock (_awaiter._tokensLock) {
                    _awaiter._tokens.Remove(_msgId);
                }

                await _registration.DisposeAsync();
            }

            public void Dispose() {
                lock (_awaiter._tokensLock) {
                    _awaiter._tokens.Remove(_msgId);
                }

                _registration.Dispose();
            }
        }

        private readonly BaseSocketClient         _client;
        private readonly object                   _tokensLock = new object();
        private readonly Dictionary<ulong, Token> _tokens;

        public ReactionAwaiter(BaseSocketClient client) {
            _client = client;
            _tokens = new Dictionary<ulong, Token>();
            _client.ReactionAdded += ClientOnReactionAdded;
        }

        private Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> msg,
            ISocketMessageChannel channel, SocketReaction reaction) {
            lock (_tokensLock) {
                if (!_tokens.ContainsKey(msg.Id)) return Task.CompletedTask;
                _tokens[msg.Id].Post(reaction);
            }

            return Task.CompletedTask;
        }

        public Token WaitForReaction(IUserMessage message, CancellationToken cancellationToken = default) =>
            WaitForReaction(message.Id, cancellationToken);

        public Token WaitForReaction(ulong id, CancellationToken cancellationToken = default) {
            lock (_tokensLock) {
                if (_tokens.ContainsKey(id)) {
                    throw new InvalidOperationException(
                        "Cannot await reactions on the same message from multiple places");
                }

                Token token = new Token(this, id, cancellationToken);
                _tokens[id] = token;
                return token;
            }
        }

        public void Dispose() {
            _client.ReactionAdded -= ClientOnReactionAdded;
        }
    }
}