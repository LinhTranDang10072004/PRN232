using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Helpers;
using ClientMVC.Models.Staff;

namespace ClientMVC.Services
{
    public class StaffApiClient : IStaffApiClient
    {
        private readonly HttpClient _http;

        public StaffApiClient(HttpClient http) => _http = http;

        public Task<UserProfileDto?> GetProfileAsync() =>
            GetAsync<UserProfileDto>("api/auth/me");

        public async Task<(bool Ok, string? Error)> UpdateProfileAsync(ProfileFormModel model)
        {
            var response = await _http.PutAsJsonAsync("api/auth/profile", new
            {
                email = model.Email,
                fullName = model.FullName
            }, ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public async Task<(bool Ok, string? Error)> ChangePasswordAsync(ChangePasswordFormModel model)
        {
            var response = await _http.PutAsJsonAsync("api/auth/change-password", new
            {
                currentPassword = model.CurrentPassword,
                newPassword = model.NewPassword
            }, ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public Task<StaffDashboardDto?> GetDashboardAsync() =>
            GetAsync<StaffDashboardDto>("api/staff/expenses/dashboard");

        public Task<List<StaffExpenseDto>> GetExpensesAsync(string? oDataFilter = null, string? oDataOrderBy = null)
        {
            var url = "api/staff/expenses" + ODataExpenseFilterBuilder.BuildQueryString(oDataFilter, oDataOrderBy ?? "ExpenseDate desc");
            return GetODataListAsync<StaffExpenseDto>(url);
        }

        public Task<StaffExpenseDto?> GetExpenseAsync(int id) =>
            GetAsync<StaffExpenseDto>($"api/staff/expenses/{id}");

        public async Task<(bool Ok, StaffExpenseDto? Data, string? Error)> CreateExpenseAsync(StaffExpenseFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/staff/expenses", MapExpense(model), ApiJsonOptions.Default);
            return await ParseDataAsync<StaffExpenseDto>(response);
        }

        public async Task<(bool Ok, StaffExpenseDto? Data, string? Error)> UpdateExpenseAsync(int id, StaffExpenseFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/staff/expenses/{id}", MapExpense(model), ApiJsonOptions.Default);
            return await ParseDataAsync<StaffExpenseDto>(response);
        }

        public Task<(bool Ok, string? Error)> DeleteExpenseAsync(int id) =>
            DeleteAsync($"api/staff/expenses/{id}");

        public Task<List<StaffCategoryDto>> GetCategoriesAsync() =>
            GetODataListAsync<StaffCategoryDto>("api/staff/categories");

        public Task<List<StaffAccountDto>> GetAccountsAsync() =>
            GetODataListAsync<StaffAccountDto>("api/staff/accounts");

        public Task<List<NotificationDto>> GetNotificationsAsync() =>
            GetODataListAsync<NotificationDto>("api/staff/notifications");

        public async Task<int> GetUnreadCountAsync()
        {
            var result = await GetAsync<UnreadCountDto>("api/staff/notifications/unread-count");
            return result?.Count ?? 0;
        }

        public async Task MarkNotificationReadAsync(int id)
        {
            await _http.PutAsync($"api/staff/notifications/{id}/read", null);
        }

        public async Task MarkAllNotificationsReadAsync()
        {
            await _http.PutAsync("api/staff/notifications/read-all", null);
        }

        private static object MapExpense(StaffExpenseFormModel model) => new
        {
            title = model.Title,
            description = model.Description,
            amount = model.Amount,
            expenseDate = model.ExpenseDate,
            categoryId = model.CategoryId,
            accountId = model.AccountId
        };

        private async Task<T?> GetAsync<T>(string url) where T : class
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default);
        }

        private async Task<List<T>> GetODataListAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<T>();

            var body = await response.Content.ReadAsStringAsync();
            if (body.TrimStart().StartsWith('['))
                return JsonSerializer.Deserialize<List<T>>(body, ApiJsonOptions.Default) ?? new List<T>();

            var odata = JsonSerializer.Deserialize<ODataList<T>>(body, ApiJsonOptions.Default);
            return odata?.Value ?? new List<T>();
        }

        private static async Task<(bool Ok, T? Data, string? Error)> ParseDataAsync<T>(HttpResponseMessage response)
            where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default);
                return (true, data, null);
            }
            return (false, null, await ReadErrorAsync(response));
        }

        private static async Task<(bool Ok, string? Error)> ParseMessageAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return (true, null);
            return (false, await ReadErrorAsync(response));
        }

        private async Task<(bool Ok, string? Error)> DeleteAsync(string url)
        {
            var response = await _http.DeleteAsync(url);
            return await ParseMessageAsync(response);
        }

        private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return msg.GetString() ?? "Yêu cầu thất bại.";
            }
            catch { /* ignore */ }
            return "Yêu cầu thất bại.";
        }

        private class UnreadCountDto
        {
            public int Count { get; set; }
        }
    }
}
