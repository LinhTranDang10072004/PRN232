using ExpenseManagementAPI.DTOs.Auth;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<(AuthResponse? Response, string? Error)> RegisterUserAsync(RegisterRequest request);
    }
}
