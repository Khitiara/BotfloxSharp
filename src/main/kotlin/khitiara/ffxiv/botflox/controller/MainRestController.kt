package khitiara.ffxiv.botflox.controller

import khitiara.ffxiv.botflox.CharacterRepository
import khitiara.ffxiv.botflox.StaticRepository
import khitiara.ffxiv.botflox.xivapi.XIVApi
import khitiara.ffxiv.botflox.xivapi.XivCharacterResponse
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.RestController

@RestController
class MainRestController(private val characterRepository: CharacterRepository,
                         private val staticRepository: StaticRepository,
                         private val xivApi: XIVApi) {
    @GetMapping("/character/{id}")
    suspend fun characterById(@PathVariable id: Long): XivCharacterResponse = xivApi.loadDataFromLodestone(id)
}