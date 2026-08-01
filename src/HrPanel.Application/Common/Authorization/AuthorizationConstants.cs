namespace HrPanel.Application.Common.Authorization;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string HrStaff = "HrStaff";
}
public static class PolicyNames
{
    public const string AdministratorOnly = "AdministratorOnly";
    public const string HrAccess = "HrAccess";
}
public static class CustomClaimTypes
{
    public const string EmployeeId = "employee_id";
}