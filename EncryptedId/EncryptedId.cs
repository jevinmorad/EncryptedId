using EncryptedId.Converters;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace EncryptedId;

/// <summary>
/// Strongly typed integer ID used with the EncryptedId encoding/decoding pipeline.
/// The <see cref="Value"/> is what you persist in the database (INT),
/// while JSON / string conversion uses the configured obfuscated format
/// via <see cref="EncryptedIdJsonConverter"/> and <see cref="EncryptedIdTypeConverter"/>.
/// </summary>
[TypeConverter(typeof(EncryptedIdTypeConverter))]
[JsonConverter(typeof(EncryptedIdJsonConverter))]
public readonly struct EncryptedId : IEquatable<EncryptedId>, IComparable<EncryptedId>
{
    /// <summary>
    /// The underlying integer value. This is the value you should store in your database.
    /// </summary>
    public int Value { get; }

    public EncryptedId(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Implicit conversion from <see cref="int"/> to <see cref="EncryptedId"/>.
    /// </summary>
    public static implicit operator EncryptedId(int value) => new(value);

    /// <summary>
    /// Implicit conversion from <see cref="EncryptedId"/> to <see cref="int"/>.
    /// </summary>
    public static implicit operator int(EncryptedId secure) => secure.Value;

    /// <summary>
    /// Returns the underlying integer value as a string (invariant culture).
    /// To get the encoded/obfuscated form, rely on JSON or <see cref="EncryptedIdTypeConverter"/>.
    /// </summary>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    #region Equality / Comparison

    public bool Equals(EncryptedId other) => Value == other.Value;

    public override bool Equals(object? obj) =>
        obj is EncryptedId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(EncryptedId left, EncryptedId right) => left.Equals(right);

    public static bool operator !=(EncryptedId left, EncryptedId right) => !left.Equals(right);

    public int CompareTo(EncryptedId other) => Value.CompareTo(other.Value);

    #endregion
}
