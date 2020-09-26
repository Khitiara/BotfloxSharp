using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace XivApi
{
    public class XivApi
    {
        private readonly IRestClient _restClient;

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

        public XivApi(string privateKey) {
            _restClient = new RestClient("https://xivapi.com") {
                Authenticator = new Authenticator(privateKey),
                ThrowOnDeserializationError = true, // Probably redundant but being safe here
                ThrowOnAnyError = true
            };
        }

        public Task<T> ApiGetAsync<T>(IRestRequest request,
            CancellationToken cancellationToken = default) =>
            _restClient.GetAsync<T>(request, cancellationToken);
    }
}