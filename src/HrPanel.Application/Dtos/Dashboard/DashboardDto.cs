namespace HrPanel.Application.Dtos.Dashboard;

public sealed record DashboardMetricDto(string Label, int Value);

public sealed record RecentEmployeeDto(long Id,string EmployeeNumber,string DisplayName,bool IsActive,DateTime ChangedAt);

public sealed record DashboardDto(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    int NewEmployeesLast30Days,
    IReadOnlyCollection<DashboardMetricDto> EmploymentStatuses,
    IReadOnlyCollection<DashboardMetricDto> EmploymentTypes,
    IReadOnlyCollection<DashboardMetricDto> OrganizationUnits,
    IReadOnlyCollection<DashboardMetricDto> WorkLocations,
    IReadOnlyCollection<DashboardMetricDto> AssetStatuses,
    IReadOnlyCollection<RecentEmployeeDto> RecentEmployees,
    int EmployeesWithoutCurrentEmployment,
    int EmploymentsWithoutHrAssignment,
    int LostAssets,
    int AssetsUnderMaintenance);
