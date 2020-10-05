#pragma warning disable 8618
namespace XivApi.Character.Raw
{
    public class ClassJobInfo
    {
        public Class Class { get; set; }
        public int ExpLevel { get; set; }
        public int ExpLevelMax { get; set; }
        public int ExpLevelTogo { get; set; }
        public bool IsSpecialised { get; set; }
        public Class Job { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public UnlockedState UnlockedState { get; set; }
    }
}