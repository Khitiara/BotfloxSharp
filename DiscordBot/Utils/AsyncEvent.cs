using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using RestSharp.Validation;

namespace Botflox.Bot.Utils
{
    public class AsyncEvent<T> where T : class
    {
        private readonly object            _subLock = new object();
        private          ImmutableArray<T> _subscriptions;

        public bool HasSubscribers => (uint) _subscriptions.Length > 0U;

        public IReadOnlyList<T> Subscriptions => _subscriptions;

        public AsyncEvent() => _subscriptions = ImmutableArray.Create<T>();

        public void Add(T subscriber) {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));
            object subLock = _subLock;
            lock (subLock) _subscriptions = _subscriptions.Add(subscriber);
        }

        public void Remove(T subscriber) {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));
            object subLock = _subLock;
            lock (subLock) _subscriptions = _subscriptions.Remove(subscriber);
        }
    }

    public static class EventExtensions
    {
        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler) {
            IReadOnlyList<Func<Task>> subscribers = eventHandler.Subscriptions;
            foreach (Func<Task> func in subscribers)
                await func().ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg) {
            IReadOnlyList<Func<T, Task>> subscribers = eventHandler.Subscriptions;
            foreach (Func<T, Task> func in subscribers)
                await func(arg).ConfigureAwait(false);
        }
    }
}