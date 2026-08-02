using System.ComponentModel.DataAnnotations;

namespace HrPanel.UI.Models.Assets;
public sealed class AssetFormViewModel
{
    public long? Id { get; set; }
    [Range(1, short.MaxValue, ErrorMessage = "نوع دارایی را انتخاب کنید")] public short AssetTypeId { get; set; }
    public string? AssetTag { get; set; }
    public string? ServiceNumber { get; set; }
    public string? Imei { get; set; }
    public string? SerialNumber { get; set; }
    public string? Notes { get; set; }
}
