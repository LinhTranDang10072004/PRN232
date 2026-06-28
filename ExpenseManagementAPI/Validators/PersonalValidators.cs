using ExpenseManagementAPI.DTOs.Personal;
using FluentValidation;

namespace ExpenseManagementAPI.Validators
{
    public class ExpenseRequestValidator : AbstractValidator<ExpenseRequest>
    {
        public ExpenseRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Vui lòng nhập tiêu đề")
                .MaximumLength(255).WithMessage("Tiêu đề tối đa 255 ký tự");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0");

            RuleFor(x => x.ExpenseDate)
                .NotEmpty().WithMessage("Vui lòng chọn ngày chi");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục");

            RuleFor(x => x.WalletId)
                .GreaterThan(0).WithMessage("Vui lòng chọn ví");
        }
    }

    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Vui lòng nhập tên danh mục")
                .MaximumLength(255).WithMessage("Tên danh mục tối đa 255 ký tự");
        }
    }

    public class WalletRequestValidator : AbstractValidator<WalletRequest>
    {
        private static readonly string[] AllowedTypes = { "Cash", "Bank", "Momo", "Savings" };

        public WalletRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Vui lòng nhập tên ví")
                .MaximumLength(255).WithMessage("Tên ví tối đa 255 ký tự");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Vui lòng chọn loại ví")
                .Must(t => AllowedTypes.Contains(t)).WithMessage("Loại ví không hợp lệ");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Số dư ban đầu không được âm");
        }
    }

    public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequest>
    {
        public CreateBudgetRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục");

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12");

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100).WithMessage("Năm không hợp lệ");

            RuleFor(x => x.LimitAmount)
                .GreaterThan(0).WithMessage("Hạn mức phải lớn hơn 0");
        }
    }

    public class UpdateBudgetLimitRequestValidator : AbstractValidator<UpdateBudgetLimitRequest>
    {
        public UpdateBudgetLimitRequestValidator()
        {
            RuleFor(x => x.LimitAmount)
                .GreaterThan(0).WithMessage("Hạn mức phải lớn hơn 0");
        }
    }
}
