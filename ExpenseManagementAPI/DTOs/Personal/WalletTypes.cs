namespace ExpenseManagementAPI.DTOs.Personal
{
    public static class WalletTypes
    {
        public const string Cash = "Cash";
        public const string Bank = "Bank";
        public const string Momo = "Momo";
        public const string Savings = "Savings";

        public static readonly string[] All = { Cash, Bank, Momo, Savings };
    }
}
