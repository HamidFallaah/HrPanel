using System.ComponentModel.DataAnnotations;
using HrPanel.Domain.Organization;

namespace HrPanel.UI.Models.Organization;

public sealed class OrganizationUnitFormViewModel
{
    public long? Id { get; set; }
    [Range(1, short.MaxValue)] public short OrganizationUnitTypeId { get; set; }
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public long? ParentOrganizationUnitId { get; set; }
}

public sealed class OrganizationItemFormViewModel
{
    public long? Id { get; set; }
    public string Section { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public OperationalGroupType GroupType { get; set; }
}
