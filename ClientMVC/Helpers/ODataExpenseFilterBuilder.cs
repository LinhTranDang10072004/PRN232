using System.Globalization;
using ClientMVC.Models;

namespace ClientMVC.Helpers
{
    public static class ODataExpenseFilterBuilder
    {
        public static string? Build(ExpenseFilterModel filter, ExpenseFilterScope scope)
        {
            var parts = new List<string>();

            AddDateFilter(parts, filter);

            if (filter.CategoryId is > 0)
                parts.Add($"CategoryId eq {filter.CategoryId.Value}");

            if (filter.WalletId is > 0 && scope == ExpenseFilterScope.Personal)
                parts.Add($"WalletId eq {filter.WalletId.Value}");

            if (filter.AccountId is > 0 && scope is ExpenseFilterScope.Staff or ExpenseFilterScope.Admin)
                parts.Add($"AccountId eq {filter.AccountId.Value}");

            if (filter.Status.HasValue)
                parts.Add($"Status eq {(int)filter.Status.Value}");

            if (filter.MinAmount is > 0)
                parts.Add($"Amount ge {filter.MinAmount.Value.ToString(CultureInfo.InvariantCulture)}");

            if (filter.MaxAmount is > 0)
                parts.Add($"Amount le {filter.MaxAmount.Value.ToString(CultureInfo.InvariantCulture)}");

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                parts.Add($"contains(Title,'{EscapeOData(filter.Keyword.Trim())}')");

            if (!string.IsNullOrWhiteSpace(filter.StaffKeyword) && scope == ExpenseFilterScope.Admin)
            {
                var kw = EscapeOData(filter.StaffKeyword.Trim());
                parts.Add($"(contains(StaffFullName,'{kw}') or contains(StaffUserName,'{kw}'))");
            }

            return parts.Count > 0 ? string.Join(" and ", parts) : null;
        }

        public static string BuildQueryString(string? filter, string orderBy = "ExpenseDate desc")
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter))
                parts.Add("$filter=" + Uri.EscapeDataString(filter));
            if (!string.IsNullOrWhiteSpace(orderBy))
                parts.Add("$orderby=" + Uri.EscapeDataString(orderBy));
            return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
        }

        public static string GetPeriodLabel(string period) => period.ToLowerInvariant() switch
        {
            "today" => "Hôm nay",
            "week" => "Tuần này",
            "month" => "Tháng này",
            "lastmonth" => "Tháng trước",
            "year" => "Năm nay",
            "custom" => "Tùy chọn",
            _ => "Tất cả"
        };

        private static void AddDateFilter(List<string> parts, ExpenseFilterModel filter)
        {
            var (start, end) = ResolveDateRange(filter);
            if (start.HasValue)
                parts.Add($"ExpenseDate ge {start.Value:yyyy-MM-dd}");
            if (end.HasValue)
                parts.Add($"ExpenseDate lt {end.Value:yyyy-MM-dd}");
        }

        private static (DateTime? Start, DateTime? End) ResolveDateRange(ExpenseFilterModel filter)
        {
            var today = DateTime.Today;

            return filter.Period.ToLowerInvariant() switch
            {
                "today" => (today, today.AddDays(1)),
                "week" => GetWeekRange(today),
                "month" => (new DateTime(today.Year, today.Month, 1),
                    new DateTime(today.Year, today.Month, 1).AddMonths(1)),
                "lastmonth" => GetLastMonthRange(today),
                "year" => (new DateTime(today.Year, 1, 1), new DateTime(today.Year + 1, 1, 1)),
                "custom" when filter.FromDate.HasValue => (
                    filter.FromDate.Value.Date,
                    (filter.ToDate ?? filter.FromDate.Value).Date.AddDays(1)),
                "all" => (null, null),
                _ => (null, null)
            };
        }

        private static (DateTime Start, DateTime End) GetWeekRange(DateTime today)
        {
            var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var start = today.AddDays(-diff);
            return (start, start.AddDays(7));
        }

        private static (DateTime Start, DateTime End) GetLastMonthRange(DateTime today)
        {
            var first = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            return (first, first.AddMonths(1));
        }

        private static string EscapeOData(string value) =>
            value.Replace("'", "''", StringComparison.Ordinal);
    }
}
