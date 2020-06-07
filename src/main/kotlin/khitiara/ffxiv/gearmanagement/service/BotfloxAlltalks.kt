package khitiara.ffxiv.gearmanagement.service

import br.com.devsrsouza.jda.command.command
import br.com.devsrsouza.jda.command.commands
import br.com.devsrsouza.jda.command.fail
import club.minnced.jda.reactor.ReactiveEventManager
import khitiara.ffxiv.gearmanagement.CharacterRepository
import khitiara.ffxiv.gearmanagement.MainConfiguration
import khitiara.ffxiv.gearmanagement.StaticRepository
import khitiara.ffxiv.gearmanagement.await
import khitiara.ffxiv.gearmanagement.xivapi.XIVApi
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.JDABuilder
import net.dv8tion.jda.api.Permission
import net.dv8tion.jda.api.entities.Activity
import net.dv8tion.jda.api.events.GenericEvent
import org.slf4j.LoggerFactory
import org.springframework.stereotype.Service
import reactor.core.publisher.Flux
import kotlin.reflect.KClass

@Service
class BotfloxAlltalks(private val xivApi: XIVApi,
                      private val characterRepository: CharacterRepository,
                      private val staticRepository: StaticRepository,
                      private val conf: MainConfiguration,
                      private val dbService: DatabaseService) {
    private val eventMgr = ReactiveEventManager()
    private val bot: JDA = JDABuilder.createDefault(conf.botToken).setEventManager(eventMgr)
        .setActivity(Activity.playing("with uplander friends!"))
        .build()
    private val log = LoggerFactory.getLogger("Botflox")

    final fun <T : GenericEvent> on(clazz: KClass<T>): Flux<T> = eventMgr.on(clazz.java)

    fun inviteUrl() = bot.getInviteUrl(Permission.MESSAGE_WRITE, Permission.MESSAGE_ATTACH_FILES,
        Permission.MESSAGE_ADD_REACTION)

    init {
        bot.commands("?") {
            command("whoami") {
                channel.sendTyping().await()
                withContext(Dispatchers.IO) { characterRepository.findByDiscordUser(member.idLong) }?.run {
                    channel.sendMessage("You are $name").await()
                } ?: run {
                    channel.sendMessage("You are not known to me").await()
                }
            }
            command("iam") {
                channel.sendTyping().await()
                when (args.size) {
                    1 -> args[0].toLongOrNull()?.let {
                        withContext(Dispatchers.IO) { characterRepository.findByLodestoneId(it) }?.also { record ->
                            if (record.discordUser != null && record.discordUser != member.idLong) fail {
                                channel.sendMessage("That character is already claimed by another discord user!")
                                    .await()
                            }
                            record.discordUser = member.idLong
                            channel.sendMessage("You have been successfully associated with ${record.name}, ${member.asMention}")
                                .await()
                        } ?: run {
                            val char = dbService.createCharacterFromLodestone(it)
                            withContext(Dispatchers.IO) { characterRepository.findByDiscordUser(member.idLong) }?.let { record ->
                                record.discordUser = null
                                withContext(Dispatchers.IO) { characterRepository.save(record) }
                            }
                            char.discordUser = member.idLong
                            log.info("XivCharacter{${char.name}, ${char.discordUser}, ${char.lodestoneId})")
                            withContext(Dispatchers.IO) { characterRepository.save(char) }
                            channel.sendMessage("You have been successfully associated with ${char.name}, ${member.asMention}")
                                .await()
                        }
                    } ?: fail {
                        channel.sendMessage("That does not appear to be a valid lodestone id!").await()
                    }
                    else -> fail {
                        channel.sendMessage("Unknown how to handle the given number of arguments!").await()
                    }
                }
            }
        }
    }
}