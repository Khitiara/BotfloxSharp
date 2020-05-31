package khitiara.ffxiv.gearmanagement.xivapi

import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.stereotype.Component
import org.springframework.web.client.RestTemplate
import org.springframework.web.client.getForObject

@Component
class XIVApi(private val template: RestTemplate) {

    companion object {
        const val characterDataUrlTemplate: String = "https://xivapi.com/character/{id}?extended=1&data=cj,fc&columns=Character.ClassJobs.*.Level,Character.ClassJobs.*.Name,Character.Name,Character.DC,Character.ID,Character.Avatar,Character.Server,FreeCompany.Name,Character.Title.Name"
    }

    @Cacheable("xivapi")
    fun getFromLodestone(lodestoneId: Long): XivCharacterResponse =
        template.getForObject(characterDataUrlTemplate, lodestoneId)

    @CachePut("xivapi")
    fun loadDataFromLodestone(lodestoneId: Long): XivCharacterResponse =
        template.getForObject(characterDataUrlTemplate, lodestoneId)
}