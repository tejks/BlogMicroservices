namespace Core.Services
{
    public interface IPasswordService
    {
        string HashPasword(string password, out byte[] salt);
        bool VerifyPassword(string password, string hash, byte[] salt);
    }
}
