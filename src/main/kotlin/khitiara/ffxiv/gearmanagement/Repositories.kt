package khitiara.ffxiv.gearmanagement

import khitiara.ffxiv.gearmanagement.xivapi.XIVApi
import org.springframework.data.jpa.repository.JpaRepository
import java.util.*

interface StaticRepository : JpaRepository<Static, UUID> {
    fun findBySlug(slug: String): Static?

}

abstract class CharacterRepository(private val xivApi: XIVApi) : JpaRepository<XivCharacter, Long> {

    abstract fun findByLodestoneId(lodestoneId: Long): XivCharacter?
    fun updateFromLodestone(lodestoneId: Long): XivCharacter {
        val oldCharacter = findByLodestoneId(lodestoneId)
        val lodestoneData = xivApi.loadDataFromLodestone(lodestoneId)
        return XivCharacter(lodestoneId, lodestoneData.character.name, oldCharacter?.static, oldCharacter?.discordUser)
            .also { save(it) }
    }
}