using ExpenseManagementAPI.DTOs.Auth;
using FluentValidation;

namespace ExpenseManagementAPI.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập hoặc email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu");
        }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập")
                .Length(3, 100).WithMessage("Tên đăng nhập từ 3–100 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu")
                .MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu hiện tại");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu mới")
                .MinimumLength(6).WithMessage("Mật khẩu mới tối thiểu 6 ký tự");
        }
    }

    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email tối đa 100 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.FullName)
                .MaximumLength(255).WithMessage("Họ tên tối đa 255 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));
        }
    }
}
