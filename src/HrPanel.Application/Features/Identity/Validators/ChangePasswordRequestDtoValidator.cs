using FluentValidation;
using HrPanel.Application.Dtos.Identity;

namespace HrPanel.Application.Features.Identity.Validators;

public sealed class ChangePasswordRequestDtoValidator: AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(request => request.CurrentPassword).NotEmpty().WithMessage("رمز عبور فعلی الزامی است").MaximumLength(256).WithMessage("رمز عبور فعلی نمی‌ تواند بیشتر از ۲۵۶ کاراکتر باشد");

        RuleFor(request => request.NewPassword).NotEmpty().WithMessage("رمز عبور جدید الزامی است").MaximumLength(256).WithMessage("رمز عبور جدید نمی ‌تواند بیشتر از ۲۵۶ کاراکتر باشد")
       .NotEqual(request => request.CurrentPassword).WithMessage("رمز عبور جدید باید با رمز عبور فعلی متفاوت باشد");

        RuleFor(request => request.ConfirmNewPassword).NotEmpty().WithMessage("تکرار رمز عبور جدید الزامی است").Equal(request => request.NewPassword).WithMessage("رمز عبور جدید و تکرار آن یکسان نیستند");
    }
}