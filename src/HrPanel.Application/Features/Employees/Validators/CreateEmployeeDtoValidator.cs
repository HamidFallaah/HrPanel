using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(employee => employee.EmployeeNumber)
            .NotEmpty()
            .WithMessage("شماره پرسنلی الزامی است")
            .MaximumLength(EmployeeConstants.EmployeeNumberMaxLength)
            .WithMessage(
                $"شماره پرسنلی نمی‌تواند بیشتر از {EmployeeConstants.EmployeeNumberMaxLength} کاراکتر باشد");

        RuleFor(employee => employee.FirstNameFa)
            .NotEmpty()
            .WithMessage("نام فارسی الزامی است")
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .WithMessage(
                $"نام فارسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(employee => employee.LastNameFa)
            .NotEmpty()
            .WithMessage("نام خانوادگی فارسی الزامی است")
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .WithMessage(
                $"نام خانوادگی فارسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(employee => employee.FirstName)
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .When(employee => !string.IsNullOrWhiteSpace(employee.FirstName))
            .WithMessage(
                $"نام انگلیسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(employee => employee.LastName)
            .MaximumLength(EmployeeConstants.NameMaxLength)
            .When(employee => !string.IsNullOrWhiteSpace(employee.LastName))
            .WithMessage(
                $"نام خانوادگی انگلیسی نمی‌تواند بیشتر از {EmployeeConstants.NameMaxLength} کاراکتر باشد");

        RuleFor(employee => employee.NationalCode)
            .Matches("^[0-9]{10}$")
            .When(employee => !string.IsNullOrWhiteSpace(employee.NationalCode))
            .WithMessage("کد ملی باید دقیقاً ۱۰ رقم باشد");
    }
}
