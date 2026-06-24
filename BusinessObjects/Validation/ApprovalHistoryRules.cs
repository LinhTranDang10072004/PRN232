using BusinessObjects.Enums;

namespace BusinessObjects.Validation
{
    /// <summary>Chỉ Nhánh 2 (Corporate) ghi ApprovalHistory. Nhánh 1 không bao giờ ghi.</summary>
    public static class ApprovalHistoryRules
    {
        public static bool ShouldRecord(UserRole expenseOwnerRole) =>
            expenseOwnerRole == UserRole.CompanyStaff;

        public const string ActionApproved = "Approved";
        public const string ActionRejected = "Rejected";
    }
}
