using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Features.Employments;

public static class EmploymentErrors
{
    public static Error NotFound(long id) => Error.NotFound("Employments.NotFound",$"استخدام با شناسه {id} یافت نشد");
    public static Error EmployeeNotFound(long id) => Error.NotFound("Employments.EmployeeNotFound",$"کارمند با شناسه {id} یافت نشد");
    public static Error CurrentEmploymentExists(long employeeId) => Error.Conflict("Employments.CurrentExists",$"برای کارمند {employeeId} استخدام فعال وجود دارد");
    public static Error ReferenceNotFound(string name) => Error.NotFound("Employments.ReferenceNotFound",$"اطلاعات مرجع {name} یافت نشد");
    public static Error AssignmentNotFound(long id) => Error.NotFound("Employments.AssignmentNotFound",$"تخصیص با شناسه {id} یافت نشد");
    public static Error CurrentAssignmentExists() => Error.Conflict("Employments.CurrentAssignmentExists","برای این حوزه تخصیص فعال وجود دارد");
    public static Error OperationalGroupAssignmentNotFound(long id) => Error.NotFound("Employments.GroupAssignmentNotFound",$"تخصیص گروه عملیاتی با شناسه {id} یافت نشد");
    public static Error RelationshipNotFound(long id) => Error.NotFound("Employments.RelationshipNotFound",$"رابطه با شناسه {id} یافت نشد");
    public static Error ExternalPersonNotFound(long id) => Error.NotFound("Employments.ExternalPersonNotFound",$"شخص خارجی با شناسه {id} یافت نشد");
    public static Error DisciplinaryActionNotFound(long id) => Error.NotFound("Employments.DisciplinaryActionNotFound",$"اقدام انضباطی با شناسه {id} یافت نشد");
}
