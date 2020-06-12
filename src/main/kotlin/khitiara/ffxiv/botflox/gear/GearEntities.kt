package khitiara.ffxiv.botflox.gear

import com.vladmihalcea.hibernate.type.basic.PostgreSQLEnumType
import khitiara.ffxiv.botflox.Static
import khitiara.ffxiv.botflox.XivCharacter
import org.hibernate.annotations.Type
import org.hibernate.annotations.TypeDef
import java.util.*
import javax.persistence.*

enum class GearType {
    RAID,
    TOME,
    OTHER,

    /** Only applies to offhand */
    EMPTY
}

enum class GearSlot {
    MAIN_HAND,
    OFF_HAND,
    HEAD,
    CHEST,
    ARMS,
    BELT,
    LEGS,
    FEET,
    EAR,
    NECK,
    WRIST,
    RINGL,
    RINGR
}

@Entity
@Table(name = "bis_listing")
class BisListing(
    @Id @GeneratedValue var id: UUID? = null,
    @ManyToOne @JoinColumn(name = "static_id") var static: Static,
    @ManyToOne @JoinColumn(name = "character_id") var character: XivCharacter,
    var infoUrl: String?,
    @OneToMany(mappedBy = "listing", targetEntity = BisListingEntry::class)
    var entries: Set<BisListingEntry> = HashSet()
)

@Entity
@Table(name = "bis_listing_entry")
@TypeDef(name = "pgsql_enum", typeClass = PostgreSQLEnumType::class)
class BisListingEntry(
    @Id @GeneratedValue var id: UUID? = null,
    @ManyToOne @JoinColumn(name = "listing_id") var listing: BisListing,
    @Enumerated(EnumType.STRING) @Column(columnDefinition = "gear_slot") @Type(type = "pgsql_enum")
    var slot: GearSlot,
    @Enumerated(EnumType.STRING) @Column(columnDefinition = "bis_gear_type") @Type(type = "pgsql_enum")
    var gearType: GearType,
    var obtained: Boolean
)