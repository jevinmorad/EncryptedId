using Dapper;
using System.Data;

namespace EncryptedId.Data;

public class EncryptedIdHandler : SqlMapper.TypeHandler<EncryptedId>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedId? value)
    {
        parameter.Value = value.Value;
    }

    public override EncryptedId? Parse(object value)
    {
        return new EncryptedId(Convert.ToInt32(value));
    }
}

public class NullableEncryptedIdHandler : SqlMapper.TypeHandler<EncryptedId?>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedId? value)
    {
        parameter.Value = value.HasValue ? value.Value : DBNull.Value;
    }

    public override EncryptedId? Parse(object value)
    {
        if (value == null || value == DBNull.Value) return null;
        return new EncryptedId(Convert.ToInt32(value));
    }
}
