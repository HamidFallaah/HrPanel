using FluentValidation;
using HrPanel.Application.Dtos.Assets;

namespace HrPanel.Application.Features.Assets.Validators;

public sealed class GetAssetsDtoValidator : AbstractValidator<GetAssetsDto>
{
    public GetAssetsDtoValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.AssetTypeId).GreaterThan((short)0).When(x => x.AssetTypeId.HasValue);
        RuleFor(x => x.Status).Must(status => !status.HasValue || Enum.IsDefined(status.Value));
        RuleFor(x => x.EmployeeId).GreaterThan(0).When(x => x.EmployeeId.HasValue);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1,100);
    }
}
public sealed class CreateAssetDtoValidator : AbstractValidator<CreateAssetDto>
{
    public CreateAssetDtoValidator()
    {
        IncludeCommonRules();
    }

    private void IncludeCommonRules()
    {
        RuleFor(x => x.AssetTypeId).GreaterThan((short)0);
        RuleFor(x => x.AssetTag).MaximumLength(100);
        RuleFor(x => x.ServiceNumber).MaximumLength(100);
        RuleFor(x => x.Imei).MaximumLength(20);
        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
public sealed class UpdateAssetDtoValidator : AbstractValidator<UpdateAssetDto>
{
    public UpdateAssetDtoValidator()
    {
        RuleFor(x => x.AssetTypeId).GreaterThan((short)0);
        RuleFor(x => x.AssetTag).MaximumLength(100);
        RuleFor(x => x.ServiceNumber).MaximumLength(100);
        RuleFor(x => x.Imei).MaximumLength(20);
        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
public sealed class AssignAssetDtoValidator : AbstractValidator<AssignAssetDto>
{
    public AssignAssetDtoValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.AssignedAt).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
public sealed class ReturnAssetDtoValidator : AbstractValidator<ReturnAssetDto>
{
    public ReturnAssetDtoValidator()
    {
        RuleFor(x => x.ReturnedAt).NotEmpty();
    }
}
