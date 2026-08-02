using System.ComponentModel.DataAnnotations;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Application.Dtos.Lookups;
using HrPanel.Application.Dtos.Scheduling;

namespace HrPanel.UI.Models.Employments;

public sealed class StartEmploymentViewModel
{
    [Range(1, long.MaxValue, ErrorMessage = "کارمند را انتخاب کنید")]
    public long EmployeeId { get; set; }
    [Range(1, short.MaxValue, ErrorMessage = "نوع استخدام را انتخاب کنید")]
    public short EmploymentTypeId { get; set; }
    [Range(1, short.MaxValue, ErrorMessage = "وضعیت استخدام را انتخاب کنید")]
    public short EmploymentStatusId { get; set; }
    public short? WorkTimeTypeId { get; set; }
    [Required(ErrorMessage = "تاریخ شروع الزامی است")]
    public string StartDate { get; set; } = string.Empty;
    [Range(1, 120, ErrorMessage = "مدت قرارداد بین ۱ تا ۱۲۰ ماه باشد")]
    public short? ContractTermMonths { get; set; }
    public string? EmployeeSearch { get; set; }
}

public sealed record EmploymentDetailsViewModel(
    EmploymentDetailsDto Employment,
    EmploymentLookupsDto EmploymentLookups,
    OrganizationLookupsDto OrganizationLookups,
    SchedulingLookupsDto SchedulingLookups,
    IReadOnlyCollection<ScheduleAssignmentDto> ScheduleAssignments,
    IReadOnlyCollection<EmployeeListItemDto> EmployeeOptions,
    IReadOnlyCollection<ExternalPersonDto> ExternalPeople);
