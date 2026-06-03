using BusinessObjects.Enums;
using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services
{
    public class PersonalExpenseService : IPersonalExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IPersonalCategoryService _categoryService;

        public PersonalExpenseService(
            IExpenseRepository expenseRepository,
            IPersonalCategoryService categoryService)
        {
            _expenseRepository = expenseRepository;
            _categoryService = categoryService;
        }

        public IQueryable<ExpenseResponse> GetMyExpenses(int userId) =>
            _expenseRepository.GetForUser(userId).Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Status = e.Status == ExpenseStatus.Approved ? "Approved"
                    : e.Status == ExpenseStatus.Rejected ? "Rejected"
                    : "Pending",
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                CreatedAt = e.CreatedAt
            });

        public async Task<ExpenseResponse?> GetByIdAsync(int userId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
                return null;

            return MapToResponse(expense);
        }

        public async Task<(ExpenseResponse? Result, string? Error)> CreateAsync(
            int userId, CreateExpenseRequest request)
        {
            var categoryError = await ValidatePersonalCategoryAsync(userId, request.CategoryId);
            if (categoryError != null)
                return (null, categoryError);

            var expense = new Expense
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Amount = request.Amount,
                ExpenseDate = request.ExpenseDate,
                CategoryId = request.CategoryId,
                UserId = userId,
                Status = ExpenseStatus.Approved,
                CreatedAt = DateTime.UtcNow
            };

            await _expenseRepository.AddAsync(expense);
            var created = await _expenseRepository.GetByIdAsync(expense.Id);
            return (created == null ? null : MapToResponse(created), null);
        }

        public async Task<(ExpenseResponse? Result, string? Error)> UpdateAsync(
            int userId, int expenseId, UpdateExpenseRequest request)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
                return (null, "Không tìm thấy khoản chi hoặc bạn không có quyền sửa.");

            var categoryError = await ValidatePersonalCategoryAsync(userId, request.CategoryId);
            if (categoryError != null)
                return (null, categoryError);

            expense.Title = request.Title.Trim();
            expense.Description = request.Description?.Trim();
            expense.Amount = request.Amount;
            expense.ExpenseDate = request.ExpenseDate;
            expense.CategoryId = request.CategoryId;
            expense.Status = ExpenseStatus.Approved;

            await _expenseRepository.UpdateAsync(expense);
            var updated = await _expenseRepository.GetByIdAsync(expenseId);
            return (updated == null ? null : MapToResponse(updated), null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int userId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
                return (false, "Không tìm thấy khoản chi hoặc bạn không có quyền xóa.");

            await _expenseRepository.DeleteAsync(expense);
            return (true, null);
        }

        private async Task<string?> ValidatePersonalCategoryAsync(int userId, int categoryId)
        {
            if (!await _categoryService.CanUserUseCategoryAsync(userId, categoryId))
                return "Danh mục không hợp lệ hoặc bạn không có quyền dùng.";
            return null;
        }

        private static ExpenseResponse MapToResponse(Expense e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Amount = e.Amount,
            ExpenseDate = e.ExpenseDate,
            Status = e.Status == ExpenseStatus.Approved ? "Approved"
                : e.Status == ExpenseStatus.Rejected ? "Rejected"
                : "Pending",
            CategoryId = e.CategoryId,
            CategoryName = e.Category.Name,
            CreatedAt = e.CreatedAt
        };
    }
}
