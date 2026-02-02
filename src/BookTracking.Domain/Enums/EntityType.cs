namespace BookTracking.Domain.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityType
{
    Book,
    Author
}