using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace BusinessObjects.Validation
{
    public static class UserAccountRules
    {
        public static bool IsPersonalBranch(UserRole role) => role == UserRole.User;

        public static bool IsCorporateBranch(UserRole role) =>
            role is UserRole.CompanyAdmin or UserRole.CompanyStaff;

        /// <summary>Nhánh 1: CompanyId phải null. Nhánh 2: CompanyId bắt buộc.</summary>
        public static (bool Ok, string? Error) ValidateCompanyId(UserRole role, int? companyId)
        {
            if (IsPersonalBranch(role) && companyId != null)
                return (false, "Tài khoản cá nhân không được gắn công ty.");

            if (IsCorporateBranch(role) && companyId == null)
                return (false, "Admin/Staff phải thuộc một công ty.");

            return (true, null);
        }

        public static IQueryable<User> StaffOfCompany(IQueryable<User> users, int companyId) =>
            users.Where(u =>
                u.Role == UserRole.CompanyStaff &&
                u.CompanyId == companyId);
    }
}
