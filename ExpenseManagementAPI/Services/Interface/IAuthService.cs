using ExpenseManagementAPI.DTOs.Auth;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAuthService
    {
        Task<(bool Success, AuthResponse? Data, string? Error)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, AuthResponse? Data, string? Error)> LoginAsync(LoginRequest request);
        Task<(bool Success, UserProfileResponse? Data, string? Error)> GetProfileAsync(int userId);
        Task<(bool Success, UserProfileResponse? Data, string? Error)> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
