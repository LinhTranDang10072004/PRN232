using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace BusinessObjects.Validation
{
    public static class CategoryScopeRules
    {
        /// <summary>Nhánh 1: danh mục hệ thống (OwnerUserId null) + danh mục tự tạo.</summary>
        public static IQueryable<Category> ForPersonalUser(IQueryable<Category> query, int userId) =>
            query.Where(c =>
                c.CompanyId == null &&
                c.Status == CategoryStatus.Active &&
                (c.OwnerUserId == null || c.OwnerUserId == userId));

        /// <summary>Nhánh 1: danh mục hệ thống (OwnerUserId null) + danh mục tự tạo (kể cả inactive).</summary>
        public static IQueryable<Category> ForPersonalUserManage(IQueryable<Category> query, int userId) =>
            query.Where(c =>
                c.CompanyId == null &&
                ((c.OwnerUserId == null && c.Status == CategoryStatus.Active) ||
                 c.OwnerUserId == userId));

        public static bool CanModifyPersonalCategory(Category category, int userId) =>
            category.OwnerUserId == userId;

        public static IQueryable<Category> ForCorporateCompany(IQueryable<Category> query, int companyId) =>
            query.Where(c =>
                c.CompanyId == companyId &&
                c.Status == CategoryStatus.Active);

        public static (bool Ok, string? Error) ValidatePersonalCreate(int userId, int? companyId, int? ownerUserId)
        {
            if (companyId != null)
                return (false, "Danh mục cá nhân không được gắn công ty.");

            if (ownerUserId != userId)
                return (false, "OwnerUserId phải là chính người tạo.");

            return (true, null);
        }

        public static (bool Ok, string? Error) ValidateCorporateCreate(int adminCompanyId, int? companyId, int? ownerUserId)
        {
            if (companyId != adminCompanyId)
                return (false, "Danh mục công ty phải thuộc công ty của Admin.");

            if (ownerUserId != null)
                return (false, "Danh mục công ty không gắn OwnerUserId.");

            return (true, null);
        }
    }
}
