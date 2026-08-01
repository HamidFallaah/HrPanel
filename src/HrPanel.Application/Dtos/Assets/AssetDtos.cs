using HrPanel.Domain.Assets;

namespace HrPanel.Application.Dtos.Assets;

public sealed record GetAssetsDto(string? Search = null,short? AssetTypeId = null,AssetStatus? Status = null,long? EmployeeId = null,int PageNumber = 1,int PageSize = 20);

public sealed class CreateAssetDto
{
public short AssetTypeId { get; set; }
public string? AssetTag { get; set; }
public string? ServiceNumber { get; set; }
public string? Imei { get; set; }
public string? SerialNumber { get; set; }
public string? Notes { get; set; }

}

public sealed class UpdateAssetDto
{
    public short AssetTypeId { get; set; }
    public string? AssetTag { get; set; }
    public string? ServiceNumber { get; set; }
    public string? Imei { get; set; }
    public string? SerialNumber { get; set; }
    public string? Notes { get; set; }
}

public sealed class AssignAssetDto
{
    public long EmployeeId { get; set; }
    public DateOnly AssignedAt { get; set; }
    public string? Notes { get; set; }
}

public sealed class ReturnAssetDto
{
    public DateOnly ReturnedAt { get; set; }
}

public sealed record AssetListItemDto(
    long Id,
    short AssetTypeId,
    string AssetTypeName,
    string? AssetTag,
    string? ServiceNumber,
    string? Imei,
    string? SerialNumber,
    AssetStatus Status,
    long? AssignedEmployeeId,
    string? AssignedEmployeeNumber,
    string? AssignedEmployeeName);

public sealed record AssetDetailsDto(
    long Id,
    short AssetTypeId,
    string AssetTypeCode,
    string AssetTypeName,
    string? AssetTag,
    string? ServiceNumber,
    string? Imei,
    string? SerialNumber,
    AssetStatus Status,
    string? Notes,
    IReadOnlyCollection<AssetAssignmentDto> Assignments,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record AssetAssignmentDto(
    long Id,
    long EmployeeId,
    string EmployeeNumber,
    string EmployeeDisplayName,
    DateOnly AssignedAt,
    DateOnly? ReturnedAt,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
