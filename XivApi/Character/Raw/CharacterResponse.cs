#pragma warning disable 8618
using System;

namespace XivApi.Character.Raw
{
    public class CharacterResponse
    {
        public Character Character { get; set; }
        public FreeCompany? FreeCompany { get; set; }
    }

    public class CharacterSearchResult
    {
        public Uri Avatar { get; set; }
        public int FeastMatches { get; set; }
        public ulong ID { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public object Rank { get; set; }
        public object RankIcon { get; set; }
        public string Server { get; set; }
    }
}