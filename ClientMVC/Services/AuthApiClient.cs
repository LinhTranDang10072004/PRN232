using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Models.Auth;

namespace ClientMVC.Services
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(AuthApiResponse? Data, string? Error)> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                model.Username,
                model.Password
            });

            return await ParseResponseAsync(response);
        }

        public async Task<(AuthApiResponse? Data, string? Error)> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
            {
                model.Username,
                model.Password,
                model.Email,
                model.FullName
            });

            return await ParseResponseAsync(response);
        }

        private static async Task<(AuthApiResponse? Data, string? Error)> ParseResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<AuthApiResponse>();
                return (data, null);
            }

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            if (error?.Message != null)
                return (null, error.Message);

            var raw = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return (null, msg.GetString());
            }
            catch
            {
                // ignore
            }

            return (null, response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? "Tên đăng nhập hoặc mật khẩu không đúng."
                : "Đã xảy ra lỗi. Vui lòng thử lại.");
        }
    }
}
