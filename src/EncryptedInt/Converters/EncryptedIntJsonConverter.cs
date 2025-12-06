using EncryptedInt.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EncryptedInt.Converters;

/// <summary>
/// JSON converter that reads/writes <see cref="global::EncryptedInt"/> values
/// as encoded strings using <see cref="HashEngine"/>.
/// </summary>
public class EncryptedIntJsonConverter : JsonConverter<EncryptedInt>
{
    public override EncryptedInt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                {
                    var text = reader.GetString();
                    var decoded = HashEngine.Decode(text);
                    if (!decoded.HasValue)
                    {
                        throw new JsonException("Invalid encoded EncryptedInt value.");
                    }

                    return new EncryptedInt(decoded.Value);
                }

            case JsonTokenType.Number:
                {
                    var value = reader.GetInt32();
                    return new EncryptedInt(value);
                }

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when parsing EncryptedInt.");
        }
    }

    public override void Write(Utf8JsonWriter writer, EncryptedInt value, JsonSerializerOptions options)
    {
        var encoded = HashEngine.Encode(value.Value);
        writer.WriteStringValue(encoded);
    }
}
