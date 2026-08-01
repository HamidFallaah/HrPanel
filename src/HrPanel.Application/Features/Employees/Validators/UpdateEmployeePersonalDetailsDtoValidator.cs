using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class UpdateEmployeePersonalDetailsDtoValidator
    : AbstractValidator<UpdateEmployeePersonalDetailsDto>
{
    public UpdateEmployeePersonalDetailsDtoValidator()
    {
        RuleFor(request => request.FirstNameFa)
            .NotEmpty().WithMessage("نام فارسی الزامی است")
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .WithMessage($"نام فارسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(request => request.LastNameFa)
            .NotEmpty().WithMessage("نام خانوادگی فارسی الزامی است")
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .WithMessage($"نام خانوادگی فارسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(request => request.FirstName)
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.FirstName))
            .WithMessage($"نام انگلیسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(request => request.LastName)
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.LastName))
            .WithMessage($"نام خانوادگی انگلیسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(request => request.NationalCode)
            .Matches("^[0-9]{10}$")
            .When(request => !string.IsNullOrWhiteSpace(request.NationalCode))
            .WithMessage("کد ملی باید دقیقاً ۱۰ رقم باشد");

        RuleFor(request => request.FatherName)
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.FatherName))
            .WithMessage($"نام پدر نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(request => request.FatherNationalCode)
            .Matches("^[0-9]{10}$")
            .When(request => !string.IsNullOrWhiteSpace(request.FatherNationalCode))
            .WithMessage("کد ملی پدر باید دقیقاً ۱۰ رقم باشد");

        RuleFor(request => request.BirthPlace)
            .MaximumLength(EmployeeConstants.BirthPlaceMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.BirthPlace))
            .WithMessage($"محل تولد نمی‌تواند بیشتر از {EmployeeConstants.BirthPlaceMaxLength} کاراکتر باشد");

        RuleFor(request => request.Gender)
            .IsInEnum().WithMessage("جنسیت انتخاب‌شده معتبر نیست");

        RuleFor(request => request.MaritalStatus)
            .IsInEnum().WithMessage("وضعیت تأهل انتخاب‌شده معتبر نیست");
    }
}
