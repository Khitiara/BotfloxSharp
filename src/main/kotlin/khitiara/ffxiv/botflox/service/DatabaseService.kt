package khitiara.ffxiv.botflox.service

import khitiara.ffxiv.botflox.CharacterRepository
import khitiara.ffxiv.botflox.Static
import khitiara.ffxiv.botflox.StaticRepository
import khitiara.ffxiv.botflox.XivCharacter
import khitiara.ffxiv.botflox.xivapi.XIVApi
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.springframework.stereotype.Service

@Service
class DatabaseService(private var xivApi: XIVApi, private var staticRepository: StaticRepository, private var characterRepository: CharacterRepository) {

    suspend fun findStaticBySlug(slug: String): Static? = withContext(Dispatchers.IO) { staticRepository.findBySlug(slug) }

    suspend fun findCharacterByLodestoneId(lodestoneId: Long): XivCharacter? = withContext(Dispatchers.IO) { characterRepository.findByLodestoneId(lodestoneId) }

    suspend fun updateCharacterFromLodestone(lodestoneId: Long): XivCharacter {
        val oldCharacter = findCharacterByLodestoneId(lodestoneId)
        val lodestoneData = xivApi.loadDataFromLodestone(lodestoneId)
        return XivCharacter(lodestoneId, lodestoneData.character.name, oldCharacter?.static, oldCharacter?.discordUser)
            .also { characterRepository.save(it) }
    }

    suspend fun lodestoneDataForCharacter(character: XivCharacter) = xivApi.getFromLodestone(character.lodestoneId)

    suspend fun createCharacterFromLodestone(lodestoneId: Long): XivCharacter {
        val lodestoneData = xivApi.loadDataFromLodestone(lodestoneId)
        return XivCharacter(lodestoneId, lodestoneData.character.name, null, null)
    }
}