namespace BusinessObjects.Validation
{
    public static class PersonalBudgetRules
    {
        public static decimal EffectiveLimit(decimal limitAmount, decimal carryOverDebt) =>
            Math.Max(0, limitAmount - carryOverDebt);

        public static decimal Remaining(decimal limitAmount, decimal carryOverDebt, decimal currentAmount) =>
            EffectiveLimit(limitAmount, carryOverDebt) - currentAmount;

        public static bool IsExceeded(decimal limitAmount, decimal carryOverDebt, decimal currentAmount) =>
            currentAmount > EffectiveLimit(limitAmount, carryOverDebt);

        public static decimal OverflowAmount(decimal limitAmount, decimal currentAmount) =>
            Math.Max(0, currentAmount - limitAmount);

        public static decimal SurplusAmount(decimal limitAmount, decimal carryOverDebt, decimal currentAmount) =>
            Math.Max(0, Remaining(limitAmount, carryOverDebt, currentAmount));

        public static string CloseOutcome(decimal limitAmount, decimal carryOverDebt, decimal currentAmount)
        {
            if (IsExceeded(limitAmount, carryOverDebt, currentAmount))
                return "Vượt";
            if (SurplusAmount(limitAmount, carryOverDebt, currentAmount) > 0)
                return "Dư";
            return "Vừa đủ";
        }

        public static (int Month, int Year) NextMonth(int month, int year) =>
            month == 12 ? (1, year + 1) : (month + 1, year);

        public static (int Month, int Year) PreviousMonth(int month, int year) =>
            month == 1 ? (12, year - 1) : (month - 1, year);
    }
}
