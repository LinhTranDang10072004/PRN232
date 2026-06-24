using ClientMVC.Models.Personal;

namespace ClientMVC.Helpers
{
    public static class CalendarHelper
    {
        private static readonly string[] WeekdayHeaders = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };

        public static string[] GetWeekdayHeaders() => WeekdayHeaders;

        public static CalendarMonthViewModel Build(int year, int month, List<ExpenseDto> expenses, int? selectedDay = null)
        {
            var firstDay = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var today = DateTime.Today;

            var byDay = expenses
                .Where(e => e.ExpenseDate.HasValue)
                .GroupBy(e => e.ExpenseDate!.Value.Day)
                .ToDictionary(g => g.Key, g => g.ToList());

            var startOffset = ((int)firstDay.DayOfWeek + 6) % 7;
            var weeks = new List<CalendarWeekViewModel>();
            var currentWeek = new CalendarWeekViewModel();

            for (var i = 0; i < startOffset; i++)
                currentWeek.Days.Add(new CalendarDayViewModel { IsCurrentMonth = false });

            for (var day = 1; day <= daysInMonth; day++)
            {
                if (currentWeek.Days.Count == 7)
                {
                    weeks.Add(currentWeek);
                    currentWeek = new CalendarWeekViewModel();
                }

                byDay.TryGetValue(day, out var dayExpenses);
                dayExpenses ??= new List<ExpenseDto>();

                currentWeek.Days.Add(new CalendarDayViewModel
                {
                    Day = day,
                    IsCurrentMonth = true,
                    IsToday = today.Year == year && today.Month == month && today.Day == day,
                    Expenses = dayExpenses,
                    DayTotal = dayExpenses.Sum(e => e.Amount)
                });
            }

            while (currentWeek.Days.Count > 0 && currentWeek.Days.Count < 7)
                currentWeek.Days.Add(new CalendarDayViewModel { IsCurrentMonth = false });

            if (currentWeek.Days.Count > 0)
                weeks.Add(currentWeek);

            return new CalendarMonthViewModel
            {
                Year = year,
                Month = month,
                MonthLabel = $"Tháng {month}/{year}",
                Weeks = weeks,
                MonthTotal = expenses.Sum(e => e.Amount),
                ExpenseCount = expenses.Count,
                SelectedDay = selectedDay,
                SelectedDayExpenses = selectedDay.HasValue && byDay.TryGetValue(selectedDay.Value, out var selected)
                    ? selected
                    : new List<ExpenseDto>()
            };
        }

        public static (int Year, int Month) ShiftMonth(int year, int month, int delta)
        {
            var date = new DateTime(year, month, 1).AddMonths(delta);
            return (date.Year, date.Month);
        }
    }
}
