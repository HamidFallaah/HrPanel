using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.Scheduling;

public static class SchedulingErrors
{
    public static Error ShiftNotFound(long id) => Error.NotFound("Scheduling.ShiftNotFound",$"شیفت با شناسه {id} یافت نشد");
    public static Error WorkScheduleNotFound(long id) => Error.NotFound("Scheduling.WorkScheduleNotFound",$"برنامه کاری با شناسه {id} یافت نشد");
    public static Error ScheduleAssignmentNotFound(long id) => Error.NotFound("Scheduling.AssignmentNotFound",$"تخصیص برنامه کاری با شناسه {id} یافت نشد");
    public static Error EmploymentNotFound(long id) => Error.NotFound("Scheduling.EmploymentNotFound",$"استخدام فعال با شناسه {id} یافت نشد");
    public static Error CodeExists(string code) => Error.Conflict("Scheduling.CodeExists",$"کد {code} قبلاً ثبت شده است");
    public static Error CurrentAssignmentExists() => Error.Conflict("Scheduling.CurrentAssignmentExists","برای این استخدام یک برنامه کاری فعال وجود دارد");
    public static Error InvalidShift() => Error.NotFound("Scheduling.InvalidShift","یک یا چند شیفت برنامه کاری معتبر نیست");
    public static Error InvalidCycle() => Error.Failure("Scheduling.InvalidCycle","طول چرخه جدید با روزهای ثبت‌شده سازگار نیست");
}
