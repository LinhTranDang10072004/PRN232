using BusinessObjects.Enums;

namespace BusinessObjects.Extensions
{
    public static class UserRoleExtensions
    {
        /// <summary>Map enum → claim/API role (giữ tương thích [Authorize(Roles = "Admin")]).</summary>
        public static string ToClaimValue(this UserRole role) => role switch
        {
            UserRole.User => "User",
            UserRole.CompanyAdmin => "Admin",
            UserRole.CompanyStaff => "Staff",
            _ => role.ToString()
        };

        public static UserRole FromClaimValue(string? claim) => claim switch
        {
            "User" => UserRole.User,
            "Admin" => UserRole.CompanyAdmin,
            "Staff" => UserRole.CompanyStaff,
            _ => Enum.TryParse<UserRole>(claim, true, out var r) ? r : UserRole.User
        };
    }
}
