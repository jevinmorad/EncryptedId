using Dapper;
using System.Data;
using System.Globalization;

namespace EncryptedInt.Data;

/// <summary>
/// Dapper type handler for nullable <see cref="EncryptedInt"/> (EncryptedId?).
/// </summary>
public class NullableEncryptedIdHandler : SqlMapper.TypeHandler<EncryptedInt?>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedInt? value)
    {
        if (!value.HasValue)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            parameter.Value = value.Value.Value;
        }
    }

    public override EncryptedInt? Parse(object value)
    {
        if (value is null || value is DBNull)
        {
            return null;
        }

        var intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return new EncryptedInt(intValue);
    }
}
