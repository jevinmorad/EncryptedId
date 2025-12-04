using Dapper;
using System.Data;
using System.Globalization;

namespace EncryptedId.Data;

/// <summary>
/// Dapper type handler for non-nullable <see cref="EncryptedId"/>.
/// Stores the underlying <see cref="EncryptedId.Value"/> as an INT in the database.
/// </summary>
public class EncryptedIdHandler : SqlMapper.TypeHandler<EncryptedId>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedId value)
    {
        parameter.Value = value.Value;
    }

    public override EncryptedId Parse(object value)
    {
        if (value is null || value is DBNull)
        {
            throw new DataException("Cannot parse null database value into EncryptedId.");
        }

        var intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return new EncryptedId(intValue);
    }
}