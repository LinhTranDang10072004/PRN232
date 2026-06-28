using ExpenseManagementAPI.DTOs.Staff;
using FluentValidation;

namespace ExpenseManagementAPI.Validators
{
    public class StaffExpenseRequestValidator : AbstractValidator<StaffExpenseRequest>
    {
        public StaffExpenseRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Vui lòng nhập tiêu đề")
                .MaximumLength(255).WithMessage("Tiêu đề tối đa 255 ký tự");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tài khoản công ty");
        }
    }
}
