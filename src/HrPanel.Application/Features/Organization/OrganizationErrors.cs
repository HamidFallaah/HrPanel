using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.Organization;

public static class OrganizationErrors
{
    public static Error UnitNotFound(long id) => Error.NotFound("Organization.UnitNotFound",$"واحد سازمانی با شناسه {id} یافت نشد");
    public static Error PositionNotFound(long id) => Error.NotFound("Organization.PositionNotFound",$"سمت شغلی با شناسه {id} یافت نشد");
    public static Error WorkLocationNotFound(long id) => Error.NotFound("Organization.WorkLocationNotFound",$"محل کار با شناسه {id} یافت نشد");
    public static Error OperationalGroupNotFound(long id) => Error.NotFound("Organization.OperationalGroupNotFound",$"گروه عملیاتی با شناسه {id} یافت نشد");
    public static Error ReferenceNotFound(string name) => Error.NotFound("Organization.ReferenceNotFound",$"اطلاعات مرجع {name} یافت نشد");
    public static Error CodeExists(string code) => Error.Conflict("Organization.CodeExists",$"کد {code} قبلاً ثبت شده است");
    public static Error InvalidParent() => Error.Failure("Organization.InvalidParent","واحد سازمانی والد معتبر نیست");
    public static Error ParentCycle() => Error.Conflict("Organization.ParentCycle","انتقال واحد سازمانی باعث ایجاد چرخه در ساختار می‌شود");
}
