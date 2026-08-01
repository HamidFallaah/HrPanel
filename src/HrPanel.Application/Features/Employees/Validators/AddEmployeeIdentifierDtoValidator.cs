using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class AddEmployeeIdentifierDtoValidator
    : AbstractValidator<AddEmployeeIdentifierDto>
{
    public AddEmployeeIdentifierDtoValidator()
    {
        RuleFor(request => request.Type)
            .IsInEnum().WithMessage("نوع شناسه معتبر نیست");

        RuleFor(request => request.Value)
            .NotEmpty().WithMessage("مقدار شناسه الزامی است")
            .MaximumLength(EmployeeConstants.IdentifierValueMaxLength)
            .WithMessage($"مقدار شناسه نمی‌تواند بیشتر از {EmployeeConstants.IdentifierValueMaxLength} کاراکتر باشد");
    }
}
