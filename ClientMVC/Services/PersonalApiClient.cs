using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Helpers;
using ClientMVC.Models.Personal;

namespace ClientMVC.Services
{
    public class PersonalApiClient : IPersonalApiClient
    {
        private readonly HttpClient _http;

        public PersonalApiClient(HttpClient http) => _http = http;

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

        public async Task<List<WalletDto>> GetWalletsAsync() =>
            await GetODataListAsync<WalletDto>("api/personal/wallets");

        public Task<WalletDto?> GetWalletAsync(int id) =>
            GetAsync<WalletDto>($"api/personal/wallets/{id}");

        public async Task<(bool Ok, WalletDto? Data, string? Error)> CreateWalletAsync(WalletFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/personal/wallets", new
            {
                name = model.Name,
                type = model.Type,
                initialBalance = model.InitialBalance
            }, ApiJsonOptions.Default);
            return await ParseDataAsync<WalletDto>(response);
        }

        public async Task<(bool Ok, WalletDto? Data, string? Error)> UpdateWalletAsync(int id, WalletFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/personal/wallets/{id}", new
            {
                name = model.Name,
                type = model.Type,
                initialBalance = model.InitialBalance
            }, ApiJsonOptions.Default);
            return await ParseDataAsync<WalletDto>(response);
        }

        public Task<(bool Ok, string? Error)> DeactivateWalletAsync(int id) =>
            DeleteAsync($"api/personal/wallets/{id}");

        public async Task<List<CategoryDto>> GetCategoriesAsync() =>
            await GetODataListAsync<CategoryDto>("api/personal/categories");

        public async Task<(bool Ok, CategoryDto? Data, string? Error)> CreateCategoryAsync(CategoryFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/personal/categories", new { name = model.Name }, ApiJsonOptions.Default);
            return await ParseDataAsync<CategoryDto>(response);
        }

        public async Task<(bool Ok, CategoryDto? Data, string? Error)> UpdateCategoryAsync(int id, CategoryFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/personal/categories/{id}", new { name = model.Name }, ApiJsonOptions.Default);
            return await ParseDataAsync<CategoryDto>(response);
        }

        public Task<(bool Ok, string? Error)> DeactivateCategoryAsync(int id) =>
            DeleteAsync($"api/personal/categories/{id}");

        public async Task<List<BudgetDto>> GetBudgetsAsync(int? month = null, int? year = null)
        {
            var qs = new List<string>();
            if (month.HasValue) qs.Add($"month={month}");
            if (year.HasValue) qs.Add($"year={year}");
            var query = qs.Count > 0 ? "?" + string.Join("&", qs) : "";
            return await GetODataListAsync<BudgetDto>($"api/personal/budgets{query}");
        }

        public async Task<(bool Ok, BudgetDto? Data, string? Error)> CreateBudgetAsync(BudgetFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/personal/budgets", new
            {
                categoryId = model.CategoryId,
                month = model.Month,
                year = model.Year,
                limitAmount = model.LimitAmount,
                name = model.Name
            }, ApiJsonOptions.Default);
            return await ParseDataAsync<BudgetDto>(response);
        }

        public async Task<(bool Ok, BudgetDto? Data, string? Error)> UpdateBudgetLimitAsync(int id, decimal limitAmount)
        {
            var response = await _http.PutAsJsonAsync($"api/personal/budgets/{id}/limit",
                new { limitAmount }, ApiJsonOptions.Default);
            return await ParseDataAsync<BudgetDto>(response);
        }

        public Task<List<ExpenseDto>> GetExpensesAsync(string? oDataFilter = null, string? oDataOrderBy = null)
        {
            var url = "api/personal/expenses" + ODataExpenseFilterBuilder.BuildQueryString(oDataFilter, oDataOrderBy ?? "ExpenseDate desc");
            return GetODataListAsync<ExpenseDto>(url);
        }

        public Task<List<ExpenseDto>> GetExpensesForMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            var filter = $"ExpenseDate ge {start:yyyy-MM-dd} and ExpenseDate lt {end:yyyy-MM-dd}";
            return GetExpensesAsync(filter);
        }

        public Task<ExpenseDto?> GetExpenseAsync(int id) =>
            GetAsync<ExpenseDto>($"api/personal/expenses/{id}");

        public async Task<(bool Ok, ExpenseDto? Data, string? Error)> CreateExpenseAsync(ExpenseFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/personal/expenses", MapExpense(model), ApiJsonOptions.Default);
            return await ParseDataAsync<ExpenseDto>(response);
        }

        public async Task<(bool Ok, ExpenseDto? Data, string? Error)> UpdateExpenseAsync(int id, ExpenseFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/personal/expenses/{id}", MapExpense(model), ApiJsonOptions.Default);
            return await ParseDataAsync<ExpenseDto>(response);
        }

        public Task<(bool Ok, string? Error)> DeleteExpenseAsync(int id) =>
            DeleteAsync($"api/personal/expenses/{id}");

        public Task<MonthlySummaryDto?> GetMonthlySummaryAsync(int month, int year) =>
            GetAsync<MonthlySummaryDto>($"api/personal/reports/monthly?month={month}&year={year}");

        public Task<List<CategoryReportDto>> GetCategoryReportAsync(int month, int year) =>
            GetListAsync<CategoryReportDto>($"api/personal/reports/by-category?month={month}&year={year}");

        public Task<List<BudgetStatusDto>> GetBudgetStatusAsync(int month, int year) =>
            GetListAsync<BudgetStatusDto>($"api/personal/reports/budget-status?month={month}&year={year}");

        public Task<YearlyReportDto?> GetYearlyReportAsync(int year) =>
            GetAsync<YearlyReportDto>($"api/personal/reports/yearly?year={year}");

        public async Task<List<NotificationDto>> GetNotificationsAsync() =>
            await GetODataListAsync<NotificationDto>("api/personal/notifications");

        public async Task<int> GetUnreadCountAsync()
        {
            var result = await GetAsync<UnreadCountDto>("api/personal/notifications/unread-count");
            return result?.Count ?? 0;
        }

        public async Task MarkNotificationReadAsync(int id)
        {
            await _http.PutAsync($"api/personal/notifications/{id}/read", null);
        }

        public async Task MarkAllNotificationsReadAsync()
        {
            await _http.PutAsync("api/personal/notifications/read-all", null);
        }

        private static object MapExpense(ExpenseFormModel model) => new
        {
            title = model.Title,
            description = model.Description,
            amount = model.Amount,
            expenseDate = model.ExpenseDate,
            categoryId = model.CategoryId,
            walletId = model.WalletId
        };

        private async Task<T?> GetAsync<T>(string url) where T : class
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default);
        }

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<T>();
            return await response.Content.ReadFromJsonAsync<List<T>>(ApiJsonOptions.Default) ?? new List<T>();
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
