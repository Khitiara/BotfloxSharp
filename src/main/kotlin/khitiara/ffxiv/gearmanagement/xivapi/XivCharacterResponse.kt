package khitiara.ffxiv.gearmanagement.xivapi


import com.fasterxml.jackson.annotation.JsonProperty

data class XivCharacterResponse(
    @JsonProperty("Character")
    var character: XivCharacterData,
    @JsonProperty("FreeCompany")
    var freeCompany: BasicNamed
) {
    data class XivCharacterData(
        @JsonProperty("ID")
        var id: Int,
        @JsonProperty("Name")
        var name: String,
        @JsonProperty("Avatar")
        var avatar: String,
        @JsonProperty("ClassJobs")
        var classJobs: List<ClassJobInfo>,
        @JsonProperty("DC")
        var dataCenter: String,
        @JsonProperty("Server")
        var server: String,
        @JsonProperty("Title")
        var titleInfo: BasicNamed
    )

    data class ClassJobInfo(
        @JsonProperty("Level")
        var level: Int,
        @JsonProperty("Name")
        var name: String
    )

    data class BasicNamed(
        @JsonProperty("Name")
        var name: String
    )
}