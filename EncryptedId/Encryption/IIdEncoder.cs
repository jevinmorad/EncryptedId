namespace EncryptedId.Encryption;

public interface IIdEncoder
{
    string Encode(int id);
    int Decoded(string encoded);
}
