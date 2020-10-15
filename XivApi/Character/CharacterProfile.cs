using System;
using XivApi.Character.Raw;

namespace XivApi.Character
{
    public class CharacterProfile
    {
        public struct ContentLevelInfo
        {
            public ContentLevelInfo(int? elementalLevel, bool eurekaCapped, int? bozjaLevel) {
                int bozjaMaxLevel = 15;

                ElementalLevel = elementalLevel > 0 ? elementalLevel : null;
                EurekaCapped = eurekaCapped;
                
                ResistanceRank = bozjaLevel > 0 ? bozjaLevel : null;
                ResistanceCapped = (bozjaLevel == bozjaMaxLevel);
            }

            public int? ElementalLevel { get; }
            public bool EurekaCapped { get; }

            public int? ResistanceRank { get; }
            public bool ResistanceCapped { get; }
        }

        public enum GenderId
        {
            Unknown = 0,
            Male    = 1,
            Female  = 2
        }

        public struct FcCrest {
            public string? Background;
            public string? Shape;
            public string? Icon;
        }

        public ulong LodestoneId { get; }
        public string Name { get; }

        public Uri Avatar { get; }
        public Uri Portrait { get; }

        public CharacterClassJobs ClassJobLevels { get; }
        public ContentLevelInfo ContentLevels { get; }

        public string DataCenter { get; }
        public string Server { get; }

        public string NameDay { get; }
        public string Bio { get; }
        public string? Title { get; }

        public bool TitleTop { get; }
        public string Tribe { get; }
        public string Race { get; }
        public string GuardianDeity { get; }
        public GrandCompany? GrandCompany { get; }
        public GenderId Gender { get; }

        public ulong? FreeCompanyId { get; }
        public string? FreeCompanyName { get; }
        public string? FreeCompanyTag { get; }
        public FcCrest? FreeCompanyCrest { get; }

        public CharacterProfile(CharacterResponse response) {
            Raw.Character character = response.Character;
            LodestoneId = character.ID;
            Name = character.Name;

            Avatar = character.Avatar;
            Portrait = character.Portrait;

            ClassJobLevels = new CharacterClassJobs(character.ClassJobs);
            ContentLevels = new ContentLevelInfo(
                character.ClassJobsElemental.Level,
                character.ClassJobsElemental.ExpLevelMax == 0 && character.ClassJobsElemental.Level > 0,
                character.ClassJobsBozjan.Level);

            DataCenter = character.DC;
            Server = character.Server;

            NameDay = character.Nameday;
            Bio = character.Bio;
            Title = string.IsNullOrWhiteSpace(character.Title.Name) ? null : character.Title.Name;
            TitleTop = character.TitleTop;

            Tribe = character.Tribe.Name;
            Race = character.Race.Name;
            GuardianDeity = character.GuardianDeity.Name;
            GrandCompany = character.GrandCompany;
            Gender = (GenderId) character.Gender;

            FreeCompany? freeCompany = response.FreeCompany;
            FreeCompanyId = (freeCompany?.ID != null) ? ulong.Parse(freeCompany?.ID ?? "0") : null;
            FreeCompanyName = freeCompany?.Name;
            FreeCompanyTag = freeCompany?.Tag;
            FreeCompanyCrest = (freeCompany?.ID != null) ? new FcCrest{
                Background = freeCompany?.Crest[0],
                Shape = freeCompany?.Crest[1] ,
                Icon = freeCompany?.Crest[2]
            } : null;
        }
    }
}