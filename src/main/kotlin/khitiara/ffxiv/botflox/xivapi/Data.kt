package khitiara.ffxiv.botflox.xivapi

data class ClassJobLevel(val classJob: ClassJobs.ClassJob, val level: Int?, val specialist: Boolean?)

class ClassJobLevelData(raw: List<XivCharacterResponse.ClassJobInfo>) {
    private val baseMap: Map<ClassJobs.ClassJob, ClassJobLevel> = raw.map {
        val job: ClassJobs.ClassJob = (it.unlockedState.id ?: it.clazz.id)?.let { it1 -> ClassJobs.byId(it1) }!!
        job to ClassJobLevel(job, if (it.level == 0) null else it.level,
            if (job is ClassJobs.HandClass) it.specialist else null)
    }.toMap()

    fun get(id: Int): ClassJobLevel? = ClassJobs.byId(id)?.let { get(it) }

    fun get(classJob: ClassJobs.ClassJob): ClassJobLevel? = baseMap[classJob]
        ?: (classJob as? ClassJobs.CombatJob)?.clazz?.let { baseMap[it] }
}

data class CharacterInfo(val lodestone: Int, val name: String, val avatarUri: String, val portraitUri: String,
                         val cjInfo: ClassJobLevelData, val dc: String, val server: String, val title: String?,
                         val titleTop: Boolean?, val tribe: String, val race: String, val nameday: String,
                         val guardian: String, val gcRank: String?, val fcName: String?, 
                         val fcTag: String?) {
    constructor(response: XivCharacterResponse) : this(
        response.character.id, 
        response.character.name,
        response.character.avatar, 
        response.character.portrait, 
        ClassJobLevelData(response.character.classJobs),
        response.character.dataCenter, 
        response.character.server, 
        response.character.titleInfo?.name,
        response.character.titleTop, 
        response.character.tribe.name, 
        response.character.race.name,
        response.character.nameday,
        response.character.guardian.name,
        response.character.grandCompany?.rank?.name,
        response.freeCompany?.name, 
        response.freeCompany?.tag)
}