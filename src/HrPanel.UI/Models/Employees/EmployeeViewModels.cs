using System.ComponentModel.DataAnnotations;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Employees;

namespace HrPanel.UI.Models.Employees;

public sealed class CreateEmployeeViewModel
{
    [Required(ErrorMessage = "شماره پرسنلی الزامی است")]
    [Display(Name = "شماره پرسنلی")]
    public string EmployeeNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "نام فارسی الزامی است")]
    [Display(Name = "نام")]
    public string FirstNameFa { get; set; } = string.Empty;
    [Required(ErrorMessage = "نام خانوادگی فارسی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastNameFa { get; set; } = string.Empty;
    [Display(Name = "نام انگلیسی")]
    public string? FirstName { get; set; }
    [Display(Name = "نام خانوادگی انگلیسی")]
    public string? LastName { get; set; }
    [RegularExpression("^[0-9۰-۹]{10}$", ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
    [Display(Name = "کد ملی")]
    public string? NationalCode { get; set; }
}

public sealed class EditEmployeeViewModel
{
    public long Id { get; set; }
    [Required] public string EmployeeNumber { get; set; } = string.Empty;
    [Required] public string FirstNameFa { get; set; } = string.Empty;
    [Required] public string LastNameFa { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [RegularExpression("^[0-9۰-۹٠-٩]{10}$", ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
    public string? NationalCode { get; set; }
    [StringLength(EmployeeConstants.NameMaxLength, ErrorMessage = "نام پدر بیش از حد مجاز است")]
    public string? FatherName { get; set; }
    [RegularExpression("^[0-9۰-۹٠-٩]{10}$", ErrorMessage = "کد ملی پدر باید ۱۰ رقم باشد")]
    public string? FatherNationalCode { get; set; }
    public string? BirthDate { get; set; }
    public string? BirthPlace { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
}

public sealed record EmployeeProfileViewModel(EmployeeDetailsDto Employee,IReadOnlyCollection<EmploymentListItemDto> Employments,IReadOnlyCollection<AssetListItemDto> Assets,IReadOnlyCollection<ScheduleAssignmentDto> ScheduleAssignments);
