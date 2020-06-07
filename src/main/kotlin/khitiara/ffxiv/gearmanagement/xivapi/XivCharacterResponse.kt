package khitiara.ffxiv.gearmanagement.xivapi


import com.fasterxml.jackson.annotation.JsonIgnoreProperties
import com.fasterxml.jackson.annotation.JsonProperty

private val jobIdToIconPath: Map<Int, String> = mapOf(
    1 to "/cj-icons/gladiator.png",
    2 to "/cj-icons/pugilist.png",
    3 to "/cj-icons/marauder.png",
    4 to "/cj-icons/lancer.png",
    5 to "/cj-icons/archer.png",
    6 to "/cj-icons/conjurer.png",
    7 to "/cj-icons/thaumaturge.png",
    8 to "/cj-icons/carpenter.png",
    9 to "/cj-icons/blacksmith.png",
    10 to "/cj-icons/armorer.png",
    11 to "/cj-icons/goldsmith.png",
    12 to "/cj-icons/leatherworker.png",
    13 to "/cj-icons/weaver.png",
    14 to "/cj-icons/alchemist.png",
    15 to "/cj-icons/culinarian.png",
    16 to "/cj-icons/miner.png",
    17 to "/cj-icons/botanist.png",
    18 to "/cj-icons/fisher.png",
    19 to "/cj-icons/paladin.png",
    20 to "/cj-icons/monk.png",
    21 to "/cj-icons/warrior.png",
    22 to "/cj-icons/dragoon.png",
    23 to "/cj-icons/bard.png",
    24 to "/cj-icons/whitemage.png",
    25 to "/cj-icons/blackmage.png",
    26 to "/cj-icons/arcanist.png",
    27 to "/cj-icons/summoner.png",
    28 to "/cj-icons/scholar.png",
    29 to "/cj-icons/rogue.png",
    30 to "/cj-icons/ninja.png",
    31 to "/cj-icons/machinist.png",
    32 to "/cj-icons/darkknight.png",
    33 to "/cj-icons/astrologian.png",
    34 to "/cj-icons/samurai.png",
    35 to "/cj-icons/redmage.png",
    36 to "/cj-icons/bluemage.png",
    37 to "/cj-icons/gunbreaker.png",
    38 to "/cj-icons/dancer.png"
)

data class XivCharacterResponse(
    @JsonProperty("Character")
    var character: XivCharacterData,
    @JsonProperty("FreeCompany")
    var freeCompany: FreeCompanyData
) {
    data class XivCharacterData(
        @JsonProperty("ID")
        var id: Int,
        @JsonProperty("Name")
        var name: String,
        @JsonProperty("Avatar")
        var avatar: String,
        @JsonProperty("Portrait")
        var portrait: String,
        @JsonProperty("ClassJobs")
        var classJobs: List<ClassJobInfo>,
        @JsonProperty("DC")
        var dataCenter: String,
        @JsonProperty("Server")
        var server: String,
        @JsonProperty("Title")
        var titleInfo: BasicNamed,
        @JsonProperty("TitleTop")
        var titleTop: Boolean,
        @JsonProperty("Tribe")
        var tribe: BasicNamed,
        @JsonProperty("Race")
        var race: BasicNamed,
        @JsonProperty("Nameday")
        var nameday: String
    )

    @JsonIgnoreProperties(ignoreUnknown = true)
    data class ClassJobInfo(
        @JsonProperty("Level")
        var level: Int,
        @JsonProperty("Name")
        var name: String,
        @JsonProperty("UnlockedState")
        var unlockedState: BasicNamed
    ) {
        val iconUrl: String
            @JsonProperty("IconUrl", access = JsonProperty.Access.READ_ONLY)
            get() = jobIdToIconPath[unlockedState.id]
                ?: (if (unlockedState.name == "Blue Mage (Limited Job)") jobIdToIconPath[36] else null)
                ?: error("Somehow unrecognized job ${unlockedState.name}")
    }

    data class FreeCompanyData(
        @JsonProperty("Name")
        var name: String,
        @JsonProperty("Tag")
        var tag: String
    )
}