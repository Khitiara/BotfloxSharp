package khitiara.ffxiv.botflox.xivapi

object ClassJobs {
    interface ClassJob {
        val id: Int
        val icon: String
        val abbrev: String
    }

    @Suppress("USELESS_CAST")
    fun values(): Collection<ClassJob> = arrayOf(
        CombatClass.values(),
        CombatJob.values(),
        CombatLimitedJob.values(),
        HandClass.values(),
        LandClass.values()
    ).flatten().map { it as ClassJob }

    private val idMap: Map<Int, ClassJob> = values().map { it.id to it }.toMap()
    fun byId(id: Int): ClassJob? = idMap[id]

    enum class CombatClass(override val id: Int, override val icon: String, override val abbrev: String) : ClassJob {
        Gladiator(1, "/cj-icons/gladiator.png", "GLA"),
        Pugilist(2, "/cj-icons/pugilist.png", "PGL"),
        Marauder(3, "/cj-icons/marauder.png", "MRD"),
        Lancer(4, "/cj-icons/lancer.png", "LNC"),
        Archer(5, "/cj-icons/archer.png", "ARC"),
        Conjurer(6, "/cj-icons/conjurer.png", "CNJ"),
        Thaumaturge(7, "/cj-icons/thaumaturge.png", "THM"),
        Arcanist(26, "/cj-icons/arcanist.png", "ACN"),
        Rogue(29, "/cj-icons/rogue.png", "ROG")
    }

    enum class CombatJob(override val id: Int, override val icon: String, override val abbrev: String, val clazz: CombatClass?) : ClassJob {
        Paladin(19, "/cj-icons/paladin.png", "PLD", CombatClass.Gladiator),
        Monk(20, "/cj-icons/monk.png", "MNK", CombatClass.Pugilist),
        Warrior(21, "/cj-icons/warrior.png", "WAR", CombatClass.Marauder),
        Dragoon(22, "/cj-icons/dragoon.png", "DRG", CombatClass.Lancer),
        Bard(23, "/cj-icons/bard.png", "BRD", CombatClass.Archer),
        WhiteMage(24, "/cj-icons/whitemage.png", "WHM", CombatClass.Conjurer),
        BlackMage(25, "/cj-icons/blackmage.png", "BLM", CombatClass.Thaumaturge),
        Summoner(27, "/cj-icons/summoner.png", "SMN", CombatClass.Arcanist),
        Scholar(28, "/cj-icons/scholar.png", "SCH", CombatClass.Arcanist),
        Ninja(30, "/cj-icons/ninja.png", "NIN", CombatClass.Rogue),
        Machinist(31, "/cj-icons/machinist.png", "MCH", null),
        DarkKnight(32, "/cj-icons/darkknight.png", "DRK", null),
        Astrologian(33, "/cj-icons/astrologian.png", "AST", null),
        Samurai(34, "/cj-icons/samurai.png", "SAM", null),
        RedMage(35, "/cj-icons/redmage.png", "RDM", null),
        Gunbreaker(37, "/cj-icons/gunbreaker.png", "GNB", null),
        Dancer(38, "/cj-icons/dancer.png", "DNC", null)
    }

    enum class CombatLimitedJob(override val id: Int, override val icon: String, override val abbrev: String) : ClassJob {
        BlueMage(36, "/cj-icons/bluemage.png", "BLU")
    }

    enum class HandClass(override val id: Int, override val icon: String, override val abbrev: String) : ClassJob {
        Carpenter(8, "/cj-icons/carpenter.png", "CRP"),
        Blacksmith(9, "/cj-icons/blacksmith.png", "BSM"),
        Armorer(10, "/cj-icons/armorer.png", "ARM"),
        Goldsmith(11, "/cj-icons/goldsmith.png", "GSM"),
        Leatherworker(12, "/cj-icons/leatherworker.png", "LTW"),
        Weaver(13, "/cj-icons/weaver.png", "WVR"),
        Alchemist(14, "/cj-icons/alchemist.png", "ALC"),
        Culinarian(15, "/cj-icons/culinarian.png", "CUL"),
    }

    enum class LandClass(override val id: Int, override val icon: String, override val abbrev: String) : ClassJob {
        Miner(16, "/cj-icons/miner.png", "MIN"),
        Botanist(17, "/cj-icons/botanist.png", "BTN"),
        Fisher(18, "/cj-icons/fisher.png", "FSH")
    }
}