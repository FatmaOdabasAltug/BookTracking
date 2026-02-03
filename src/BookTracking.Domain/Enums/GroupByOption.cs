using System.Text.Json.Serialization;

namespace BookTracking.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupByOption
{
    None,
    EntityId,
    EntityType,
    Date,
    Action
}
