using ClientMVC.Models.Auth;

namespace ClientMVC.Services
{
    public interface IAuthApiClient
    {
        Task<(AuthApiResponse? Data, string? Error)> LoginAsync(LoginViewModel model);
        Task<(AuthApiResponse? Data, string? Error)> RegisterAsync(RegisterViewModel model);
    }
}
