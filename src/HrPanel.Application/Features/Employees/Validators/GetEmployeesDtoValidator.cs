using FluentValidation;
using HrPanel.Application.Dtos.Employees;

namespace HrPanel.Application.Features.Employees.Validators;

public sealed class GetEmployeesDtoValidator : AbstractValidator<GetEmployeesDto>
{
    public GetEmployeesDtoValidator()
    {
        RuleFor(request => request.Search)
            .MaximumLength(100)
            .When(request => !string.IsNullOrWhiteSpace(request.Search))
            .WithMessage("عبارت جستجو نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد");

        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("تعداد رکوردهای هر صفحه باید بین ۱ تا ۱۰۰ باشد");
    }
}
