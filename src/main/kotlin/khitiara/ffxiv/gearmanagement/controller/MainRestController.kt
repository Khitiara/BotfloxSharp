package khitiara.ffxiv.gearmanagement.controller

import khitiara.ffxiv.gearmanagement.CharacterRepository
import khitiara.ffxiv.gearmanagement.StaticRepository
import khitiara.ffxiv.gearmanagement.xivapi.XIVApi
import khitiara.ffxiv.gearmanagement.xivapi.XivCharacterResponse
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.RestController

@RestController
class MainRestController(private val characterRepository: CharacterRepository,
                         private val staticRepository: StaticRepository,
                         private val xivApi: XIVApi) {
    @GetMapping("/character/{id}")
    fun characterById(@PathVariable id: Long): XivCharacterResponse = xivApi.loadDataFromLodestone(id)
}