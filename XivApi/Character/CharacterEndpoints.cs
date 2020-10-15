using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using XivApi.Character.Raw;
using XivApi.Pagination;

namespace XivApi.Character
{
    public static class CharacterEndpoints
    {
        private static readonly string Columns = string.Join(',', "Character.ClassJobs", "Character.Name", 
            "Character.DC", "Character.ID", "Character.Avatar", "Character.Portrait", "Character.Bio", 
            "Character.Server", "Character.ClassJobsElemental", "FreeCompany.Name", "FreeCompany.Tag", 
            "FreeCompany.Crest", "FreeCompany.ID", "Character.Title.Name", "Character.Race", "Character.Tribe", 
            "Character.TitleTop", "Character.Nameday", "Character.GuardianDeity", "Character.GrandCompany", 
            "Character.Gender");

        private const string Data = "cj,fc";

        public static IRestRequest CharacterByIdRequest(ulong id) => new RestRequest("/character/{id}", DataFormat.Json)
            .AddUrlSegment("id", id)
            .AddQueryParameter("extended", "1")
            .AddQueryParameter("language", "en")
            .AddQueryParameter("data", Data)
            .AddQueryParameter("columns", Columns);

        public static async Task<CharacterProfile> CharacterProfileAsync(this XivApiClient client, ulong id,
            CancellationToken cancellationToken = default) {
            CharacterResponse response =
                await client.ApiGetAsync<CharacterResponse>(CharacterByIdRequest(id), id.ToString(), cancellationToken);
            return await Task.Run(() => ProcessCharacterProfile(response), cancellationToken);
        }

        public static IAsyncEnumerable<CharacterSearchResult> CharacterSearchAsync(this XivApiClient client,
            string name, string? dcserver = null, CancellationToken cancellationToken = default) {
            IRestRequest BuildSearch() {
                IRestRequest req =
                    new RestRequest("/character/search", DataFormat.Json).AddQueryParameter("name", name);
                return dcserver != null ? req.AddQueryParameter("server", dcserver) : req;
            }

            return client.GetPaginatedAsync<CharacterSearchResult>(BuildSearch, $"search:{name}:{dcserver}",
                cancellationToken);
        }

        public static async ValueTask<CharacterProfile> FindSingleCharacterAsync(this XivApiClient client, string name,
            string? dcserver = null, CancellationToken cancellationToken = default) {
            CharacterSearchResult searchResult = await client.CharacterSearchAsync(name, dcserver, cancellationToken)
                .SingleAsync(cancellationToken);
            return await client.CharacterProfileAsync(searchResult.ID, cancellationToken);
        }

        public static CharacterProfile ProcessCharacterProfile(CharacterResponse response) =>
            new CharacterProfile(response);
    }
}