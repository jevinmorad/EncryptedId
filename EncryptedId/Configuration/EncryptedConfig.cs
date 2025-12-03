namespace EncryptedId.Configuration;

public class EncryptedConfig
{
    public static string Salt { get; set; } = string.Empty;

    public static string Alphabet { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ012345789";

    public static int MinLength { get; set; } = 0;
}
