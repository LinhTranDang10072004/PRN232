using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using ExpenseManagementAPI.DTOs.Corporate;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services
{
    public class CorporateExpenseService : ICorporateExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CorporateExpenseService(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
        }

        public IQueryable<CorporateExpenseResponse> GetForStaff(int staffId) =>
            _expenseRepository.GetForStaff(staffId).Select(e => new CorporateExpenseResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Status = e.Status == ExpenseStatus.Approved ? "Approved"
                    : e.Status == ExpenseStatus.Rejected ? "Rejected" : "Pending",
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                CreatedAt = e.CreatedAt,
                StaffId = e.UserId,
                StaffUsername = e.User.Username,
                StaffFullName = e.User.FullName,
                RejectionReason = e.RejectionReason,
                CanEdit = e.Status != ExpenseStatus.Approved,
                CanDelete = e.Status != ExpenseStatus.Approved
            });

        public IQueryable<CorporateExpenseResponse> GetForAdmin(int adminId) =>
            _expenseRepository.GetForAdmin(adminId).Select(e => new CorporateExpenseResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Status = e.Status == ExpenseStatus.Approved ? "Approved"
                    : e.Status == ExpenseStatus.Rejected ? "Rejected" : "Pending",
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                CreatedAt = e.CreatedAt,
                StaffId = e.UserId,
                StaffUsername = e.User.Username,
                StaffFullName = e.User.FullName,
                RejectionReason = e.RejectionReason,
                CanEdit = e.Status != ExpenseStatus.Approved,
                CanDelete = e.Status != ExpenseStatus.Approved
            });

        public async Task<CorporateExpenseResponse?> GetByIdForStaffAsync(int staffId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != staffId)
                return null;
            return MapToEntity(expense);
        }

        public async Task<CorporateExpenseResponse?> GetByIdForAdminAsync(int adminId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null ||
                expense.User.Role != UserRole.Staff ||
                expense.User.ParentAdminId != adminId)
                return null;
            return MapToEntity(expense);
        }

        public async Task<(CorporateExpenseResponse? Result, string? Error)> CreateForStaffAsync(
            int staffId, CorporateExpenseRequest request)
        {
            if (!await IsCorporateCategoryAsync(request.CategoryId))
                return (null, "Danh mục công ty không hợp lệ.");

            var dateError = ExpenseDateRules.ValidateNotFuture(request.ExpenseDate);
            if (dateError != null)
                return (null, dateError);

            var expense = new Expense
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Amount = request.Amount,
                ExpenseDate = request.ExpenseDate.Date,
                CategoryId = request.CategoryId,
                UserId = staffId,
                Status = ExpenseStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _expenseRepository.AddAsync(expense);
            var created = await _expenseRepository.GetByIdAsync(expense.Id);
            return (created == null ? null : MapToEntity(created), null);
        }

        public async Task<(CorporateExpenseResponse? Result, string? Error)> UpdateForStaffAsync(
            int staffId, int expenseId, CorporateExpenseRequest request)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != staffId)
                return (null, "Không tìm thấy khoản chi.");

            if (expense.Status == ExpenseStatus.Approved)
                return (null, "Khoản chi đã được duyệt — không thể sửa.");

            if (!await IsCorporateCategoryAsync(request.CategoryId))
                return (null, "Danh mục công ty không hợp lệ.");

            var dateError = ExpenseDateRules.ValidateNotFuture(request.ExpenseDate);
            if (dateError != null)
                return (null, dateError);

            expense.Title = request.Title.Trim();
            expense.Description = request.Description?.Trim();
            expense.Amount = request.Amount;
            expense.ExpenseDate = request.ExpenseDate.Date;
            expense.CategoryId = request.CategoryId;
            if (expense.Status == ExpenseStatus.Rejected)
            {
                expense.Status = ExpenseStatus.Pending;
                expense.RejectionReason = null;
                expense.ReviewedAt = null;
                expense.ReviewedByAdminId = null;
            }

            await _expenseRepository.UpdateAsync(expense);
            var updated = await _expenseRepository.GetByIdAsync(expenseId);
            return (updated == null ? null : MapToEntity(updated), null);
        }

        public async Task<(bool Success, string? Error)> DeleteForStaffAsync(int staffId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != staffId)
                return (false, "Không tìm thấy khoản chi.");

            if (expense.Status == ExpenseStatus.Approved)
                return (false, "Khoản chi đã được duyệt — không thể xóa.");

            await _expenseRepository.DeleteAsync(expense);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ApproveAsync(int adminId, int expenseId)
        {
            var ok = await _expenseRepository.ApproveAsync(expenseId, adminId);
            return ok ? (true, null) : (false, "Không thể duyệt (đã xử lý hoặc không thuộc quyền quản lý).");
        }

        public async Task<(bool Success, string? Error)> RejectAsync(int adminId, int expenseId, string reason)
        {
            var ok = await _expenseRepository.RejectAsync(expenseId, adminId, reason.Trim());
            return ok ? (true, null) : (false, "Không thể từ chối (đã xử lý hoặc không thuộc quyền quản lý).");
        }

        public async Task<List<CategoryOptionDto>> GetCorporateCategoriesAsync()
        {
            var list = await _categoryRepository.GetByBranchAsync(CategoryBranch.Corporate);
            return list.Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name }).ToList();
        }

        private async Task<bool> IsCorporateCategoryAsync(int categoryId)
        {
            var cat = await _categoryRepository.GetByIdAsync(categoryId);
            return cat != null && cat.Branch == CategoryBranch.Corporate;
        }

        private static CorporateExpenseResponse Project(Expense e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Amount = e.Amount,
            ExpenseDate = e.ExpenseDate,
            Status = StatusText(e.Status),
            CategoryId = e.CategoryId,
            CategoryName = e.Category.Name,
            CreatedAt = e.CreatedAt,
            StaffId = e.UserId,
            StaffUsername = e.User.Username,
            StaffFullName = e.User.FullName,
            RejectionReason = e.RejectionReason,
            CanEdit = e.Status != ExpenseStatus.Approved,
            CanDelete = e.Status != ExpenseStatus.Approved
        };

        private static CorporateExpenseResponse MapToEntity(Expense e) => Project(e);

        private static string StatusText(ExpenseStatus s) => s switch
        {
            ExpenseStatus.Approved => "Approved",
            ExpenseStatus.Rejected => "Rejected",
            _ => "Pending"
        };
    }
}
