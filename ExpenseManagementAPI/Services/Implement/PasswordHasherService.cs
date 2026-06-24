using ExpenseManagementAPI.Services.Interface;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
