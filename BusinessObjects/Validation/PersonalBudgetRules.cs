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

        public static (int Month, int Year) NextMonth(int month, int year) =>
            month == 12 ? (1, year + 1) : (month + 1, year);

        public static (int Month, int Year) PreviousMonth(int month, int year) =>
            month == 1 ? (12, year - 1) : (month - 1, year);
    }
}
