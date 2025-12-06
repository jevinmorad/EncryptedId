using EncryptedInt.Configuration;
using EncryptedInt.Core;
using Xunit;


namespace EncryptedInt.Test;

public class HashEngineTests
{
    public HashEngineTests()
    {
        EncryptedConfig.Salt = "TestSalt123!";
        EncryptedConfig.Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        EncryptedConfig.MinLength = 6;
    }

    [Fact]
    public void EncodeDecode_ShouldReturnOriginalValue()
    {
        for (int i = 0; i < 10000; i += 123)
        {
            var encoded = HashEngine.Encode(i);
            var decoded = HashEngine.Decode(encoded);

            Assert.Equal(i, decoded.Value);
        }
    }

    [Fact]
    public void Encode_ShouldProduceDifferentOutput_WhenSaltChanges()
    {
        var a = HashEngine.Encode(123);

        EncryptedConfig.Salt = "AnotherSalt!!";
        var b = HashEngine.Encode(123);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Decode_ShouldReturnNull_ForInvalidChars()
    {
        Assert.Null(HashEngine.Decode("!@#$%^"));
    }

    [Fact]
    public void Encode_ShouldHonorMinLength()
    {
        EncryptedConfig.MinLength = 10;
        var encoded = HashEngine.Encode(1);

        Assert.True(encoded.Length >= 10);
    }
}