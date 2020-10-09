using System.Text.Json.Serialization;

namespace XivApi.Status
{
    public class StatusDataCenter    {
        [JsonPropertyName("name")]
        public string Name { get; set; } 

        [JsonPropertyName("region")]
        public string Region { get; set; } 
    }
}