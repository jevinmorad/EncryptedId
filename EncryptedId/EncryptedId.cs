using System.Text.Json.Serialization;

namespace EncryptedId;

public class EncryptedId : IEquatable<EncryptedId>
{
    public int Value { get; }
    public bool HasValue { get; internal set; }

    public EncryptedId(int value) => Value = value;

    public static implicit operator EncryptedId(int value) => new EncryptedId(value);
    public static implicit operator int(EncryptedId secure) => secure.Value;

    public override string ToString() => Value.ToString();

    public override bool Equals(object? obj) => obj is EncryptedId other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public bool Equals(EncryptedId other) => Value == other.Value;
    public static bool operator ==(EncryptedId left, EncryptedId right) => left.Value == right.Value;
    public static bool operator !=(EncryptedId left, EncryptedId right) => left.Value != right.Value;
}
