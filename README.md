
# Project Title

A brief description of what this project does and who it's for

# 🔐 EncryptedId

A **lightweight, fast, reversible ID obfuscation library for .NET**.

EncryptedId allows you to safely expose IDs (integers) without revealing your real ID.

It converts **integer IDs** into short, encoded strings before sending them to the client and decodes them back into integers when received.

## ✨ Why Use EncryptedId?

This library is perfect for:
- **API Responses:** Safely exposing internal IDs in JSON.
- **Preventing ID Enumeration:** Stops attackers from scraping your data by simply changing the ID from `1` to `2` to `3`.
- **Controlled Reversible Mapping:** A consistent, two-way mapping between the integer and the encoded string.

## 🚀 Installation

Install the package via the .NET CLI:

```c#
dotnet add package EncryptedId
```

## Configuration
Configure the library once at application startup.
```c#
using EncryptedId.Configuration;

EncryptedConfig.Salt = builder.Configuration["EncryptedId:Salt"];
EncryptedConfig.Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
EncryptedConfig.MinLength = 6; // optional padding
```

## 🔑 Salt
- Must be secret
- Must be long and random
- Must never change once in production

Store it in
```json
{
  "EncryptedId": {
    "Salt": "SuperSecret_Long_Random_Value_Here"
  }
}

```

## 🌐 API / ASP.NET Core Integration
### Register the JSON converter:
```c#
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
{
    opts.SerializerOptions.Converters.Add(new EncryptedIdJsonConverter());
});
```

### Use in controller models:
```c#
public record UserDto(EncryptedId Id, string Name);
```

API response:
```json
{
  "id": "Xkd02p",
  "name": "John Doe"
}
```
Controller receives decoded ID automatically.

## 🎛 MVC & Model Binding

`EncryptedId` supports model binding through `EncryptedIdTypeConverter`.

Works automatically with:
```bash
GET /users/Ab39Fp
```

Controller:
```c#
[HttpGet("{id}")]
public IActionResult GetUser(EncryptedId id)
{
    int internalId = id.Value;
    ...
}
```

### 🧪 Error Handling

If an invalid encoded ID is received:
- JSON converter throws `JsonException`
- TypeConverter throws `FormatException`
- Decode returns `null`

This prevents:
- tampering
- invalid requests
- malformed IDs
- injection attacks

## ❓ FAQ

### Why not use GUIDs?
They're long, ugly, and user-unfriendly.

### Why not use encryption instead?
Overkill.
APIs rarely require cryptographic secrecy for primary keys.

### Can someone reverse the encoding?
Not without:
- your salt
- your shuffled alphabet
- your internal algorithm

Which is realistically non-practical.

### Can I rotate salt?
Not without invalidating previous encoded IDs.
