using EncryptedId.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EncryptedId.Converters;

/// <summary>
/// JSON converter that reads/writes <see cref="EncryptedId"/> values
/// as encoded strings using <see cref="HashEngine"/>.
/// </summary>
public class EncryptedIdJsonConverter : JsonConverter<EncryptedId>
{
    public override EncryptedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                {
                    var text = reader.GetString();
                    var decoded = HashEngine.Decode(text);
                    if (!decoded.HasValue)
                    {
                        throw new JsonException("Invalid encoded EncryptedId value.");
                    }

                    return new EncryptedId(decoded.Value);
                }

            case JsonTokenType.Number:
                {
                    var value = reader.GetInt32();
                    return new EncryptedId(value);
                }

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when parsing EncryptedId.");
        }
    }

    public override void Write(Utf8JsonWriter writer, EncryptedId value, JsonSerializerOptions options)
    {
        var encoded = HashEngine.Encode(value.Value);
        writer.WriteStringValue(encoded);
    }
}
