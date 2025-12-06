# Dapper Integration

Dapper requires type handlers for custom types.  
EncryptedInt provides two handlers:

- `EncryptedIntHandler`
- `NullableEncryptedIntHandler`

## Setup

Register handlers once at startup:

```csharp
SqlMapper.AddTypeHandler(new EncryptedIntHandler());
SqlMapper.AddTypeHandler(new NullableEncryptedIntHandler());
```
