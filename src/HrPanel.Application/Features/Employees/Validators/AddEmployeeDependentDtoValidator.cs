using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class AddEmployeeDependentDtoValidator
    : AbstractValidator<AddEmployeeDependentDto>
{
    public AddEmployeeDependentDtoValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty().WithMessage("نام وابسته الزامی است")
            .MaximumLength(EmployeeConstants.DependentFullNameMaxLength)
            .WithMessage($"نام وابسته نمی‌تواند بیشتر از {EmployeeConstants.DependentFullNameMaxLength} کاراکتر باشد");

        RuleFor(request => request.NationalCode)
            .Matches("^[0-9]{10}$")
            .When(request => !string.IsNullOrWhiteSpace(request.NationalCode))
            .WithMessage("کد ملی وابسته باید دقیقاً ۱۰ رقم باشد");

        RuleFor(request => request.RelationshipType)
            .IsInEnum().WithMessage("نوع نسبت معتبر نیست");

        RuleFor(request => request.EmergencyPhone)
            .NotEmpty().WithMessage("تلفن اضطراری الزامی است")
            .When(request => request.IsEmergencyContact);

        RuleFor(request => request.EmergencyPhone)
            .MaximumLength(EmployeeConstants.EmergencyPhoneMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.EmergencyPhone))
            .WithMessage($"تلفن اضطراری نمی‌تواند بیشتر از {EmployeeConstants.EmergencyPhoneMaxLength} کاراکتر باشد");
    }
}
