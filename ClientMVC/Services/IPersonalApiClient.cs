using ClientMVC.Models.Personal;

namespace ClientMVC.Services
{
    public interface IPersonalApiClient
    {
        Task<(List<ExpenseItem>? Data, string? Error)> GetExpensesAsync(string? odataQuery = null);
        Task<(ExpenseItem? Data, string? Error)> GetExpenseAsync(int id);
        Task<(ExpenseItem? Data, string? Error)> CreateExpenseAsync(ExpenseFormModel model);
        Task<(ExpenseItem? Data, string? Error)> UpdateExpenseAsync(int id, ExpenseFormModel model);
        Task<(bool Success, string? Error)> DeleteExpenseAsync(int id);

        Task<(List<CategoryItem>? Data, string? Error)> GetCategoriesAsync(string? odataQuery = null);
        Task<(CategoryItem? Data, string? Error)> GetCategoryAsync(int id);
        Task<(CategoryItem? Data, string? Error)> CreateCategoryAsync(CategoryFormModel model);
        Task<(CategoryItem? Data, string? Error)> UpdateCategoryAsync(int id, CategoryFormModel model);
        Task<(bool Success, string? Error)> DeleteCategoryAsync(int id);
    }
}
