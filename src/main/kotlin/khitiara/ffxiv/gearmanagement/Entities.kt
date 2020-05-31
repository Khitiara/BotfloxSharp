package khitiara.ffxiv.gearmanagement

import java.util.*
import javax.persistence.*

@Entity
class Static(
    @Id @GeneratedValue var id: UUID? = null,
    var name: String,
    var slug: String = name.toSlug(),
    @OneToMany(mappedBy = "static", targetEntity = XivCharacter::class)
    var characters: Set<XivCharacter>
)

@Entity
class XivCharacter(
    @Id var lodestoneId: Long,
    var name: String,
    @ManyToOne var static: Static?,
    var discordUser: Long?
)