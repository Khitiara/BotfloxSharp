package khitiara.ffxiv.botflox

import org.springframework.data.jpa.repository.JpaRepository
import java.util.*

interface StaticRepository : JpaRepository<Static, UUID> {
    fun findBySlug(slug: String): Static?

}

interface CharacterRepository : JpaRepository<XivCharacter, Long> {
    fun findByLodestoneId(lodestoneId: Long): XivCharacter?

    fun findByDiscordUser(discordUser: Long): XivCharacter?
}