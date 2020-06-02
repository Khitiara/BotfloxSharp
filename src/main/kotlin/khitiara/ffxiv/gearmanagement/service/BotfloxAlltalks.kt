package khitiara.ffxiv.gearmanagement.service

import club.minnced.jda.reactor.ReactiveEventManager
import khitiara.ffxiv.gearmanagement.CharacterRepository
import khitiara.ffxiv.gearmanagement.MainConfiguration
import khitiara.ffxiv.gearmanagement.xivapi.XIVApi
import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.JDABuilder
import net.dv8tion.jda.api.entities.Activity
import net.dv8tion.jda.api.events.GenericEvent
import org.springframework.stereotype.Service
import reactor.core.publisher.Flux
import kotlin.reflect.KClass

@Service
class BotfloxAlltalks(private val xivApi: XIVApi,
                      private val characterRepository: CharacterRepository,
                      private val conf: MainConfiguration) {
    private val eventMgr = ReactiveEventManager()
    private val bot: JDA = JDABuilder.createDefault(conf.botToken).setEventManager(eventMgr)
        .setActivity(Activity.playing("with many uplander friends!"))
        .build()

    fun <T : GenericEvent> on(clazz: KClass<T>): Flux<T> = eventMgr.on(clazz.java)

    fun inviteUrl() = bot.getInviteUrl()
}