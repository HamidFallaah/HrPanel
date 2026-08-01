using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.Employees;

public static class EmployeeErrors
{
    public static Error NotFound(long employeeId)
    {
        return Error.NotFound("Employees.NotFound",$"کارمند با شناسه {employeeId} یافت نشد");
    }
    public static Error EmployeeNumberAlreadyExists(string employeeNumber)
    {
        return Error.Conflict("Employees.EmployeeNumberAlreadyExists", $"شماره پرسنلی {employeeNumber} قبلاً ثبت شده است");
    }
    public static Error NationalCodeAlreadyExists(string nationalCode)
    {
        return Error.Conflict("Employees.NationalCodeAlreadyExists",$"کد ملی {nationalCode} قبلاً ثبت شده است");
    }
    public static Error ContactNotFound(long contactId)
    {
        return Error.NotFound("Employees.ContactNotFound", $"راه ارتباطی با شناسه {contactId} برای این کارمند یافت نشد");
    }
    public static Error IdentifierNotFound(long identifierId)
    {
        return Error.NotFound("Employees.IdentifierNotFound", $"شناسه با شماره {identifierId} برای این کارمند یافت نشد");
    }
    public static Error ActiveIdentifierAlreadyExists(string value)
    {
        return Error.Conflict("Employees.ActiveIdentifierAlreadyExists",$"شناسه فعال با مقدار {value} قبلاً ثبت شده است");
    }
    public static Error EducationNotFound(long educationId)
    {
        return Error.NotFound("Employees.EducationNotFound",$"سابقه تحصیلی با شناسه {educationId} یافت نشد");
    }
    public static Error DependentNotFound(long dependentId)
    {
        return Error.NotFound("Employees.DependentNotFound",$"وابسته با شناسه {dependentId} یافت نشد");
    }
}
