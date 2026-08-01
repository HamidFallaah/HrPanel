namespace HrPanel.Domain.Assets;
public enum AssetStatus : short
{
    Available = 1,
    Assigned = 2,
    UnderMaintenance = 3,
    Retired = 4,
    Lost = 5
}