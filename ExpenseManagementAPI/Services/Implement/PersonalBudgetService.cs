using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalBudgetService : IPersonalBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;

        public PersonalBudgetService(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
        }

        public IQueryable<BudgetResponse> GetForUser(int userId, int? month = null, int? year = null)
        {
            var query = _budgetRepository.GetForPersonalUser(userId);
            if (month.HasValue)
                query = query.Where(b => b.Month == month);
            if (year.HasValue)
                query = query.Where(b => b.Year == year);

            return query.Select(b => new BudgetResponse
            {
                Id = b.Id,
                Name = b.Name,
                Month = b.Month,
                Year = b.Year,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : null,
                LimitAmount = b.Details.Sum(d => d.LimitAmount),
                SpentAmount = b.Details.Sum(d => d.CurrentAmount),
                RemainingAmount = b.Details.Sum(d => d.LimitAmount) - b.Details.Sum(d => d.CurrentAmount),
                IsExceeded = b.Details.Sum(d => d.CurrentAmount) > b.Details.Sum(d => d.LimitAmount),
                Status = b.Status
            });
        }

        public async Task<(bool Success, BudgetResponse? Data, string? Error)> GetByIdAsync(int userId, int id)
        {
            var budget = await _budgetRepository.GetByIdForUserAsync(userId, id);
            return budget == null
                ? (false, null, "Không tìm thấy budget.")
                : (true, MapBudget(budget), null);
        }

        public async Task<(bool Success, BudgetResponse? Data, string? Error)> CreateAsync(
            int userId, CreateBudgetRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null || category.CompanyId != null)
                return (false, null, "Danh mục không hợp lệ.");

            if (await _budgetRepository.ExistsForMonthAsync(
                    userId, request.CategoryId, request.Month, request.Year))
                return (false, null, "Budget cho danh mục và tháng này đã tồn tại.");

            var budget = new Budget
            {
                UserId = userId,
                CategoryId = request.CategoryId,
                Month = request.Month,
                Year = request.Year,
                Name = request.Name?.Trim() ?? $"{category.Name} tháng {request.Month}/{request.Year}",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                Details = new List<BudgetDetail>
                {
                    new()
                    {
                        LimitAmount = request.LimitAmount,
                        CurrentAmount = 0,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            await _budgetRepository.AddAsync(budget);
            var created = await _budgetRepository.GetByIdForUserAsync(userId, budget.Id);
            return (true, MapBudget(created!), null);
        }

        public async Task<(bool Success, BudgetResponse? Data, string? Error)> UpdateLimitAsync(
            int userId, int id, UpdateBudgetLimitRequest request)
        {
            var budget = await _budgetRepository.GetByIdForUserAsync(userId, id);
            if (budget == null)
                return (false, null, "Không tìm thấy budget.");

            var detail = budget.Details.FirstOrDefault();
            if (detail == null)
                return (false, null, "Budget chưa có chi tiết hạn mức.");

            detail.LimitAmount = request.LimitAmount;
            detail.UpdatedAt = DateTime.UtcNow;
            budget.UpdatedAt = DateTime.UtcNow;
            await _budgetRepository.UpdateDetailAsync(detail);
            await _budgetRepository.UpdateAsync(budget);

            var updated = await _budgetRepository.GetByIdForUserAsync(userId, id);
            return (true, MapBudget(updated!), null);
        }

        internal static BudgetResponse MapBudget(Budget b)
        {
            var detail = b.Details.FirstOrDefault();
            var limit = detail?.LimitAmount ?? 0;
            var spent = detail?.CurrentAmount ?? 0;
            return new BudgetResponse
            {
                Id = b.Id,
                Name = b.Name,
                Month = b.Month,
                Year = b.Year,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name,
                LimitAmount = limit,
                SpentAmount = spent,
                RemainingAmount = limit - spent,
                IsExceeded = spent > limit,
                Status = b.Status
            };
        }
    }
}
