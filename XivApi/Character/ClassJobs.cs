using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace XivApi.Character
{
    public interface IClassJob : IEquatable<IClassJob>
    {
        int Id { get; }
        Bitmap Icon { get; }
        string Abbrev { get; }

        string Name { get; }
    }

    public abstract class BaseClassJob : IClassJob
    {
        private static readonly Dictionary<int, IClassJob> _classJobs = new Dictionary<int, IClassJob>();

        public static IReadOnlyDictionary<int, IClassJob> ClassJobs { get; } =
            new ReadOnlyDictionary<int, IClassJob>(_classJobs);

        protected BaseClassJob(int id, string abbrev, string name) {
            Id = id;
            Icon = (Bitmap) Resources.ResourceManager.GetObject(name.ToLowerInvariant(), Resources.Culture)!;
            Abbrev = abbrev.ToUpperInvariant();
            Name = name;
            _classJobs[Id] = this;
        }

        public bool Equals(IClassJob? other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return Id == other.Id;
        }

        public int Id { get; }
        public Bitmap Icon { get; }
        public string Abbrev { get; }
        public string Name { get; }
    }

    public class CombatClass : BaseClassJob
    {
        internal static void Init() { }
        private CombatClass(int id, string abbrev, string name) : base(id, abbrev, name) { }

        public static readonly CombatClass Gladiator,
            Pugilist,
            Marauder,
            Lancer,
            Archer,
            Conjurer,
            Thaumaturge,
            Arcanist,
            Rogue;

        static CombatClass() {
            Gladiator = new CombatClass(1, "GLA", "Gladiator");
            Pugilist = new CombatClass(2, "PGL", "Pugilist");
            Marauder = new CombatClass(3, "MRD", "Marauder");
            Lancer = new CombatClass(4, "LNC", "Lancer");
            Archer = new CombatClass(5, "ARC", "Archer");
            Conjurer = new CombatClass(6, "CNJ", "Conjurer");
            Thaumaturge = new CombatClass(7, "THM", "Thaumaturge");
            Arcanist = new CombatClass(26, "ACN", "Arcanist");
            Rogue = new CombatClass(29, "ROG", "Rogue");
        }
    }

    public class CombatJob : BaseClassJob
    {
        internal static void Init() { }
        public CombatClass? Class { get; }

        private CombatJob(int id, string abbrev, string name, CombatClass? @class) : base(id, abbrev, name) {
            Class = @class;
        }

        public static readonly CombatJob
            Paladin,
            Monk,
            Warrior,
            Dragoon,
            Bard,
            WhiteMage,
            BlackMage,
            Summoner,
            Scholar,
            Ninja,
            Machinist,
            DarkKnight,
            Astrologian,
            Samurai,
            RedMage,
            Gunbreaker,
            Dancer;

        static CombatJob() {
            Dancer = new CombatJob(38, "DNC", "Dancer", null);
            Gunbreaker = new CombatJob(37, "GNB", "Gunbreaker", null);
            RedMage = new CombatJob(35, "RDM", "RedMage", null);
            Samurai = new CombatJob(34, "SAM", "Samurai", null);
            Astrologian = new CombatJob(33, "AST", "Astrologian", null);
            DarkKnight = new CombatJob(32, "DRK", "DarkKnight", null);
            Machinist = new CombatJob(31, "MCH", "Machinist", null);
            Ninja = new CombatJob(30, "NIN", "Ninja", CombatClass.Rogue);
            Scholar = new CombatJob(28, "SCH", "Scholar", CombatClass.Arcanist);
            Summoner = new CombatJob(27, "SMN", "Summoner", CombatClass.Arcanist);
            BlackMage = new CombatJob(25, "BLM", "BlackMage", CombatClass.Thaumaturge);
            WhiteMage = new CombatJob(24, "WHM", "WhiteMage", CombatClass.Conjurer);
            Bard = new CombatJob(23, "BRD", "Bard", CombatClass.Archer);
            Dragoon = new CombatJob(22, "DRG", "Dragoon", CombatClass.Lancer);
            Warrior = new CombatJob(21, "WAR", "Warrior", CombatClass.Marauder);
            Monk = new CombatJob(20, "MNK", "Monk", CombatClass.Pugilist);
            Paladin = new CombatJob(19, "PLD", "Paladin", CombatClass.Gladiator);
        }
    }

    public class LimitedCombatJob : BaseClassJob
    {
        internal static void Init() { }
        private LimitedCombatJob(int id, string abbrev, string name) : base(id, abbrev, name) { }

        public static readonly LimitedCombatJob
            BlueMage;

        static LimitedCombatJob() {
            BlueMage = new LimitedCombatJob(36, "BLU", "BlueMage");
        }
    }

    public class HandClass : BaseClassJob
    {
        internal static void Init() { }
        private HandClass(int id, string abbrev, string name) : base(id, abbrev, name) { }

        public static readonly HandClass
            Carpenter,
            Blacksmith,
            Armorer,
            Goldsmith,
            Leatherworker,
            Weaver,
            Alchemist,
            Culinarian;

        static HandClass() {
            Culinarian = new HandClass(15, "CUL", "Culinarian");
            Alchemist = new HandClass(14, "ALC", "Alchemist");
            Weaver = new HandClass(13, "WVR", "Weaver");
            Leatherworker = new HandClass(12, "LTW", "Leatherworker");
            Goldsmith = new HandClass(11, "GSM", "Goldsmith");
            Armorer = new HandClass(10, "ARM", "Armorer");
            Blacksmith = new HandClass(9, "BSM", "Blacksmith");
            Carpenter = new HandClass(8, "CRP", "Carpenter");
        }
    }

    public class LandClass : BaseClassJob
    {
        internal static void Init() { }

        private LandClass(int id, string abbrev, string name) : base(id, abbrev, name) { }

        public static readonly LandClass
            Miner,
            Botanist,
            Fisher;

        static LandClass() {
            Fisher = new LandClass(18, "FSH", "Fisher");
            Botanist = new LandClass(17, "BTN", "Botanist");
            Miner = new LandClass(16, "MIN", "Miner");
        }
    }

    public static class ClassJobs
    {
        static ClassJobs() {
            CombatClass.Init();
            CombatJob.Init();
            LimitedCombatJob.Init();
            HandClass.Init();
            LandClass.Init();
            Console.WriteLine("ClassJobs Initialized");
        }

        public static IReadOnlyDictionary<int, IClassJob> ById => BaseClassJob.ClassJobs;

        public static IReadOnlyDictionary<string, IClassJob> ByAbbrev =>
            ById.Values.ToImmutableDictionary(job => job.Abbrev);

        public static IClassJob FindByName(string name) =>
            ById.Values.Single(job => job.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
    }
}