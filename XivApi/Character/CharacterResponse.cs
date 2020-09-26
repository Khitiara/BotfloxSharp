using System;
using RestSharp;

namespace XivApi.Character
{
    public class CharacterResponse
    {
        public Character Character { get; set; }
        public FreeCompany FreeCompany { get; set; }

        private static readonly string Columns = string.Join(',', "Character.ClassJobs",
            "Character.ClassJobsElemental", "Character.Name", "Character.DC", "Character.ID", "Character.Avatar",
            "Character.Portrait", "Character.Server", "FreeCompany.Name", "FreeCompany.Tag", "Character.Title.Name",
            "Character.Race", "Character.Tribe", "Character.TitleTop", "Character.Nameday");


        private const string Data = "cj,fc";

        public IRestRequest CharacterById(long id) => new RestRequest("/character/{id}", DataFormat.Json)
            .AddUrlSegment("id", id)
            .AddQueryParameter("extended", "1")
            .AddQueryParameter("language", "en")
            .AddQueryParameter("data", Data)
            .AddQueryParameter("columns", Columns);
    }
}