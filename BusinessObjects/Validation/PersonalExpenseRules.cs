namespace BusinessObjects.Validation
{
    public static class PersonalExpenseRules
    {
        public static (bool Ok, string? Error) ValidateWalletBalance(decimal walletBalance, decimal expenseAmount)
        {
            if (expenseAmount <= 0)
                return (false, "Số tiền chi phải lớn hơn 0.");

            if (walletBalance < expenseAmount)
                return (false, "Số dư ví không đủ.");

            return (true, null);
        }

        public static bool ExpenseDateMatchesBudget(DateTime expenseDate, int? budgetMonth, int? budgetYear) =>
            budgetMonth == expenseDate.Month && budgetYear == expenseDate.Year;
    }
}