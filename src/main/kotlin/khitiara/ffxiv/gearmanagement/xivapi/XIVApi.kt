package khitiara.ffxiv.gearmanagement.xivapi

import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.stereotype.Component
import org.springframework.web.reactive.function.client.WebClient
import org.springframework.web.reactive.function.client.awaitBody

@Component
class XIVApi(private val client: WebClient) {

    companion object {
        const val characterDataUrlTemplate: String = "https://xivapi.com/character/{id}?extended=1&data=cj," +
            "fc&columns=Character.ClassJobs.*.Level,Character.ClassJobs.*.Name,Character.Name,Character.DC," +
            "Character.ID,Character.Avatar,Character.Server,FreeCompany.Name,Character.Title.Name"
    }

    @Cacheable("xivapi")
    suspend fun getFromLodestone(lodestoneId: Long): XivCharacterResponse =
        client.get().uri(characterDataUrlTemplate, lodestoneId)
            .retrieve().awaitBody()

    @CachePut("xivapi")
    suspend fun loadDataFromLodestone(lodestoneId: Long): XivCharacterResponse =
        client.get().uri(characterDataUrlTemplate, lodestoneId)
            .retrieve().awaitBody()
}