using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.Assets;

public static class AssetErrors
{
    public static Error NotFound(long id) => Error.NotFound("Assets.NotFound",$"دارایی با شناسه {id} یافت نشد");
    public static Error TypeNotFound(short id) => Error.NotFound("Assets.TypeNotFound",$"نوع دارایی با شناسه {id} یافت نشد");
    public static Error EmployeeNotFound(long id) => Error.NotFound("Assets.EmployeeNotFound",$"کارمند با شناسه {id} یافت نشد");
    public static Error IdentifierExists(string name,string value) => Error.Conflict("Assets.IdentifierExists",$"{name} با مقدار {value} قبلاً ثبت شده است");
    public static Error NotAvailable() => Error.Conflict("Assets.NotAvailable","فقط دارایی آماده قابل واگذاری است");
    public static Error NoActiveAssignment() => Error.NotFound("Assets.NoActiveAssignment","واگذاری فعال برای این دارایی یافت نشد");
}
