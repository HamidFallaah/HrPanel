using System.ComponentModel.DataAnnotations;
using HrPanel.Domain.Scheduling;

namespace HrPanel.UI.Models.Scheduling;

public sealed class ShiftFormViewModel
{
    public long? Id { get; set; }
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    [Range(0.01, 24)] public decimal WorkHours { get; set; }
}
public sealed class ScheduleFormViewModel
{
    public long? Id { get; set; }
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public WorkSchedulePatternType PatternType { get; set; }
    [Range(1, 366)] public short CycleLengthDays { get; set; } = 7;
    public string? AnchorDate { get; set; }
}
