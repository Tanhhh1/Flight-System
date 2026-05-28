using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlightStatus
    {
        Inactive = 0,
        Active = 1,
        Suspended = 2,
        Completed = 3,
        Delayed = 4,
        Cancelled = 5
    }
}
