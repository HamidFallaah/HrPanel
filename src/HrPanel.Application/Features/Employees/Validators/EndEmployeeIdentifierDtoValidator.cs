using FluentValidation;
using HrPanel.Application.Dtos.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class EndEmployeeIdentifierDtoValidator
    : AbstractValidator<EndEmployeeIdentifierDto>
{
    public EndEmployeeIdentifierDtoValidator()
    {
        RuleFor(request => request.EffectiveTo)
            .NotEmpty().WithMessage("تاریخ پایان شناسه الزامی است");
    }
}
