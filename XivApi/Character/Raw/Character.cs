#pragma warning disable 8618
using System;
using System.Collections.Generic;

namespace XivApi.Character.Raw
{
    public class Character
    {
        public Uri Avatar { get; set; }
        public string Bio { get; set; }
        public List<ClassJobInfo> ClassJobs { get; set; }
        public ClassJobsBozjan ClassJobsBozjan { get; set; }
        public ClassJobsElemental ClassJobsElemental { get; set; }
        public string DC { get; set; }
        public ulong ID { get; set; }
        public string Name { get; set; }
        public string Nameday { get; set; }
        public Uri Portrait { get; set; }
        public Race Race { get; set; }
        public string Server { get; set; }
        public Title Title { get; set; }
        public bool TitleTop { get; set; }
        public int Gender { get; set; }
        public Tribe Tribe { get; set; }
        public GrandCompany GrandCompany { get; set; }
        public GuardianDeityInfo GuardianDeity { get; set; }
    }
}