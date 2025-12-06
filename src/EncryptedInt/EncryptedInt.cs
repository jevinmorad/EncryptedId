using EncryptedInt.Converters;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace EncryptedInt;

/// <summary>
/// Strongly typed integer ID used with the EncryptedInt encoding/decoding pipeline.
/// The <see cref="Value"/> is what you persist in the database (INT),
/// while JSON / string conversion uses the configured obfuscated format
/// via <see cref="EncryptedIntJsonConverter"/> and <see cref="EncryptedIntTypeConverter"/>.
/// </summary>
[TypeConverter(typeof(EncryptedIntTypeConverter))]
[JsonConverter(typeof(EncryptedIntJsonConverter))]
public readonly struct EncryptedInt : IEquatable<EncryptedInt>, IComparable<EncryptedInt>
{
    /// <summary>
    /// The underlying integer value. This is the value you should store in your database.
    /// </summary>
    public int Value { get; }

    public EncryptedInt(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Implicit conversion from <see cref="int"/> to <see cref="EncryptedInt"/>.
    /// </summary>
    public static implicit operator EncryptedInt(int value) => new(value);

    /// <summary>
    /// Implicit conversion from <see cref="EncryptedInt"/> to <see cref="int"/>.
    /// </summary>
    public static implicit operator int(EncryptedInt secure) => secure.Value;

    /// <summary>
    /// Returns the underlying integer value as a string (invariant culture).
    /// To get the encoded/obfuscated form, rely on JSON or <see cref="EncryptedIntTypeConverter"/>.
    /// </summary>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    #region Equality / Comparison

    public bool Equals(EncryptedInt other) => Value == other.Value;

    public override bool Equals(object? obj) =>
        obj is EncryptedInt other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator == (EncryptedInt left, EncryptedInt right) => left.Equals(right);

    public static bool operator != (EncryptedInt left, EncryptedInt right) => !left.Equals(right);

    public int CompareTo(EncryptedInt other) => Value.CompareTo(other.Value);

    #endregion
}
