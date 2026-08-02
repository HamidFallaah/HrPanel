using HrPanel.Application.Dtos.Lookups;

namespace HrPanel.UI.Models.ReferenceData;

public sealed record ReferenceDataViewModel(EmployeeLookupsDto Employees,EmploymentLookupsDto Employments,OrganizationLookupsDto Organization,SchedulingLookupsDto Scheduling,AssetLookupsDto Assets);
