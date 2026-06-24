using BusinessObjects.Enums;

namespace BusinessObjects.Validation
{
    public static class ExpenseStatusRules
    {
        public static ExpenseStatus InitialStatusForRole(UserRole role) => role switch
        {
            UserRole.User => ExpenseStatus.Completed,
            UserRole.CompanyStaff => ExpenseStatus.Pending,
            _ => throw new InvalidOperationException("Admin không tạo khoản chi qua luồng Staff.")
        };

        /// <summary>Khóa sửa/xóa khi Admin đã duyệt (Nhánh 2).</summary>
        public static bool IsLocked(ExpenseStatus status) => status == ExpenseStatus.Approved;

        public static bool CanUpdate(UserRole ownerRole, ExpenseStatus status)
        {
            if (ownerRole == UserRole.User)
                return true;

            return !IsLocked(status);
        }

        public static bool CanDelete(UserRole ownerRole, ExpenseStatus status)
        {
            if (ownerRole == UserRole.User)
                return true;

            return !IsLocked(status);
        }
    }
}
