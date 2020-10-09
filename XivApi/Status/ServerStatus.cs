using System.Text.Json.Serialization;

namespace XivApi.Status
{
    public enum Status
    {
        Online,
    }

    public enum Congestion
    {
        Standard,
        Congested,
        Preferred
    }

    public enum CreationAvailability
    {
        Available,
        Unavailable
    }

    public class ServerStatus
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("congestion")]
        public Congestion Congestion { get; set; }

        [JsonPropertyName("creation")]
        public CreationAvailability Creation { get; set; }

        [JsonPropertyName("last_online")]
        public string LastOnline { get; set; } = null!;

        [JsonPropertyName("data_center")]
        public StatusDataCenter? DataCenter { get; set; }
    }
}