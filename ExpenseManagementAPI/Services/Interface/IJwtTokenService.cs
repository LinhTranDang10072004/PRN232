using BusinessObjects.Models;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
