using ClientMVC.Models.Corporate;
using ClientMVC.Models.Personal;
using ClientMVC.Models.Shared;

namespace ClientMVC.Helpers
{
    public static class CalendarHelper
    {
        public static List<CalendarDayCell> BuildMonthGrid(
            int year, int month, List<CorporateExpenseItem> monthExpenses, int selectedDay)
        {
            var today = DateTime.Today;
            var cells = new List<CalendarDayCell>();
            var first = new DateTime(year, month, 1);
            var startOffset = (int)first.DayOfWeek;

            for (var i = 0; i < startOffset; i++)
                cells.Add(new CalendarDayCell { Day = null });

            for (var d = 1; d <= DateTime.DaysInMonth(year, month); d++)
            {
                var dayItems = monthExpenses.Where(e => e.ExpenseDate.Day == d).ToList();
                cells.Add(new CalendarDayCell
                {
                    Day = d,
                    ExpenseCount = dayItems.Count,
                    TotalAmount = dayItems.Sum(x => x.Amount),
                    IsToday = today.Year == year && today.Month == month && today.Day == d,
                    IsSelected = d == selectedDay,
                    HasPending = dayItems.Any(x => x.Status == "Pending")
                });
            }
            return cells;
        }

        public static List<PersonalCalendarDayCell> BuildPersonalMonthGrid(
            int year, int month, List<ExpenseItem> monthExpenses, int selectedDay)
        {
            var today = DateTime.Today;
            var cells = new List<PersonalCalendarDayCell>();
            var first = new DateTime(year, month, 1);
            var startOffset = (int)first.DayOfWeek;

            for (var i = 0; i < startOffset; i++)
                cells.Add(new PersonalCalendarDayCell { Day = null });

            for (var d = 1; d <= DateTime.DaysInMonth(year, month); d++)
            {
                var dayItems = monthExpenses.Where(e => e.ExpenseDate.Day == d).ToList();
                cells.Add(new PersonalCalendarDayCell
                {
                    Day = d,
                    ExpenseCount = dayItems.Count,
                    TotalAmount = dayItems.Sum(x => x.Amount),
                    IsToday = today.Year == year && today.Month == month && today.Day == d,
                    IsSelected = d == selectedDay
                });
            }
            return cells;
        }

        public static string BuildMonthODataFilter(int year, int month, CalendarFilterModel? filter = null)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            var parts = new List<string>
            {
                $"ExpenseDate ge {start:yyyy-MM-dd}",
                $"ExpenseDate lt {end:yyyy-MM-dd}"
            };

            if (filter != null)
            {
                if (filter.FilterCategoryId.HasValue)
                    parts.Add($"CategoryId eq {filter.FilterCategoryId.Value}");

                if (filter.FilterMinAmount.HasValue)
                    parts.Add($"Amount ge {filter.FilterMinAmount.Value}");

                if (!string.IsNullOrWhiteSpace(filter.FilterKeyword))
                {
                    var kw = filter.FilterKeyword.Trim().Replace("'", "''");
                    parts.Add($"contains(Title,'{kw}')");
                }

                if (!string.IsNullOrWhiteSpace(filter.FilterStatus))
                {
                    var statusCode = filter.FilterStatus switch
                    {
                        "Approved" => 1,
                        "Rejected" => 2,
                        _ => 0
                    };
                    parts.Add($"Status eq {statusCode}");
                }
            }

            return $"?$filter={string.Join(" and ", parts)}&$orderby=ExpenseDate desc";
        }

        public static object CalendarRoute(int year, int month, int day, CalendarFilterModel? filter = null) => new
        {
            year,
            month,
            day,
            filterKeyword = filter?.FilterKeyword,
            filterCategoryId = filter?.FilterCategoryId,
            filterStatus = filter?.FilterStatus,
            filterMinAmount = filter?.FilterMinAmount
        };

        public static Dictionary<string, string> FilterRouteData(CalendarFilterModel? filter)
        {
            var dict = new Dictionary<string, string>();
            if (filter == null) return dict;
            if (!string.IsNullOrWhiteSpace(filter.FilterKeyword))
                dict["filterKeyword"] = filter.FilterKeyword;
            if (filter.FilterCategoryId.HasValue)
                dict["filterCategoryId"] = filter.FilterCategoryId.Value.ToString();
            if (!string.IsNullOrWhiteSpace(filter.FilterStatus))
                dict["filterStatus"] = filter.FilterStatus;
            if (filter.FilterMinAmount.HasValue)
                dict["filterMinAmount"] = filter.FilterMinAmount.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return dict;
        }
    }
}
