using HrPanel.Domain.Organization;

namespace HrPanel.Application.Dtos.Organization;

public sealed record GetOrganizationItemsDto(string? Search = null,bool? IsActive = null,int PageNumber = 1,int PageSize = 20);

public sealed record GetOrganizationUnitsDto(string? Search = null,bool? IsActive = null,short? OrganizationUnitTypeId = null,long? ParentOrganizationUnitId = null,int PageNumber = 1,int PageSize = 20);

public sealed class CreateOrganizationUnitDto
{
    public short OrganizationUnitTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public long? ParentOrganizationUnitId { get; set; }
}

public sealed class UpdateOrganizationUnitDto
{
    public short OrganizationUnitTypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
}
public sealed class MoveOrganizationUnitDto
{
    public long? ParentOrganizationUnitId { get; set; }
}
public sealed class SavePositionDto
{
    public string Code { get; set; } = string.Empty;
    public string TitleFa { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
}
public sealed class SaveWorkLocationDto
{
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
}
public sealed class CreateOperationalGroupDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public OperationalGroupType Type { get; set; }
}
public sealed class UpdateOperationalGroupDto
{
    public string Name { get; set; } = string.Empty;
    public OperationalGroupType Type { get; set; }
}
public sealed record OrganizationUnitDto(
    long Id,
    short OrganizationUnitTypeId,
    string OrganizationUnitTypeName,
    long? ParentOrganizationUnitId,
    string? ParentOrganizationUnitName,
    string Code,
    string NameFa,
    string? NameEn,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
public sealed record OrganizationUnitTreeDto(long Id,string Code,string NameFa,string? NameEn,short OrganizationUnitTypeId,string OrganizationUnitTypeName,bool IsActive,
IReadOnlyCollection<OrganizationUnitTreeDto> Children);
public sealed record PositionDto(long Id,string Code,string TitleFa,string? TitleEn,bool IsActive,DateTime CreatedAt,DateTime? ModifiedAt);
public sealed record WorkLocationDto(
    long Id,
    string Code,
    string NameFa,
    string? NameEn,
    string? Province,
    string? City,
    string? Address,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
public sealed record OperationalGroupDto(long Id,string Code,string Name,OperationalGroupType Type,bool IsActive,DateTime CreatedAt,DateTime? ModifiedAt);
