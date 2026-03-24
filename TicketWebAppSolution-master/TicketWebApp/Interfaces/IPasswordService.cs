namespace TicketWebApp.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(string password, byte[]? salt, out byte[] key);
        bool VerifyPassword(string password, string hashed, byte[] key);

    }
}
