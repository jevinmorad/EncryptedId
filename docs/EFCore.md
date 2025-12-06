# EF Core Integration

EF Core uses ValueConverters for mapping types to database columns.

EncryptedInt includes:

- `EncryptedIntValueConverter`
- `NullableEncryptedIntValueConverter`

## Setup

Configure converters in `OnModelCreating`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .Property(x => x.Id)
        .HasConversion(new EncryptedIntValueConverter());
}
```