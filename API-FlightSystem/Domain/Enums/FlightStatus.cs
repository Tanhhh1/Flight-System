using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlightStatus
    {
        Active = 1,
        Inactive = 0,
        Delayed = 2,
        Cancelled = 3
    }
}
