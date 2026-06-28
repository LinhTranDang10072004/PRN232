using ExpenseManagementAPI.DTOs.Admin;
using FluentValidation;

namespace ExpenseManagementAPI.Validators
{
    public class RejectExpenseRequestValidator : AbstractValidator<RejectExpenseRequest>
    {
        public RejectExpenseRequestValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Nhập lý do từ chối")
                .MaximumLength(500).WithMessage("Lý do tối đa 500 ký tự");
        }
    }

    public class CreateStaffRequestValidator : AbstractValidator<CreateStaffRequest>
    {
        public CreateStaffRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập")
                .Length(3, 50).WithMessage("Tên đăng nhập từ 3–50 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu")
                .MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Vui lòng nhập email")
                .EmailAddress().WithMessage("Email không hợp lệ");

            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Họ tên tối đa 100 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));
        }
    }

    public class AdminCategoryRequestValidator : AbstractValidator<AdminCategoryRequest>
    {
        public AdminCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Vui lòng nhập tên danh mục")
                .MaximumLength(255).WithMessage("Tên danh mục tối đa 255 ký tự");
        }
    }

    public class AdminAccountRequestValidator : AbstractValidator<AdminAccountRequest>
    {
        public AdminAccountRequestValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Vui lòng nhập số tài khoản")
                .MaximumLength(100).WithMessage("Số tài khoản tối đa 100 ký tự");

            RuleFor(x => x.Name)
                .MaximumLength(255).WithMessage("Tên tài khoản tối đa 255 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
        }
    }
}
