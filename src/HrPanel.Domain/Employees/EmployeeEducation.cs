using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class EmployeeEducation : AuditableEntity<long>
{
    private EmployeeEducation()
    {

    }

    private EmployeeEducation(string? degreeTitle,string? fieldOfStudy,string? institutionName)
    {
        var cleanedDegreeTitle = Clean(degreeTitle);
        var cleanedFieldOfStudy = Clean(fieldOfStudy);
        var cleanedInstitutionName = Clean(institutionName);

        if (cleanedDegreeTitle is null && cleanedFieldOfStudy is null)
        {
            throw new DomainRuleException( "حداقل عنوان مدرک یا رشته تحصیلی الزامی است");
        }

        DegreeTitle = cleanedDegreeTitle;
        FieldOfStudy = cleanedFieldOfStudy;
        InstitutionName = cleanedInstitutionName;
    }
    public long EmployeeId { get; private set; }
    public string? DegreeTitle { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public string? InstitutionName { get; private set; }
    public DateOnly? GraduationDate { get; private set; }
    public bool IsHighestDegree { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public static EmployeeEducation Create(string degreeTitle,string? fieldOfStudy = null,string? institutionName = null)
    {
        if (string.IsNullOrWhiteSpace(degreeTitle))
        {
            throw new DomainRuleException("عنوان مدرک تحصیلی الزامی است");
        }

        return new EmployeeEducation(degreeTitle: degreeTitle,fieldOfStudy: fieldOfStudy,institutionName: institutionName);
    }
    public static EmployeeEducation CreateFieldOfStudyOnly(string fieldOfStudy,string? institutionName = null)
    {
        if (string.IsNullOrWhiteSpace(fieldOfStudy))
        {
            throw new DomainRuleException("رشته تحصیلی الزامی است");
        }

        return new EmployeeEducation(degreeTitle: null,fieldOfStudy: fieldOfStudy, institutionName: institutionName);
    }
    public void MarkAsHighestDegree()
    {
        IsHighestDegree = true;
    }
    public void SetGraduationDate(DateOnly? graduationDate)
    {
        GraduationDate = graduationDate;
    }

    internal void Update(
        string? degreeTitle,
        string? fieldOfStudy,
        string? institutionName,
        DateOnly? graduationDate)
    {
        var cleanedDegreeTitle = Clean(degreeTitle);
        var cleanedFieldOfStudy = Clean(fieldOfStudy);

        if (cleanedDegreeTitle is null && cleanedFieldOfStudy is null)
        {
            throw new DomainRuleException(
                "حداقل عنوان مدرک یا رشته تحصیلی الزامی است");
        }

        DegreeTitle = cleanedDegreeTitle;
        FieldOfStudy = cleanedFieldOfStudy;
        InstitutionName = Clean(institutionName);
        GraduationDate = graduationDate;
    }
    internal void RemoveHighestDegreeFlag()
    {
        IsHighestDegree = false;
    }
    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }
}
