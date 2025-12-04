using Dapper;
using System.Data;
using System.Globalization;

namespace EncryptedInt.Data;

/// <summary>
/// Dapper type handler for non-nullable <see cref="EncryptedInt"/>.
/// Stores the underlying <see cref="EncryptedInt.Value"/> as an INT in the database.
/// </summary>
public class EncryptedIdHandler : SqlMapper.TypeHandler<EncryptedInt>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedInt value)
    {
        parameter.Value = value.Value;
    }

    public override EncryptedInt Parse(object value)
    {
        if (value is null || value is DBNull)
        {
            throw new DataException("Cannot parse null database value into EncryptedId.");
        }

        var intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return new EncryptedInt(intValue);
    }
}