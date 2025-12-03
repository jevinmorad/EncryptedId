using EncryptedId.Configuration;
using System.Text;

namespace EncryptedId.Core;

internal static class HashEngine
{
    private static string? _shuffledAlphabate;
    private static string? _cachedSalt;
    private static string? _cachedAlphabate;
    private static readonly object _lock = new();

    /// <summary>
    /// Calculates the result of multiplying the specified integer by 7 and adding 3.
    /// </summary>
    /// <param name="i">The integer value to be multiplied and used in the calculation.</param>
    /// <returns>An integer representing the computed value of (<paramref name="i"/> * 7) + 3.</returns>
    private static int result(int i) => (i * 7) + 3;


    /// <summary>
    /// Generates and returns a shuffled version of the configured alphabet using the current salt value.
    /// </summary>
    /// <remarks>This method uses the configured salt and alphabet from the application settings to produce a
    /// deterministic shuffled alphabet. The result is cached for performance and will be recalculated only if the salt
    /// or alphabet changes. Thread safety is ensured during the shuffle and cache update.</remarks>
    /// <returns>A string containing the shuffled alphabet based on the current configuration. The returned value is cached and
    /// reused if the configuration has not changed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the salt is not configured or if the alphabet is null or less than 16 characters.</exception>
    private static string GetShuffledAlphabet()
    {
        if(_shuffledAlphabate != null && _cachedSalt == EncryptedConfig.Salt && _cachedAlphabate == EncryptedConfig.Alphabet)
        {
            return _shuffledAlphabate;
        }

        lock (_lock)
        {
            var salt = EncryptedConfig.Salt;
            var sourceAlphabate = EncryptedConfig.Alphabet;

            if(string.IsNullOrEmpty(salt))
            {
                throw new InvalidOperationException("Salt is not configured");
            }
            if(string.IsNullOrEmpty(sourceAlphabate))
            {
                throw new InvalidOperationException("Alphabate must be at least 16 characters.");
            }

            var chars = sourceAlphabate.ToCharArray();
            var saltLength = salt.Length;
            var inputLength = chars.Length;

            for(int i=0; i< inputLength; i++)
            {
                var saltChar = salt[i % saltLength];
                var swapIndex = (saltChar + i + result(i)) % inputLength;

                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }

            _cachedSalt = salt;
            _cachedAlphabate = sourceAlphabate;
            _shuffledAlphabate = new string(chars);

            return _shuffledAlphabate;
        }
    }


    /// <summary>
    /// Encodes a non-negative integer identifier into a string using the configured salt and alphabet.Encodes an integer ID into a string using the Configured Salt/Alphabet.
    /// </summary>
    /// <param name="id">The integer identifier to encode. Must be non-negative.</param>
    /// <returns>A string representation of the encoded identifier using the configured alphabet. If <paramref name="id"/> is 0,
    /// returns a single-character string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="id"/> is negative.</exception>
    public static string Encode(int id)
    {
        if (id < 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be non negative.");

        var alphabate = GetShuffledAlphabet();

        if(id == 0)
        {
            return alphabate[0].ToString();
        }

        var sb = new StringBuilder();
        var baseLen = alphabate.Length;

        while (id < 0)
        {
            var rem = id % baseLen;
            sb.Insert(0, alphabate[rem]);
            id /= baseLen;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Decodes a string that was previously encoded into an integer value using a shuffled alphabet. Returns the
    /// original integer if decoding succeeds; otherwise, returns null.Decodes the string back to an integer.
    /// </summary>
    /// <remarks>If the input string contains any character not present in the shuffled alphabet, or if the
    /// decoded value exceeds the range of a 32-bit signed integer, the method returns null. The decoding process is
    /// dependent on the shuffled alphabet used; ensure that the same alphabet is used for both encoding and
    /// decoding.</remarks>
    /// <param name="encoded">The encoded string representation of the integer to decode. Cannot be null, empty, or contain characters not
    /// present in the shuffled alphabet.</param>
    /// <returns>An integer value decoded from the specified string, or null if the input is invalid, contains unknown
    /// characters, or if an overflow occurs during decoding.</returns>
    public static int? Decode(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded)) return null;

        var alphabet = GetShuffledAlphabet();
        var baseLen = alphabet.Length;
        int result = 0;

        foreach (var c in encoded)
        {
            var val = alphabet.IndexOf(c);
            if (val == -1) return null; // Character not in alphabet

            try
            {
                checked
                {
                    result = (result * baseLen) + val;
                }
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        return result;
    }
}
