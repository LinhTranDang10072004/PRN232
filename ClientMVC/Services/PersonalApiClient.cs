using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Models.Auth;
using ClientMVC.Models.Personal;

namespace ClientMVC.Services
{
    public class PersonalApiClient : IPersonalApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PersonalApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<(List<ExpenseItem>? Data, string? Error)> GetExpensesAsync(string? odataQuery = null) =>
            GetListAsync<ExpenseItem>($"api/personal/expenses{odataQuery}");

        public async Task<(ExpenseItem? Data, string? Error)> GetExpenseAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/personal/expenses/{id}");
            return await ReadSingleAsync<ExpenseItem>(response);
        }

        public async Task<(ExpenseItem? Data, string? Error)> CreateExpenseAsync(ExpenseFormModel model)
        {
            var response = await SendAsync(HttpMethod.Post, "api/personal/expenses", model);
            return await ReadSingleAsync<ExpenseItem>(response);
        }

        public async Task<(ExpenseItem? Data, string? Error)> UpdateExpenseAsync(int id, ExpenseFormModel model)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/personal/expenses/{id}", model);
            return await ReadSingleAsync<ExpenseItem>(response);
        }

        public async Task<(bool Success, string? Error)> DeleteExpenseAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/personal/expenses/{id}");
            if (response.IsSuccessStatusCode)
                return (true, null);
            return (false, await ReadErrorAsync(response));
        }

        public Task<(List<CategoryItem>? Data, string? Error)> GetCategoriesAsync(string? odataQuery = null) =>
            GetListAsync<CategoryItem>($"api/personal/categories{odataQuery}");

        public async Task<(CategoryItem? Data, string? Error)> GetCategoryAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/personal/categories/{id}");
            return await ReadSingleAsync<CategoryItem>(response);
        }

        public async Task<(CategoryItem? Data, string? Error)> CreateCategoryAsync(CategoryFormModel model)
        {
            var response = await SendAsync(HttpMethod.Post, "api/personal/categories", model);
            return await ReadSingleAsync<CategoryItem>(response);
        }

        public async Task<(CategoryItem? Data, string? Error)> UpdateCategoryAsync(int id, CategoryFormModel model)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/personal/categories/{id}", model);
            return await ReadSingleAsync<CategoryItem>(response);
        }

        public async Task<(bool Success, string? Error)> DeleteCategoryAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/personal/categories/{id}");
            if (response.IsSuccessStatusCode)
                return (true, null);
            return (false, await ReadErrorAsync(response));
        }

        private async Task<(List<T>? Data, string? Error)> GetListAsync<T>(string url)
        {
            var response = await SendAsync(HttpMethod.Get, url);
            if (!response.IsSuccessStatusCode)
                return (null, await ReadErrorAsync(response));

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var list = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
                    return (list, null);
                }

                if (doc.RootElement.TryGetProperty("value", out var value))
                {
                    var list = JsonSerializer.Deserialize<List<T>>(value.GetRawText(), JsonOptions);
                    return (list, null);
                }
            }
            catch
            {
                return (null, "Không đọc được dữ liệu từ API.");
            }

            return (new List<T>(), null);
        }

        private static async Task<(T? Data, string? Error)> ReadSingleAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                return (data, null);
            }
            return (default, await ReadErrorAsync(response));
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("jwt")?.Value;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.");

            using var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body != null)
                request.Content = JsonContent.Create(body);

            return await _httpClient.SendAsync(request);
        }

        private static async Task<string?> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                if (!string.IsNullOrWhiteSpace(error?.Message))
                    return error.Message;
            }
            catch
            {
                // ignore
            }

            return $"Lỗi API ({(int)response.StatusCode})";
        }
    }
}
