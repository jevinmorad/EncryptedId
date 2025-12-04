using EncryptedId.Core;
using System.ComponentModel;
using System.Globalization;

namespace EncryptedId.Converters;

/// <summary>
/// TypeConverter allowing EncryptedId to be used with model binding, query string parsing, etc.
/// It expects an encoded string and decodes it back to an <see cref="EncryptedId"/>.
/// </summary>
public class EncryptedIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var decoded = HashEngine.Decode(text);
            if (!decoded.HasValue)
            {
                throw new FormatException("Invalid encoded EncryptedId value.");
            }

            return new EncryptedId(decoded.Value);
        }

        return base.ConvertFrom(context, culture, value);
    }
}
