package khitiara.ffxiv.botflox.controller

import org.springframework.stereotype.Controller
import org.springframework.ui.Model
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.RequestMapping

@Controller
@RequestMapping("/gear")
class GearController {
    @GetMapping("/build-bis/{static}/{character}")
    suspend fun buildBis(model: Model, @PathVariable static: String, @PathVariable character: Long): String {
        model.addAttribute("static", static)
            .addAttribute("character", character)
        return "build-bis"
    }
}