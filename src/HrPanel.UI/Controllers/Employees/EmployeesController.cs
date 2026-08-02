using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Application.Features.Assets;
using HrPanel.Application.Features.Employees;
using HrPanel.Application.Features.Employments;
using HrPanel.Application.Features.Lookups;
using HrPanel.Application.Features.Scheduling;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Employees;
using HrPanel.UI.Common.Results;
using HrPanel.UI.Ui;
using HrPanel.UI.Models.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmploymentService _employmentService;
    private readonly IAssetService _assetService;
    private readonly ISchedulingService _schedulingService;
    private readonly IEmployeeLookupService _employeeLookupService;

    public EmployeesController(IEmployeeService employeeService,IEmploymentService employmentService,IAssetService assetService,ISchedulingService schedulingService,IEmployeeLookupService employeeLookupService)
    {
        _employeeService = employeeService;
        _employmentService = employmentService;
        _assetService = assetService;
        _schedulingService = schedulingService;
        _employeeLookupService = employeeLookupService;
    }

    [HttpGet("/employees")]
    public async Task<IActionResult> Index(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetEmployeesAsync(new GetEmployeesDto(search, isActive, page, 20), cancellationToken);
        if (result.IsFailure) return View("ErrorState", result.Error.Description);
        ViewBag.Search = search; ViewBag.IsActive = isActive;
        return View(result.Value);
    }

    [HttpGet("/employees/create")]
    public IActionResult Create()
    {
         return View(new CreateEmployeeViewModel());
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/create")]
    public async Task<IActionResult> Create(CreateEmployeeViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _employeeService.CreateEmployeeAsync(new CreateEmployeeDto(
            model.EmployeeNumber, model.FirstNameFa, model.LastNameFa, model.FirstName, model.LastName, NormalizeDigits(model.NationalCode)), cancellationToken);
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); return View(model);
        }
        TempData["SuccessMessage"] = "کارمند با موفقیت ثبت شد";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet("/employees/{id:long}")]
    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetEmployeeDetailsAsync(id, cancellationToken);
        if (employee.IsFailure)
        {
            return employee.Error.Type == ErrorType.NotFound ? NotFound() : View("ErrorState", employee.Error.Description);
        }
        var employments = await _employmentService.GetEmploymentsAsync(new GetEmploymentsDto(EmployeeId: id, PageSize: 100), cancellationToken);
        var assets = await _assetService.GetAssetsAsync(new GetAssetsDto(EmployeeId: id, PageSize: 100), cancellationToken);
        var schedules = new List<ScheduleAssignmentDto>();
        if (employments.IsSuccess)
        {
            foreach (var employment in employments.Value.Items)
            {
                var schedule = await _schedulingService.GetScheduleAssignmentsAsync(employment.Id, cancellationToken);
                if (schedule.IsSuccess) schedules.AddRange(schedule.Value);
            }
        }
        ViewBag.Lookups = _employeeLookupService.GetEmployeeLookups();
        return View(new EmployeeProfileViewModel(employee.Value, employments.IsSuccess ? employments.Value.Items : [], assets.IsSuccess ? assets.Value.Items : [], schedules));
    }

    [HttpGet("/employees/{id:long}/edit")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetEmployeeDetailsAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound();
        }
        var p = result.Value.PersonalDetails;
        return View(new EditEmployeeViewModel
        {
            Id = id,
            EmployeeNumber = result.Value.EmployeeNumber,
            FirstNameFa = p?.FirstNameFa ?? string.Empty,
            LastNameFa = p?.LastNameFa ?? string.Empty,
            FirstName = p?.FirstName,
            LastName = p?.LastName,
            NationalCode = p?.NationalCode,
            FatherName = p?.FatherName,
            FatherNationalCode = p?.FatherNationalCode,
            BirthDate = p?.BirthDate is null ? null : PersianDate.Format(p.BirthDate),
            BirthPlace = p?.BirthPlace,
            Gender = (Gender)(p?.GenderId ?? 0),
            MaritalStatus = (MaritalStatus)(p?.MaritalStatusId ?? 0)
        });
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/edit")]
    public async Task<IActionResult> Edit(long id, EditEmployeeViewModel model, CancellationToken cancellationToken)
    {
        model.Id = id;
        DateOnly? birthDate = null;
        if (!string.IsNullOrWhiteSpace(model.BirthDate))
        {
            if (PersianDate.TryParse(model.BirthDate, out var parsed))
            {
                birthDate = parsed;
            }
            else
            {
                ModelState.AddModelError(nameof(model.BirthDate), "تاریخ تولد معتبر نیست");
            }
        }

        if (!ModelState.IsValid) return View(model);
        var result = await _employeeService.UpdatePersonalDetailsAsync(id, new UpdateEmployeePersonalDetailsDto
        {
            FirstNameFa = model.FirstNameFa,
            LastNameFa = model.LastNameFa,
            FirstName = model.FirstName,
            LastName = model.LastName,
            NationalCode = NormalizeDigits(model.NationalCode),
            FatherName = model.FatherName,
            FatherNationalCode = NormalizeDigits(model.FatherNationalCode),
            BirthDate = birthDate,
            BirthPlace = model.BirthPlace,
            Gender = model.Gender,
            MaritalStatus = model.MaritalStatus
        }, cancellationToken);
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); 
            return View(model); 
        }
        TempData["SuccessMessage"] = "اطلاعات کارمند به‌روزرسانی شد";
        return RedirectToAction(nameof(Details), new { id });
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/number")]
    public async Task<IActionResult> UpdateNumber(long id, string employeeNumber, CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateEmployeeNumberAsync(id, new UpdateEmployeeNumberDto { EmployeeNumber = employeeNumber }, cancellationToken);
        SetMessage(result, "شماره پرسنلی به‌روزرسانی شد");
        return RedirectToAction(nameof(Edit), new { id });
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/status")]
    public async Task<IActionResult> ChangeStatus(long id, bool isActive, CancellationToken cancellationToken)
    {
        var result = isActive ? await _employeeService.ActivateEmployeeAsync(id, cancellationToken) : await _employeeService.DeactivateEmployeeAsync(id, cancellationToken);
        SetMessage(result, isActive ? "کارمند فعال شد" : "کارمند غیرفعال شد");
        return RedirectToAction(nameof(Details), new { id });
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/contacts")]
    public async Task<IActionResult> AddContactFromView(long id, AddEmployeeContactDto request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.AddContactAsync(id, request, cancellationToken); SetMessage(result, "راه ارتباطی ثبت شد");
        return RedirectToDetails(id, "contacts");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/contacts/{contactId:long}/primary")]
    public async Task<IActionResult> PrimaryContact(long id, long contactId, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.SelectPrimaryContactAsync(id, contactId, cancellationToken); SetMessage(result, "راه ارتباطی اصلی تغییر کرد"); 
        return RedirectToDetails(id, "contacts"); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/contacts/{contactId:long}/edit")]
    public async Task<IActionResult> EditContact(long id, long contactId, string value, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.UpdateContactAsync(id, contactId, new UpdateEmployeeContactDto { Value = value }, cancellationToken); 
        SetMessage(result, "راه ارتباطی ویرایش شد"); return RedirectToDetails(id, "contacts");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/contacts/{contactId:long}/remove")]
    public async Task<IActionResult> RemoveContactFromView(long id, long contactId, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.RemoveContactAsync(id, contactId, cancellationToken); 
        SetMessage(result, "راه ارتباطی حذف شد"); return RedirectToDetails(id, "contacts");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/identifiers")]
    public async Task<IActionResult> AddIdentifierFromView(long id, IdentifierType type, string value, string? effectiveFrom, CancellationToken cancellationToken)
    {
        DateOnly? date = null; 
        if (!string.IsNullOrWhiteSpace(effectiveFrom)) 
        { 
            if (!PersianDate.TryParse(effectiveFrom, out var parsed))
            {
                return InvalidDate(id, "identifiers"); date = parsed;
            }               
        }
        var result = await _employeeService.AddIdentifierAsync(id, new AddEmployeeIdentifierDto { Type = type, Value = value, EffectiveFrom = date }, cancellationToken);
        SetMessage(result, "شناسه ثبت شد."); 
        return RedirectToDetails(id, "identifiers");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/identifiers/{identifierId:long}/end")]
    public async Task<IActionResult> EndIdentifierFromView(long id, long identifierId, string effectiveTo, CancellationToken cancellationToken)
    { 
        if (!PersianDate.TryParse(effectiveTo, out var date)) 
        { 
            TempData["ErrorMessage"] = "تاریخ پایان معتبر نیست"; 
            return RedirectToDetails(id, "identifiers"); 
        } 
        var result = await _employeeService.EndIdentifierAsync(id, identifierId, new EndEmployeeIdentifierDto { EffectiveTo = date }, cancellationToken); 
        SetMessage(result, "شناسه پایان یافت"); return RedirectToDetails(id, "identifiers");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/education")]
    public async Task<IActionResult> AddEducationFromView(long id, string? degreeTitle, string? fieldOfStudy, string? institutionName, string? graduationDate, bool isHighestDegree, CancellationToken cancellationToken)
    {
        DateOnly? date = null; 
        if (!string.IsNullOrWhiteSpace(graduationDate)) 
        { 
            if (!PersianDate.TryParse(graduationDate, out var parsed))
            {
                return InvalidDate(id, "education"); date = parsed;
            }
                 
        } 
        var result = await _employeeService.AddEducationAsync(id, new AddEmployeeEducationDto { DegreeTitle = degreeTitle, FieldOfStudy = fieldOfStudy, InstitutionName = institutionName, GraduationDate = date, IsHighestDegree = isHighestDegree }, cancellationToken);
        SetMessage(result, "سابقه تحصیلی ثبت شد"); 
        return RedirectToDetails(id, "education"); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/education/{educationId:long}/highest")]
    public async Task<IActionResult> HighestEducation(long id, long educationId, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.SelectHighestEducationAsync(id, educationId, cancellationToken); 
        SetMessage(result, "بالاترین مدرک تعیین شد"); 
        return RedirectToDetails(id, "education"); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/education/{educationId:long}/edit")]
    public async Task<IActionResult> EditEducation(long id, long educationId, string? degreeTitle, string? fieldOfStudy, string? institutionName, string? graduationDate, bool isHighestDegree, CancellationToken cancellationToken)
    { DateOnly? date = null;
        if (!string.IsNullOrWhiteSpace(graduationDate)) 
        { 
            if (!PersianDate.TryParse(graduationDate, out var parsed))
            {
                return InvalidDate(id, "education"); date = parsed;
            }
        }         
        var result = await _employeeService.UpdateEducationAsync(id, educationId, new AddEmployeeEducationDto { DegreeTitle = degreeTitle, FieldOfStudy = fieldOfStudy, InstitutionName = institutionName, GraduationDate = date, IsHighestDegree = isHighestDegree }, cancellationToken);
        SetMessage(result, "سابقه تحصیلی ویرایش شد"); return RedirectToDetails(id, "education"); }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/education/{educationId:long}/remove")]
    public async Task<IActionResult> RemoveEducationFromView(long id, long educationId, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.RemoveEducationAsync(id, educationId, cancellationToken); 
        SetMessage(result, "سابقه تحصیلی حذف شد"); return RedirectToDetails(id, "education");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/dependents")]
    public async Task<IActionResult> AddDependentFromView(long id, string fullName, string? nationalCode, string? birthDate, DependentRelationshipType relationshipType, bool isEmergencyContact, string? emergencyPhone, CancellationToken cancellationToken)
    { DateOnly? date = null; if (!string.IsNullOrWhiteSpace(birthDate)) 
        { 
            if (!PersianDate.TryParse(birthDate, out var parsed)) 
                return InvalidDate(id, "dependents"); date = parsed; 
        } 
        var result = await _employeeService.AddDependentAsync(id, new AddEmployeeDependentDto { FullName = fullName, NationalCode = NormalizeDigits(nationalCode), BirthDate = date, RelationshipType = relationshipType, IsEmergencyContact = isEmergencyContact, EmergencyPhone = emergencyPhone }, cancellationToken);
        SetMessage(result, "فرد تحت تکفل ثبت شد"); return RedirectToDetails(id, "dependents"); }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/dependents/{dependentId:long}/remove")]
    public async Task<IActionResult> RemoveDependentFromView(long id, long dependentId, CancellationToken cancellationToken)
    { 
        var result = await _employeeService.RemoveDependentAsync(id, dependentId, cancellationToken); SetMessage(result, "فرد تحت تکفل حذف شد");
        return RedirectToDetails(id, "dependents");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employees/{id:long}/dependents/{dependentId:long}/edit")]
    public async Task<IActionResult> EditDependent(long id, long dependentId, string fullName, string? nationalCode, string? birthDate, DependentRelationshipType relationshipType, bool isEmergencyContact, string? emergencyPhone, CancellationToken cancellationToken)
    { 
        DateOnly? date = null; 
        if (!string.IsNullOrWhiteSpace(birthDate)) 
        { 
            if (!PersianDate.TryParse(birthDate, out var parsed)) 
                return InvalidDate(id, "dependents"); date = parsed; } var result = await _employeeService.UpdateDependentAsync(id, dependentId, new AddEmployeeDependentDto { FullName = fullName, NationalCode = NormalizeDigits(nationalCode), BirthDate = date, RelationshipType = relationshipType, IsEmergencyContact = isEmergencyContact, EmergencyPhone = emergencyPhone }, cancellationToken); 
                SetMessage(result, "اطلاعات فرد تحت تکفل ویرایش شد"); 
                return RedirectToDetails(id, "dependents");
    }

    private RedirectToActionResult RedirectToDetails(long id, string tab) => RedirectToAction(nameof(Details), new { id, tab });
    private RedirectToActionResult InvalidDate(long id, string tab) 
    { 
        TempData["ErrorMessage"] = "تاریخ واردشده معتبر نیست"; return RedirectToDetails(id, tab);
    }
    private void SetMessage(Result result, string success) 
    {
        if (result.IsSuccess) TempData["SuccessMessage"] = success; else result.SetFailureMessage(TempData);
    }
    private static string? NormalizeDigits(string? value) 
    {
        if (string.IsNullOrWhiteSpace(value)) 
            return value; const string fa = "۰۱۲۳۴۵۶۷۸۹"; const string ar = "٠١٢٣٤٥٦٧٨٩"; 
            var result = value.Trim(); for (var i = 0; i < 10; i++) result = result.Replace(fa[i], (char)('0' + i)).Replace(ar[i], (char)('0' + i)); 
            return result;
    }
}
