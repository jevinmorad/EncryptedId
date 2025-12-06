namespace EncryptedInt.Configuration;

/// <summary>
/// Global configuration for EncryptedInt encoding/decoding.
/// You should typically configure these values at application startup.
/// </summary>
public static class EncryptedConfig
{
    /// <summary>
    /// Secret salt used to shuffle the alphabet and influence encoding.
    /// This must be stable across encode/decode calls and should not be publicly exposed.
    /// </summary>
    public static string Salt { get; set; } = string.Empty;

    /// <summary>
    /// Alphabet used for base-N encoding. Must be at least 16 characters and unique enough
    /// to provide a good character space for obfuscated IDs.
    /// </summary>
    public static string Alphabet { get; set; } =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Optional minimum encoded length. When set &gt; 0, encoded IDs will be left-padded
    /// with the first character of the shuffled alphabet until they reach this length.
    /// </summary>
    public static int MinLength { get; set; } = 0;
}