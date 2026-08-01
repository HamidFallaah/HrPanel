using FluentValidation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class AddEmployeeEducationDtoValidator
    : AbstractValidator<AddEmployeeEducationDto>
{
    public AddEmployeeEducationDtoValidator()
    {
        RuleFor(request => request)
            .Must(request =>
                !string.IsNullOrWhiteSpace(request.DegreeTitle) ||
                !string.IsNullOrWhiteSpace(request.FieldOfStudy))
            .WithName(nameof(AddEmployeeEducationDto.DegreeTitle))
            .WithMessage("حداقل عنوان مدرک یا رشته تحصیلی الزامی است");

        RuleFor(request => request.DegreeTitle)
            .MaximumLength(EmployeeConstants.EducationTitleMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.DegreeTitle))
            .WithMessage($"عنوان مدرک نمی‌تواند بیشتر از {EmployeeConstants.EducationTitleMaxLength} کاراکتر باشد");

        RuleFor(request => request.FieldOfStudy)
            .MaximumLength(EmployeeConstants.EducationTitleMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.FieldOfStudy))
            .WithMessage($"رشته تحصیلی نمی‌تواند بیشتر از {EmployeeConstants.EducationTitleMaxLength} کاراکتر باشد");

        RuleFor(request => request.InstitutionName)
            .MaximumLength(EmployeeConstants.InstitutionNameMaxLength)
            .When(request => !string.IsNullOrWhiteSpace(request.InstitutionName))
            .WithMessage($"نام مؤسسه نمی‌تواند بیشتر از {EmployeeConstants.InstitutionNameMaxLength} کاراکتر باشد");
    }
}
