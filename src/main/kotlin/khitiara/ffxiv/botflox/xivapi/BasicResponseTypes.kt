package khitiara.ffxiv.botflox.xivapi

import com.fasterxml.jackson.annotation.JsonProperty

data class BasicNamed(
    @JsonProperty("ID")
    var id: Int,
    @JsonProperty("Name")
    var name: String
)