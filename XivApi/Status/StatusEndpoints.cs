using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace XivApi.Status
{
    public static class StatusEndpoints
    {
        public static async Task<ImmutableDictionary<string, ServerStatus>> GetServerStatusAsync(this XivApiClient xivApiClient,
            CancellationToken cancellationToken = default) =>
            (await xivApiClient.ApiGetAsync<List<ServerStatus>>(
                new RestRequest("https://api.xivstatus.com/api/servers", DataFormat.Json),
                cancellationToken: cancellationToken)).ToImmutableDictionary(status => status.Name);
    }
}