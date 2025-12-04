using Dapper;
using System.Data;
using System.Globalization;

namespace EncryptedId.Data;

/// <summary>
/// Dapper type handler for nullable <see cref="EncryptedId"/> (EncryptedId?).
/// </summary>
public class NullableEncryptedIdHandler : SqlMapper.TypeHandler<EncryptedId?>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedId? value)
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

    public override EncryptedId? Parse(object value)
    {
        if (value is null || value is DBNull)
        {
            return null;
        }

        var intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return new EncryptedId(intValue);
    }
}
