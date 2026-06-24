using ClientMVC.Models;

namespace ClientMVC.Services
{
    public interface IAuthApiClient
    {
        Task<(bool Success, AuthResult? Data, string? Error)> LoginAsync(LoginViewModel model);
        Task<(bool Success, AuthResult? Data, string? Error)> RegisterAsync(RegisterViewModel model);
    }
}
