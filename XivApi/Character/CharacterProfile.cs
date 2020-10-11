﻿using System;
using XivApi.Character.Raw;

namespace XivApi.Character
{
    public class CharacterProfile
    {
        public struct ContentLevelInfo
        {
            public ContentLevelInfo(int? elementalLevel, bool eurekaCapped) {
                EurekaCapped = eurekaCapped;
                ElementalLevel = elementalLevel > 0 ? elementalLevel : null;
                ResistanceRank = null;
            }

            public int? ElementalLevel { get; }

            public bool EurekaCapped { get; }
            public int? ResistanceRank { get; }
        }

        public enum GenderId
        {
            Unknown = 0,
            Male    = 1,
            Female  = 2
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
        public string? GrandCompanyRank { get; }
        public GenderId Gender { get; }

        public string? FreeCompanyName { get; }
        public string? FreeCompanyTag { get; }

        public CharacterProfile(CharacterResponse response) {
            Raw.Character character = response.Character;
            LodestoneId = character.ID;
            Name = character.Name;

            Avatar = character.Avatar;
            Portrait = character.Portrait;

            ClassJobLevels = new CharacterClassJobs(character.ClassJobs);
            ContentLevels = new ContentLevelInfo(character.ClassJobsElemental.Level,
                character.ClassJobsElemental.ExpLevelMax > 0 && character.ClassJobsElemental.Level > 0);

            DataCenter = character.DC;
            Server = character.Server;

            NameDay = character.Nameday;
            Bio = character.Bio;
            Title = string.IsNullOrWhiteSpace(character.Title.Name) ? null : character.Title.Name;
            TitleTop = character.TitleTop;

            Tribe = character.Tribe.Name;
            Race = character.Race.Name;
            GuardianDeity = character.GuardianDeity.Name;
            GrandCompanyRank = character.GrandCompany.Rank?.Name;
            Gender = (GenderId) character.Gender;

            FreeCompany? freeCompany = response.FreeCompany;
            FreeCompanyName = freeCompany?.Name;
            FreeCompanyTag = freeCompany?.Tag;
        }
    }
}