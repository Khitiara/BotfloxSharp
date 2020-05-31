package khitiara.ffxiv.gearmanagement

import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.scheduling.annotation.Async
import java.util.*

interface StaticRepository : JpaRepository<Static, UUID> {
    @Async
    suspend fun findBySlug(slug: String): Static?

}

interface CharacterRepository : JpaRepository<XivCharacter, Long> {
    @Async
    suspend fun findByLodestoneId(lodestoneId: Long): XivCharacter?
}