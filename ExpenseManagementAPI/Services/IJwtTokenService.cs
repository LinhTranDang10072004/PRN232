using BusinessObjects.Models;

namespace ExpenseManagementAPI.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(AppUser user);
    }
}
