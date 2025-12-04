using System;
using System.Text;
using EncryptedId.Configuration;

namespace EncryptedId.Core;

/// <summary>
/// Internal engine responsible for encoding and decoding integer IDs
/// using a salt-driven shuffled alphabet and base-N conversion.
/// This is intended for ID obfuscation, not strong cryptography.
/// </summary>
internal static class HashEngine
{
    private static string? _shuffledAlphabet;
    private static string? _cachedSalt;
    private static string? _cachedAlphabet;
    private static readonly object _lock = new();

    /// <summary>
    /// Small helper used in the shuffle step to add non-trivial mixing based on the index.
    /// </summary>
    private static int MixIndex(int i) => (i * 7) + 3;

    /// <summary>
    /// Generates and returns a shuffled version of the configured alphabet using the current salt value.
    /// The result is cached and recomputed only when either the salt or the alphabet changes.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the salt is not configured or the alphabet is invalid.
    /// </exception>
    private static string GetShuffledAlphabet()
    {
        if (_shuffledAlphabet != null &&
            _cachedSalt == EncryptedConfig.Salt &&
            _cachedAlphabet == EncryptedConfig.Alphabet)
        {
            return _shuffledAlphabet;
        }

        lock (_lock)
        {
            var salt = EncryptedConfig.Salt;
            var sourceAlphabet = EncryptedConfig.Alphabet;

            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new InvalidOperationException(
                    "EncryptedConfig.Salt must be configured before using EncryptedId.");
            }

            if (string.IsNullOrWhiteSpace(sourceAlphabet) || sourceAlphabet.Length < 16)
            {
                throw new InvalidOperationException(
                    "EncryptedConfig.Alphabet must be at least 16 characters long.");
            }

            var chars = sourceAlphabet.ToCharArray();
            var saltLength = salt.Length;
            var inputLength = chars.Length;

            // Deterministic salt-based shuffle
            for (var i = 0; i < inputLength; i++)
            {
                var saltChar = salt[i % saltLength];
                var swapIndex = (saltChar + i + MixIndex(i)) % inputLength;
                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }

            _cachedSalt = salt;
            _cachedAlphabet = sourceAlphabet;
            _shuffledAlphabet = new string(chars);

            return _shuffledAlphabet;
        }
    }

    /// <summary>
    /// Encodes a non-negative integer identifier into a string using the configured salt and alphabet.
    /// </summary>
    /// <param name="id">The integer identifier to encode. Must be non-negative.</param>
    /// <returns>
    /// A string representation of the encoded identifier using the configured alphabet. If <paramref name="id"/> is 0,
    /// returns a single-character string. If <see cref="EncryptedConfig.MinLength"/> is set, the result is left-padded
    /// to that length.
    /// </returns>
    public static string Encode(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be non-negative.");
        }

        var alphabet = GetShuffledAlphabet();

        // Special case for zero
        if (id == 0)
        {
            var single = alphabet[0].ToString();
            return PadToMinLength(single, alphabet);
        }

        var sb = new StringBuilder();
        var baseLen = alphabet.Length;
        var value = id;

        // Standard base-N encoding using the shuffled alphabet
        while (value > 0)
        {
            var rem = value % baseLen;
            sb.Insert(0, alphabet[rem]);
            value /= baseLen;
        }

        var encoded = sb.ToString();
        return PadToMinLength(encoded, alphabet);
    }

    /// <summary>
    /// Decodes a string that was previously encoded using <see cref="Encode(int)"/> back into the original integer value.
    /// </summary>
    /// <param name="encoded">Encoded string representation of the integer.</param>
    /// <returns>
    /// The decoded integer value, or <c>null</c> if the input is invalid, contains unknown characters,
    /// or an overflow occurs during decoding.
    /// </returns>
    public static int? Decode(string? encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
        {
            return null;
        }

        var alphabet = GetShuffledAlphabet();
        var baseLen = alphabet.Length;
        var result = 0;

        foreach (var c in encoded)
        {
            var index = alphabet.IndexOf(c);
            if (index == -1)
            {
                // Character not present in the current alphabet => invalid token
                return null;
            }

            try
            {
                checked
                {
                    result = (result * baseLen) + index;
                }
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        return result;
    }

    private static string PadToMinLength(string value, string alphabet)
    {
        var minLength = EncryptedConfig.MinLength;
        if (minLength <= 0 || value.Length >= minLength)
        {
            return value;
        }

        var padChar = alphabet[0];
        var padCount = minLength - value.Length;
        return new string(padChar, padCount) + value;
    }
}
