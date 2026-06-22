using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Validation
{
    public static class ExpenseDateRules
    {
        public const string FutureDateMessage =
            "Ngày chi tiêu phải là hôm nay hoặc trong quá khứ — không được nhập ngày tương lai.";

        public static string? ValidateNotFuture(DateTime expenseDate)
        {
            if (expenseDate.Date > DateTime.Today)
                return FutureDateMessage;
            return null;
        }
    }

    public class NotFutureDateAttribute : ValidationAttribute
    {
        public NotFutureDateAttribute() : base(ExpenseDateRules.FutureDateMessage) { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dt)
            {
                var error = ExpenseDateRules.ValidateNotFuture(dt);
                if (error != null)
                    return new ValidationResult(error);
            }
            return ValidationResult.Success;
        }
    }
}
