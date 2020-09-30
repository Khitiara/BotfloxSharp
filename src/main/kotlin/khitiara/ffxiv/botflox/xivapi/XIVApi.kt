package khitiara.ffxiv.botflox.xivapi

import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.stereotype.Component
import org.springframework.web.reactive.function.client.WebClient
import org.springframework.web.reactive.function.client.awaitBody

@Component
class XIVApi(private val client: WebClient) {

    companion object {
        const val characterDataUrlTemplate: String = "/character/{id}?extended=1&data=cj,fc&columns=" +
            "Character.ClassJobs,Character.Name,Character.DC,Character.ID,Character.Avatar,Character.Portrait," +
            "Character.Server,FreeCompany.Name,FreeCompany.Tag,Character.Title.Name,Character.Race,Character.Tribe," +
            "Character.TitleTop,Character.Nameday,Character.GuardianDeity,Character.GrandCompany"
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