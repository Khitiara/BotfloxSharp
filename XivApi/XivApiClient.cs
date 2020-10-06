using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using RestSharp.Authenticators;

namespace XivApi
{
    public class XivApiClient
    {
        private readonly IRestClient   _restClient;
        private readonly IMemoryCache? _cache;

        private class Authenticator : IAuthenticator
        {
            private readonly string _privateKey;

            public Authenticator(string privateKey) {
                _privateKey = privateKey;
            }

            public void Authenticate(IRestClient client, IRestRequest request) {
                request.AddQueryParameter("private_key", _privateKey);
            }
        }

        public XivApiClient(string privateKey, IMemoryCache? cache) {
            _cache = cache;
            _restClient = new RestClient("https://xivapi.com") {
                Authenticator = new Authenticator(privateKey),
                ThrowOnDeserializationError = true, // Probably redundant but being safe here
                ThrowOnAnyError = true
            };
        }

        public Task<T> ApiGetAsync<T>(IRestRequest request, string? cacheKey = null,
            CancellationToken cancellationToken = default) => _cache == null || cacheKey == null
            ? _restClient.GetAsync<T>(request, cancellationToken)
            : _cache.GetOrCreateAsync(cacheKey, entry => {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(12));
                entry.SetSlidingExpiration(TimeSpan.FromHours(3));
                return _restClient.GetAsync<T>(request, cancellationToken);
            });
    }
}