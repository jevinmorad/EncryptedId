using EncryptedInt.Configuration;
using EncryptedInt.Core;
using System.Text.Json;
using Xunit;

namespace EncryptedInt.test;

public class JsonConverterTests
{
    public JsonConverterTests()
    {
        EncryptedConfig.Salt = "JsonSalt123!";
        EncryptedConfig.Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        EncryptedConfig.MinLength = 6;
    }

    public record UserDto(EncryptedInt Id);

    [Fact]
    public void Serialization_ShouldProduceEncodedString()
    {
        var obj = new UserDto(new EncryptedInt(123));
        var json = JsonSerializer.Serialize(obj);

        Assert.Contains("Id", json);
        Assert.DoesNotContain("\"123\"", json);
    }

    [Fact]
    public void Deserialization_ShouldDecodeEncodedString()
    {
        var encoded = HashEngine.Encode(123);
        var json = $"{{\"Id\":\"{encoded}\"}}";

        var obj = JsonSerializer.Deserialize<UserDto>(json);

        Assert.Equal(123, obj!.Id.Value);
    }
}