package khitiara.ffxiv.botflox.gear

import org.springframework.data.jpa.repository.JpaRepository
import java.util.*

interface BisListingRepo : JpaRepository<BisListing, UUID>

interface BisListingEntryRepo : JpaRepository<BisListingEntry, UUID>