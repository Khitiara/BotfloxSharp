package khitiara.ffxiv.gearmanagement.controller

import khitiara.ffxiv.gearmanagement.service.BotfloxAlltalks
import org.springframework.stereotype.Controller
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.ResponseBody
import org.webjars.RequireJS


@Controller
class BotfloxController(private val botfloxAlltalks: BotfloxAlltalks) {
    @ResponseBody
    @RequestMapping(value = ["/webjarsjs"], produces = ["application/javascript"])
    fun webjarjs(): String? {
        return RequireJS.getSetupJavaScript("/webjars/")
    }

    @GetMapping("/invite")
    fun inviteLink(): String = "redirect:" + botfloxAlltalks.inviteUrl()

    @GetMapping("/botflox")
    fun botfloxHome(): String = "botflox"
}