using FluentValidation;
using HrPanel.Application.Dtos.Organization;

namespace HrPanel.Application.Features.Organization.Validators;

public sealed class GetOrganizationItemsDtoValidator : AbstractValidator<GetOrganizationItemsDto>
{
    public GetOrganizationItemsDtoValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1,100);
    }
}

public sealed class GetOrganizationUnitsDtoValidator : AbstractValidator<GetOrganizationUnitsDto>
{
    public GetOrganizationUnitsDtoValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.OrganizationUnitTypeId).GreaterThan((short)0).When(x => x.OrganizationUnitTypeId.HasValue);
        RuleFor(x => x.ParentOrganizationUnitId).GreaterThan(0).When(x => x.ParentOrganizationUnitId.HasValue);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1,100);
    }
}

public sealed class CreateOrganizationUnitDtoValidator : AbstractValidator<CreateOrganizationUnitDto>
{
    public CreateOrganizationUnitDtoValidator()
    {
        RuleFor(x => x.OrganizationUnitTypeId).GreaterThan((short)0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameEn).MaximumLength(150);
        RuleFor(x => x.ParentOrganizationUnitId).GreaterThan(0).When(x => x.ParentOrganizationUnitId.HasValue);
    }
}

public sealed class UpdateOrganizationUnitDtoValidator : AbstractValidator<UpdateOrganizationUnitDto>
{
    public UpdateOrganizationUnitDtoValidator()
    {
        RuleFor(x => x.OrganizationUnitTypeId).GreaterThan((short)0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameEn).MaximumLength(150);
    }
}

public sealed class MoveOrganizationUnitDtoValidator : AbstractValidator<MoveOrganizationUnitDto>
{
    public MoveOrganizationUnitDtoValidator()
    {
        RuleFor(x => x.ParentOrganizationUnitId).GreaterThan(0).When(x => x.ParentOrganizationUnitId.HasValue);
    }
}

public sealed class SavePositionDtoValidator : AbstractValidator<SavePositionDto>
{
    public SavePositionDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TitleFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.TitleEn).MaximumLength(150);
    }
}

public sealed class SaveWorkLocationDtoValidator : AbstractValidator<SaveWorkLocationDto>
{
    public SaveWorkLocationDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameEn).MaximumLength(150);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(1000);
    }
}

public sealed class CreateOperationalGroupDtoValidator : AbstractValidator<CreateOperationalGroupDto>
{
    public CreateOperationalGroupDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public sealed class UpdateOperationalGroupDtoValidator : AbstractValidator<UpdateOperationalGroupDto>
{
    public UpdateOperationalGroupDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
    }
}
