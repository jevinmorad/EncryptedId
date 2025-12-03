using EncryptedId.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EncryptedId.Converters;

public class EncryptedIdJsonConverter : JsonConverter<EncryptedId>
{
    public override EncryptedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read string from JSON (e.g., "x9A2")
        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            var decoded = HashEngine.Decode(text);
            return new EncryptedId(decoded ?? 0);
        }

        if (reader.TokenType == JsonTokenType.Number)
            return new EncryptedId(reader.GetInt32());

        return default;
    }

    public override void Write(Utf8JsonWriter writer, EncryptedId value, JsonSerializerOptions options)
    {
        // Write encoded string to JSON
        var encoded = HashEngine.Encode(value.Value);
        writer.WriteStringValue(encoded);
    }
}
