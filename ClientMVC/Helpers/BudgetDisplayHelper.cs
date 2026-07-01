namespace ClientMVC.Helpers
{
    public static class BudgetDisplayHelper
    {
        private static readonly string[] Palette =
        {
            "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6",
            "#EC4899", "#06B6D4", "#F97316", "#6366F1"
        };

        public static decimal EffectiveLimit(decimal limit, decimal carryOver) =>
            Math.Max(0, limit - carryOver);

        public static int UsagePercent(decimal limit, decimal carryOver, decimal spent)
        {
            var effective = EffectiveLimit(limit, carryOver);
            if (effective <= 0)
                return spent > 0 ? 100 : 0;

            return (int)Math.Round(spent / effective * 100);
        }

        public static int BarWidthPercent(decimal limit, decimal carryOver, decimal spent) =>
            Math.Min(100, UsagePercent(limit, carryOver, spent));

        public static string GetColor(int index) => Palette[index % Palette.Length];

        public static string StatusLabel(bool isExceeded, int usagePercent) =>
            isExceeded ? "Vượt ngân sách" : usagePercent >= 80 ? "Sắp hết" : "Ổn định";
    }
}
