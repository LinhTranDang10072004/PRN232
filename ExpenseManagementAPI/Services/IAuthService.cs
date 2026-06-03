using ExpenseManagementAPI.DTOs.Auth;

namespace ExpenseManagementAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<(AuthResponse? Response, string? Error)> RegisterUserAsync(RegisterRequest request);
    }
}
