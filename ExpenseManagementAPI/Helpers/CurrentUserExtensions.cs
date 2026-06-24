using System.Security.Claims;

namespace ExpenseManagementAPI.Helpers
{
    public static class CurrentUserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (value == null || !int.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Không xác định được người dùng.");
            return userId;
        }
    }
}
