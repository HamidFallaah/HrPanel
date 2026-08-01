using FluentValidation;
using HrPanel.Application.Dtos.Identity;

namespace HrPanel.Application.Features.Identity.Validators;

public sealed class LoginRequestDtoValidator: AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(request => request.UserName).NotEmpty().WithMessage("نام کاربری الزامی است").MaximumLength(256).WithMessage("نام کاربری نمی ‌تواند بیشتر از ۲۵۶ کاراکتر باشد");

        RuleFor(request => request.Password).NotEmpty().WithMessage("رمز عبور الزامی است").MaximumLength(256).WithMessage("رمز عبور نمی ‌تواند بیشتر از ۲۵۶ کاراکتر باشد");
    }
}