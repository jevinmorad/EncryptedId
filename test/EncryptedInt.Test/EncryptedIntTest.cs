using Xunit;

namespace EncryptedInt.test;

public class EncryptedIdTests
{
    [Fact]
    public void ImplicitConversion_ShouldWork()
    {
        EncryptedInt id = 10;
        int value = id;

        Assert.Equal(10, value);
    }

    [Fact]
    public void Equality_ShouldWork()
    {
        EncryptedInt a = new(5);
        EncryptedInt b = new(5);

        Assert.True(a == b);
        Assert.False(a != b);
    }
}