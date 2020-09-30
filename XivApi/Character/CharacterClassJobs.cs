using System.Collections.Generic;
using System.Collections.ObjectModel;
using XivApi.Character.Raw;

namespace XivApi.Character
{
    public class CharacterClassJobs
    {
        public readonly struct ClassJobLevel
        {
            public IClassJob ClassJob { get; }
            public int? Level { get; }
            public bool? Specialist { get; }

            public ClassJobLevel(IClassJob classJob, int? level, bool? specialist) {
                ClassJob = classJob;
                Level = level;
                Specialist = specialist;
            }
        }

        private class JobLevelCollection : KeyedCollection<IClassJob, ClassJobLevel>
        {
            protected override IClassJob GetKeyForItem(ClassJobLevel item) => item.ClassJob;
        }

        private readonly JobLevelCollection _jobLevels = new JobLevelCollection();

        public CharacterClassJobs(IEnumerable<ClassJobInfo> classJobInfos) {
            foreach (ClassJobInfo info in classJobInfos) {
                IClassJob classJob = ClassJobs.ById[info.UnlockedState.ID ?? info.Class.ID]!;
                int? lvl = null;
                if (info.Level != 0)
                    lvl = info.Level;
                bool? specialist = null;
                if (classJob is HandClass)
                    specialist = info.IsSpecialised;
                _jobLevels.Add(new ClassJobLevel(classJob, lvl, specialist));
            }
        }

        public bool TryGetLevel(IClassJob classJob, out ClassJobLevel level) {
            if (_jobLevels.TryGetValue(classJob, out level)) return true;
            if (classJob is CombatJob job && job.Class != null) {
                return _jobLevels.TryGetValue(job.Class, out level);
            }

            return false;
        }

        public ClassJobLevel? this[int id] => this[ClassJobs.ById[id]];

        public ClassJobLevel? this[IClassJob classJob] {
            get {
                if (!TryGetLevel(classJob, out ClassJobLevel level)) return null;
                return level;
            }
        }
    }
}