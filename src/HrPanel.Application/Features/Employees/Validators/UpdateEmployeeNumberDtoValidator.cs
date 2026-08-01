using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class UpdateEmployeeNumberDtoValidator
    : AbstractValidator<UpdateEmployeeNumberDto>
{
    public UpdateEmployeeNumberDtoValidator()
    {
        RuleFor(request => request.EmployeeNumber)
            .NotEmpty().WithMessage("شماره پرسنلی الزامی است")
            .MaximumLength(EmployeeConstants.EmployeeNumberMaxLength)
            .WithMessage(
                $"شماره پرسنلی نمی‌تواند بیشتر از {EmployeeConstants.EmployeeNumberMaxLength} کاراکتر باشد");
    }
}
