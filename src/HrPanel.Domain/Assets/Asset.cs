using HrPanel.Domain.Common;

namespace HrPanel.Domain.Assets;
// because both TD and IMEI are empty in this batch, asset.Assets and asset.EmployeeAssetAssignments should remain empty for now unless the values exist in the original table
public sealed class Asset : AuditableEntity<long>
{
    private Asset()
    {

    }

    private Asset(short assetTypeId)
    {
        if (assetTypeId <= 0)
        {
            throw new DomainRuleException("شناسه نوع دارایی باید معتبر باشد");
        }

        AssetTypeId = assetTypeId;
        Status = AssetStatus.Available;
    }
    public short AssetTypeId { get; private set; }
    public string? AssetTag { get; private set; }
    public string? ServiceNumber { get; private set; }
    public string? Imei { get; private set; }
    public string? SerialNumber { get; private set; }
    public AssetStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public AssetType AssetType { get; private set; } = null!;
    public static Asset Create(short assetTypeId,string? assetTag,string? serviceNumber,string? imei,string? serialNumber)
    {
        var asset = new Asset(assetTypeId)
        {
            AssetTag = assetTag?.Trim(),
            ServiceNumber = serviceNumber?.Trim(),
            Imei = imei?.Trim(),
            SerialNumber = serialNumber?.Trim()
        };

        return asset;
    }

    public void Update(
        short assetTypeId,
        string? assetTag,
        string? serviceNumber,
        string? imei,
        string? serialNumber,
        string? notes)
    {
        if (assetTypeId <= 0)
        {
            throw new DomainRuleException("شناسه نوع دارایی باید معتبر باشد");
        }

        AssetTypeId = assetTypeId;
        AssetTag = Clean(assetTag);
        ServiceNumber = Clean(serviceNumber);
        Imei = Clean(imei);
        SerialNumber = Clean(serialNumber);
        Notes = Clean(notes);
    }
    public void MarkAsAssigned()
    {
        if (Status != AssetStatus.Available)
        {
            throw new DomainRuleException(
                "Only available assets can be assigned.");
        }

        Status = AssetStatus.Assigned;
    }

    public void MarkAsReturned()
    {
        Status = AssetStatus.Available;
    }

    public void SendToMaintenance()
    {
        if (Status is AssetStatus.Assigned or AssetStatus.Retired)
        {
            throw new DomainRuleException(
                "دارایی واگذارشده یا از رده خارج قابل ارسال به تعمیر نیست");
        }

        Status = AssetStatus.UnderMaintenance;
    }

    public void Retire()
    {
        if (Status == AssetStatus.Assigned)
        {
            throw new DomainRuleException(
                "دارایی واگذارشده را ابتدا بازگردانید");
        }

        Status = AssetStatus.Retired;
    }

    public void MarkAsLost()
    {
        if (Status == AssetStatus.Retired)
        {
            throw new DomainRuleException("دارایی از رده خارج قابل مفقودی نیست");
        }

        Status = AssetStatus.Lost;
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
