package khitiara.ffxiv.botflox

import khitiara.ffxiv.botflox.gear.BisListing
import java.util.*
import javax.persistence.*
import kotlin.collections.HashSet

@Entity
@Table(name = "statics")
class Static(
    @Id @GeneratedValue var id: UUID? = null,
    var name: String,
    var slug: String = name.toSlug(),
    @OneToMany(mappedBy = "static", targetEntity = XivCharacter::class)
    var characters: Set<XivCharacter> = HashSet(),
    @OneToMany(mappedBy = "static", targetEntity = BisListing::class)
    var bisListings: Set<BisListing> = HashSet(),
    var fflogs: Int
)

@Entity
@Table(name = "characters")
class XivCharacter(
    @Id var lodestoneId: Long,
    var name: String,
    @ManyToOne @JoinColumn(name = "static_id") var static: Static?,
    var discordUser: Long?,
    @OneToMany(mappedBy = "character", targetEntity = BisListing::class)
    var bisListings: Set<BisListing> = HashSet()
)