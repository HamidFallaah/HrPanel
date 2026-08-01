using FluentValidation;
using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Features.LegacyImport.Validators;

public sealed class LegacyImportBatchDtoValidator : AbstractValidator<LegacyImportBatchDto>
{
    public LegacyImportBatchDtoValidator()
    {
        RuleFor(request => request.BatchId)
            .NotEmpty()
            .WithMessage("شناسه دسته ورود اطلاعات الزامی است");
    }
}
