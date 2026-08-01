using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class UpdateEmployeeContactDtoValidator
    : AbstractValidator<UpdateEmployeeContactDto>
{
    public UpdateEmployeeContactDtoValidator()
    {
        RuleFor(request => request.Value)
            .NotEmpty().WithMessage("مقدار راه ارتباطی الزامی است")
            .MaximumLength(EmployeeConstants.ContactValueMaxLength)
            .WithMessage($"مقدار راه ارتباطی نمی‌تواند بیشتر از {EmployeeConstants.ContactValueMaxLength} کاراکتر باشد");
    }
}
