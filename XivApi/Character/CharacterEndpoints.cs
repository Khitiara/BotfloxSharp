using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using XivApi.Pagination;

namespace XivApi.Character
{
    public static class CharacterEndpoints
    {
        private static readonly string Columns = string.Join(',', "Character.ClassJobs",
            "Character.ClassJobsElemental", "Character.Name", "Character.DC", "Character.ID", "Character.Avatar",
            "Character.Portrait", "character.Bio", "Character.Server", "FreeCompany.Name", "FreeCompany.Tag",
            "Character.Title.Name", "Character.Race", "Character.Tribe", "Character.TitleTop", "Character.Nameday");


        private const string Data = "cj,fc";

        public static IRestRequest CharacterByIdRequest(ulong id) => new RestRequest("/character/{id}", DataFormat.Json)
            .AddUrlSegment("id", id)
            .AddQueryParameter("extended", "1")
            .AddQueryParameter("language", "en")
            .AddQueryParameter("data", Data)
            .AddQueryParameter("columns", Columns);

        public static Task<CharacterResponse> CharacterProfileAsync(this XivApiClient client, ulong id,
            CancellationToken cancellationToken = default) =>
            client.ApiGetAsync<CharacterResponse>(CharacterByIdRequest(id), cancellationToken);

        public static IAsyncEnumerable<CharacterSearchResult> CharacterSearchAsync(this XivApiClient client,
            string name, string? dcserver = null, CancellationToken cancellationToken = default) {
            IRestRequest BuildSearch() {
                IRestRequest req =
                    new RestRequest("/character/search", DataFormat.Json).AddQueryParameter("name", name);
                return dcserver != null ? req.AddQueryParameter("server", dcserver) : req;
            }

            return client.GetPaginatedAsync<CharacterSearchResult>(BuildSearch, cancellationToken);
        }

        public static async ValueTask<CharacterResponse> FindSingleCharacterAsync(this XivApiClient client, string name,
            string? dcserver = null, CancellationToken cancellationToken = default) {
            CharacterSearchResult searchResult = await client.CharacterSearchAsync(name, dcserver, cancellationToken)
                .SingleAsync(cancellationToken);
            return await client.CharacterProfileAsync(searchResult.ID, cancellationToken);
        }
    }
}