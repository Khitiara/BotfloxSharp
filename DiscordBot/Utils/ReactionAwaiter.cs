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
        public class Token : IDisposable
        {
            private readonly ReactionAwaiter                              _awaiter;
            private readonly ulong                                        _msgId;
            private readonly WeakReference<ChannelWriter<SocketReaction>> _channelWriter;
            private readonly CancellationToken                            _cancellationToken;

            public Token(ReactionAwaiter awaiter, ulong msgId,
                ChannelWriter<SocketReaction> channelWriter, CancellationToken cancellationToken) {
                _awaiter = awaiter;
                _msgId = msgId;
                _channelWriter = new WeakReference<ChannelWriter<SocketReaction>>(channelWriter);
                _cancellationToken = cancellationToken;
            }


            public async void Post(SocketReaction reaction) {
                if (_channelWriter.TryGetTarget(out ChannelWriter<SocketReaction>? writer))
                    await writer.WriteAsync(reaction, _cancellationToken);
            }

            public void Dispose() {
                lock (_awaiter._tokensLock) {
                    _awaiter._tokens.Remove(_msgId);
                }

                GC.SuppressFinalize(this);
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
            if (reaction.UserId == _client.CurrentUser.Id) return Task.CompletedTask;
            lock (_tokensLock) {
                if (!_tokens.ContainsKey(msg.Id)) return Task.CompletedTask;
                _tokens[msg.Id].Post(reaction);
            }

            return Task.CompletedTask;
        }

        public Token WaitForReaction(IUserMessage message, out ChannelReader<SocketReaction> channel,
            CancellationToken cancellationToken = default) =>
            WaitForReaction(message.Id, out channel, cancellationToken);

        public Token WaitForReaction(ulong messageId, out ChannelReader<SocketReaction> channelReader,
            CancellationToken cancellationToken = default) {
            lock (_tokensLock) {
                if (_tokens.ContainsKey(messageId)) {
                    throw new InvalidOperationException(
                        "Cannot await reactions on the same message from multiple places");
                }

                Channel<SocketReaction> channel = Channel.CreateUnbounded<SocketReaction>();
                Token token = new Token(this, messageId, channel.Writer, cancellationToken);
                _tokens[messageId] = token;
                channelReader = channel.Reader;
                return token;
            }
        }

        public void Dispose() {
            _client.ReactionAdded -= ClientOnReactionAdded;
        }
    }
}