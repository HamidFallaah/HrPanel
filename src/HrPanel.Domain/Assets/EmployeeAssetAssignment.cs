using HrPanel.Domain.Common;

namespace HrPanel.Domain.Assets;

// because both TD and IMEI are empty in this batch, asset.Assets and asset.EmployeeAssetAssignments should remain empty for now unless the values exist in the original table
public sealed class EmployeeAssetAssignment : AuditableEntity<long>
{
    private EmployeeAssetAssignment()
    {

    }

    private EmployeeAssetAssignment(long assetId,long employeeId,DateOnly assignedAt,string? notes)
    {
        AssetId = assetId;
        EmployeeId = employeeId;
        AssignedAt = assignedAt;
        Notes = notes?.Trim();
    }

    public long AssetId { get; private set; }
    public long EmployeeId { get; private set; }
    public DateOnly AssignedAt { get; private set; }
    public DateOnly? ReturnedAt { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive => ReturnedAt is null;
    public Asset Asset { get; private set; } = null!;
    public Employees.Employee Employee { get; private set; } = null!;
    public static EmployeeAssetAssignment Create(long assetId,long employeeId,DateOnly assignedAt,string? notes = null)
    {
        return new EmployeeAssetAssignment(assetId,employeeId,assignedAt,notes);
    }

    public void Return(DateOnly returnedAt)
    {
        if (ReturnedAt.HasValue)
        {
            throw new DomainRuleException("دارایی قبلاً بازگردانده شده است");
        }

        if (returnedAt < AssignedAt)
        {
            throw new DomainRuleException("تاریخ بازگشت نمی‌ تواند مقدم بر تاریخ واگذاری باشد");
        }

        ReturnedAt = returnedAt;
    }
}