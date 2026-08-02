using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Application.Features.Employees;
using HrPanel.Application.Features.Employments;
using HrPanel.Application.Features.Lookups;
using HrPanel.Application.Features.Scheduling;
using HrPanel.Domain.Employment;
using HrPanel.UI.Common.Results;
using HrPanel.UI.Ui;
using HrPanel.UI.Models.Employments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class EmploymentsController : Controller
{
    private readonly IEmploymentService _service;
    private readonly IEmployeeService _employeeService;
    private readonly ISchedulingService _schedulingService;
    private readonly ILookupService _lookupService;

    public EmploymentsController(IEmploymentService service,IEmployeeService employeeService,ISchedulingService schedulingService,ILookupService lookupService)
    {
        _service = service;
        _employeeService = employeeService;
        _schedulingService = schedulingService;
        _lookupService = lookupService;
    }

    [HttpGet("/employments")]
    public async Task<IActionResult> Index(string? search, bool? isCurrent, int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetEmploymentsAsync(new GetEmploymentsDto(search, IsCurrent: isCurrent, PageNumber: page), cancellationToken);
        if (result.IsFailure)
        {
            return View("ErrorState", result.Error.Description);
        }
        ViewBag.Search = search; ViewBag.IsCurrent = isCurrent;
        return View(result.Value);
    }

    [HttpGet("/employments/create")]
    public async Task<IActionResult> Create(long? employeeId, string? employeeSearch, CancellationToken cancellationToken)
    {
        await PopulateCreateAsync(employeeSearch, cancellationToken);
        return View(new StartEmploymentViewModel { EmployeeId = employeeId ?? 0, EmployeeSearch = employeeSearch});
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/create")]
    public async Task<IActionResult> Create(StartEmploymentViewModel model, CancellationToken cancellationToken)
    {
        if (!PersianDate.TryParse(model.StartDate, out var startDate)) ModelState.AddModelError(nameof(model.StartDate), "تاریخ شروع معتبر نیست");
        if (!ModelState.IsValid) 
        { 
            await PopulateCreateAsync(model.EmployeeSearch, cancellationToken);
            return View(model);
        }
        var result = await _service.StartEmploymentAsync(new StartEmploymentDto
        {
            EmployeeId = model.EmployeeId,
            EmploymentTypeId = model.EmploymentTypeId,
            EmploymentStatusId = model.EmploymentStatusId,
            WorkTimeTypeId = model.WorkTimeTypeId,
            StartDate = startDate,
            ContractTermMonths = model.ContractTermMonths
        }, cancellationToken);
        if (result.IsFailure)
        {
            result.AddToModelState(ModelState); await PopulateCreateAsync(model.EmployeeSearch, cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "استخدام با موفقیت آغاز شد";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet("/employments/{id:long}")]
    public async Task<IActionResult> Details(long id, string? employeeSearch, CancellationToken cancellationToken)
    {
        var employment = await _service.GetEmploymentAsync(id, cancellationToken);
        if (employment.IsFailure)
        {
            return employment.Error.Type == ErrorType.NotFound ? NotFound() : View("ErrorState", employment.Error.Description);
        }
        var employmentLookups = await _lookupService.GetEmploymentLookupsAsync(cancellationToken);
        var organizationLookups = await _lookupService.GetOrganizationLookupsAsync(cancellationToken);
        var schedulingLookups = await _lookupService.GetSchedulingLookupsAsync(cancellationToken);
        var schedules = await _schedulingService.GetScheduleAssignmentsAsync(id, cancellationToken);
        var employees = await _employeeService.GetEmployeesAsync(new GetEmployeesDto(employeeSearch, true, 1, 100), cancellationToken);
        var external = await _service.GetExternalPersonsAsync(null, true, cancellationToken);
        return View(new EmploymentDetailsViewModel(employment.Value, employmentLookups, organizationLookups, schedulingLookups,
            schedules.IsSuccess ? schedules.Value : [], employees.IsSuccess ? employees.Value.Items : [], external.IsSuccess ? external.Value : []));
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/status")]
    public async Task<IActionResult> ChangeStatusFromView(long id, short employmentStatusId, CancellationToken cancellationToken)
    { 
        var result = await _service.ChangeStatusAsync(id, new ChangeEmploymentStatusDto { EmploymentStatusId = employmentStatusId }, cancellationToken);
        SetMessage(result, "وضعیت استخدام تغییر کرد"); 
        return Back(id);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/work-time")]
    public async Task<IActionResult> ChangeWorkTime(long id, short? workTimeTypeId, CancellationToken cancellationToken)
    { 
        var result = await _service.ChangeWorkTimeTypeAsync(id, new ChangeWorkTimeTypeDto { WorkTimeTypeId = workTimeTypeId }, cancellationToken); 
        SetMessage(result, "نوع ساعت کاری تغییر کرد");
        return Back(id); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/end")]
    public async Task<IActionResult> End(long id, string endDate, short employmentStatusId, string? reason, CancellationToken cancellationToken)
    { 
        if (!TryDate(endDate, out var date)) 
            return BadDate(id); var result = await _service.EndEmploymentAsync(id, new EndEmploymentDto { EndDate = date, EmploymentStatusId = employmentStatusId, Reason = reason }, cancellationToken); 
            SetMessage(result, "استخدام پایان یافت"); 
            return Back(id);
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/assignments")]
    public async Task<IActionResult> AddAssignmentFromView(long id, AssignmentContext context, string effectiveFrom, long? organizationUnitId, long? positionId, short? jobLevelId, long? workLocationId, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveFrom, out var date)) 
            return BadDate(id); var result = await _service.AddAssignmentAsync(id, new AddEmployeeAssignmentDto { Context = context, EffectiveFrom = date, OrganizationUnitId = organizationUnitId, PositionId = positionId, JobLevelId = jobLevelId, WorkLocationId = workLocationId }, cancellationToken); 
            SetMessage(result, "تخصیص سازمانی ثبت شد");
            return Back(id, "assignments");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/assignments/{assignmentId:long}/end")]
    public async Task<IActionResult> EndAssignmentFromView(long id, long assignmentId, string effectiveTo, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveTo, out var date)) 
            return BadDate(id, "assignments"); var result = await _service.EndAssignmentAsync(id, assignmentId, new EndAssignmentDto { EffectiveTo = date }, cancellationToken);
            SetMessage(result, "تخصیص پایان یافت."); return Back(id, "assignments");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/groups")]
    public async Task<IActionResult> AddGroup(long id, long operationalGroupId, string effectiveFrom, bool isPrimary, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveFrom, out var date)) return BadDate(id, "groups"); 
        var result = await _service.AssignOperationalGroupAsync(id, new AssignOperationalGroupDto { OperationalGroupId = operationalGroupId, EffectiveFrom = date, IsPrimary = isPrimary }, cancellationToken);
        SetMessage(result, "گروه عملیاتی تخصیص یافت");
        return Back(id, "groups");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/groups/{assignmentId:long}/primary")]
    public async Task<IActionResult> PrimaryGroup(long id, long assignmentId, CancellationToken cancellationToken)
    {
        var result = await _service.SelectPrimaryOperationalGroupAsync(id, assignmentId, cancellationToken);
        SetMessage(result, "گروه اصلی تغییر کرد");
        return Back(id, "groups");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/groups/{assignmentId:long}/end")]
    public async Task<IActionResult> EndGroup(long id, long assignmentId, string effectiveTo, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveTo, out var date))
            return BadDate(id, "groups"); 
        var result = await _service.EndOperationalGroupAssignmentAsync(id, assignmentId, new EndAssignmentDto { EffectiveTo = date }, cancellationToken);
        SetMessage(result, "تخصیص گروه پایان یافت");
        return Back(id, "groups");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/relationships")]
    public async Task<IActionResult> AddRelationshipFromView(long id, long employeeId, RelationshipType type, RelationshipContext context, long? relatedEmployeeId, long? relatedExternalPersonId, string effectiveFrom, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveFrom, out var date))
            return BadDate(id, "relationships");
            var result = await _service.AddRelationshipAsync(employeeId, new AddEmployeeRelationshipDto { Type = type, Context = context, RelatedEmployeeId = relatedEmployeeId, RelatedExternalPersonId = relatedExternalPersonId, EffectiveFrom = date }, cancellationToken);
            SetMessage(result, "رابطه سازمانی ثبت شد");
            return Back(id, "relationships");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/relationships/{relationshipId:long}/end")]
    public async Task<IActionResult> EndRelationshipFromView(long id, long employeeId, long relationshipId, string effectiveTo, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveTo, out var date))
            return BadDate(id, "relationships");
            var result = await _service.EndRelationshipAsync(employeeId, relationshipId, new EndAssignmentDto { EffectiveTo = date }, cancellationToken);
            SetMessage(result, "رابطه پایان یافت"); 
            return Back(id, "relationships");
    }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/disciplinary")]
    public async Task<IActionResult> AddDisciplinary(long id, long employeeId, string startDate, string? endDate, string details, CancellationToken cancellationToken)
    {
        if (!TryDate(startDate, out var start))
            return BadDate(id, "disciplinary");
        DateOnly? end = null; if (!string.IsNullOrWhiteSpace(endDate)) 
        { 
            if (!TryDate(endDate, out var parsed)) return BadDate(id, "disciplinary"); end = parsed; } 
                var result = await _service.AddDisciplinaryActionAsync(new AddDisciplinaryActionDto { EmployeeId = employeeId, StartDate = start, EndDate = end, Details = details }, cancellationToken);
                SetMessage(result, "اقدام انضباطی ثبت شد"); 
                return Back(id, "disciplinary"); }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/disciplinary/{actionId:long}/close")]
    public async Task<IActionResult> CloseDisciplinary(long id, long employeeId, long actionId, string endDate, CancellationToken cancellationToken)
    {
        if (!TryDate(endDate, out var end)) 
            return BadDate(id, "disciplinary"); 
            var result = await _service.CloseDisciplinaryActionAsync(employeeId, actionId, new CloseDisciplinaryActionDto { EndDate = end }, cancellationToken);
            SetMessage(result, "اقدام انضباطی بسته شد");
            return Back(id, "disciplinary"); }


    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/schedules")]
    public async Task<IActionResult> AssignSchedule(long id, long workScheduleId, string effectiveFrom, short rotationOffsetDays, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveFrom, out var date)) 
            return BadDate(id, "schedules"); 
            var result = await _schedulingService.AssignWorkScheduleAsync(new AssignWorkScheduleDto { EmploymentId = id, WorkScheduleId = workScheduleId, EffectiveFrom = date, RotationOffsetDays = rotationOffsetDays }, cancellationToken);
            SetMessage(result, "برنامه کاری تخصیص یافت"); 
            return Back(id, "schedules"); }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/{id:long}/schedules/{assignmentId:long}/end")]
    public async Task<IActionResult> EndSchedule(long id, long assignmentId, string effectiveTo, CancellationToken cancellationToken)
    {
        if (!TryDate(effectiveTo, out var date))
            return BadDate(id, "schedules");
        var result = await _schedulingService.EndScheduleAssignmentAsync(assignmentId, new EndScheduleAssignmentDto { EffectiveTo = date }, cancellationToken);
        SetMessage(result, "تخصیص برنامه کاری پایان یافت"); 
        return Back(id, "schedules"); }

    [HttpGet("/employments/external-people")]
    public async Task<IActionResult> ExternalPeople(string? search, bool? isActive, CancellationToken cancellationToken)
    {
        var result = await _service.GetExternalPersonsAsync(search, isActive, cancellationToken);
        if (result.IsFailure) return View("ErrorState", result.Error.Description); ViewBag.Search = search; ViewBag.IsActive = isActive;
        return View(result.Value);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/external-people")]
    public async Task<IActionResult> CreateExternalPersonFromView(CreateExternalPersonDto request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateExternalPersonAsync(request, cancellationToken);
        SetMessage(result, "شخص بیرونی ثبت شد");
        return RedirectToAction(nameof(ExternalPeople)); }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/external-people/{externalPersonId:long}/edit")]
    public async Task<IActionResult> EditExternalPerson(long externalPersonId, UpdateExternalPersonDto request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateExternalPersonAsync(externalPersonId, request, cancellationToken); 
        SetMessage(result, "اطلاعات شخص بیرونی ویرایش شد"); return RedirectToAction(nameof(ExternalPeople));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/employments/external-people/{externalPersonId:long}/status")]
    public async Task<IActionResult> ExternalPersonStatus(long externalPersonId, bool isActive, CancellationToken cancellationToken)
    {
        var result = await _service.ChangeExternalPersonStatusAsync(externalPersonId, isActive, cancellationToken);
        SetMessage(result, "وضعیت شخص بیرونی تغییر کرد");
        return RedirectToAction(nameof(ExternalPeople)); 
    }

    private async Task PopulateCreateAsync(string? employeeSearch, CancellationToken cancellationToken)
    { 
        var employees = await _employeeService.GetEmployeesAsync(new GetEmployeesDto(employeeSearch, true, 1, 100), cancellationToken); 
        ViewBag.Employees = employees.IsSuccess ? employees.Value.Items : Array.Empty<EmployeeListItemDto>(); 
        ViewBag.Lookups = await _lookupService.GetEmploymentLookupsAsync(cancellationToken);
    }
    private static bool TryDate(string? value, out DateOnly date) => PersianDate.TryParse(value, out date);
    private IActionResult BadDate(long id, string? tab = null) { TempData["ErrorMessage"] = "تاریخ واردشده معتبر نیست"; return Back(id, tab); }
    private RedirectToActionResult Back(long id, string? tab = null) => RedirectToAction(nameof(Details), new { id, tab });
    private void SetMessage(Result result, string success) 
    { 
        if (result.IsSuccess) TempData["SuccessMessage"] = success; else result.SetFailureMessage(TempData);
    }
}
