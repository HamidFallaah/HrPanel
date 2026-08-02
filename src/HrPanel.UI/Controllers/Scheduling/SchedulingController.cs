using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Application.Features.Lookups;
using HrPanel.Application.Features.Scheduling;
using HrPanel.UI.Common.Results;
using HrPanel.UI.Ui;
using HrPanel.UI.Models.Scheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class SchedulingController : Controller
{
    private readonly ISchedulingService _service;
    private readonly ILookupService _lookupService;

    public SchedulingController(ISchedulingService service,ILookupService lookupService)
    {
        _service = service;
        _lookupService = lookupService;
    }

    [HttpGet("/scheduling/shifts")]
    public async Task<IActionResult> Shifts(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetShiftsAsync(new GetSchedulingItemsDto(search, isActive, page, 20), cancellationToken); 
        ViewBag.Search = search; ViewBag.IsActive = isActive; return result.IsSuccess ? View(result.Value) : View("ErrorState", result.Error.Description); 
    }
   
    [HttpGet("/scheduling/shifts/create")]
    public IActionResult CreateShift() => View("ShiftForm", new ShiftFormViewModel());

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/shifts/create")]
    public async Task<IActionResult> CreateShift(ShiftFormViewModel model, CancellationToken cancellationToken) 
    { 
        if (!ModelState.IsValid) return View("ShiftForm", model); 
        var result = await _service.CreateShiftAsync(ToDto(model), cancellationToken); 
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); return View("ShiftForm", model); 
        } TempData["SuccessMessage"] = "شیفت ثبت شد"; return RedirectToAction(nameof(Shifts)); 
    }
    
    [HttpGet("/scheduling/shifts/{id:long}/edit")]
    public async Task<IActionResult> EditShift(long id, CancellationToken cancellationToken) 
    {
        var result = await _service.GetShiftAsync(id, cancellationToken); 
        if (result.IsFailure) return NotFound();
        var x = result.Value; return View("ShiftForm", new ShiftFormViewModel { Id = id, Code = x.Code, NameFa = x.NameFa, NameEn = x.NameEn, StartTime = x.StartTime, EndTime = x.EndTime, WorkHours = x.WorkHours }); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/shifts/{id:long}/edit")]
    public async Task<IActionResult> EditShift(long id, ShiftFormViewModel model, CancellationToken cancellationToken) 
    { 
        model.Id = id; 
        if (!ModelState.IsValid) return View("ShiftForm", model); 
        var result = await _service.UpdateShiftAsync(id, ToDto(model), cancellationToken); 
        if (result.IsFailure) 
        {
            result.AddToModelState(ModelState); return View("ShiftForm", model); 
        } 
        TempData["SuccessMessage"] = "شیفت ویرایش شد."; return RedirectToAction(nameof(Shifts)); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/shifts/{id:long}/status")]
    public async Task<IActionResult> ShiftStatus(long id, bool isActive, CancellationToken cancellationToken) 
    { 
        var result = await _service.ChangeShiftStatusAsync(id, isActive, cancellationToken); Message(result, "وضعیت شیفت تغییر کرد."); return RedirectToAction(nameof(Shifts)); 
    }

    [HttpGet("/scheduling/schedules")]
    public async Task<IActionResult> Schedules(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetWorkSchedulesAsync(new GetSchedulingItemsDto(search, isActive, page, 20), cancellationToken); ViewBag.Search = search; ViewBag.IsActive = isActive; 
        return result.IsSuccess ? View(result.Value) : View("ErrorState", result.Error.Description); 
    }
   
    [HttpGet("/scheduling/schedules/create")]
    public IActionResult CreateSchedule() => View("ScheduleForm", new ScheduleFormViewModel());

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/schedules/create")]
    public async Task<IActionResult> CreateSchedule(ScheduleFormViewModel model, CancellationToken cancellationToken) 
    { 
        var anchor = ParseOptional(model.AnchorDate, ModelState, nameof(model.AnchorDate)); if (!ModelState.IsValid) 
            return View("ScheduleForm", model); 
        var result = await _service.CreateWorkScheduleAsync(new CreateWorkScheduleDto { Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn, PatternType = model.PatternType, CycleLengthDays = model.CycleLengthDays, AnchorDate = anchor, Days = [] }, cancellationToken); 
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); return View("ScheduleForm", model); 
        } 
        TempData["SuccessMessage"] = "برنامه کاری ثبت شد؛ اکنون روزهای چرخه را تکمیل کنید"; 
        return RedirectToAction(nameof(ScheduleDetails), new { id = result.Value }); 
    }
   
    [HttpGet("/scheduling/schedules/{id:long}/edit")]
    public async Task<IActionResult> EditSchedule(long id, CancellationToken cancellationToken) 
    { 
        var result = await _service.GetWorkScheduleAsync(id, cancellationToken); 
        if (result.IsFailure) return NotFound(); var x = result.Value; 
        return View("ScheduleForm", new ScheduleFormViewModel { Id = id, Code = x.Code, NameFa = x.NameFa, NameEn = x.NameEn, PatternType = x.PatternType, CycleLengthDays = x.CycleLengthDays, AnchorDate = x.AnchorDate.HasValue ? PersianDate.Format(x.AnchorDate) : null }); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/schedules/{id:long}/edit")]
    public async Task<IActionResult> EditSchedule(long id, ScheduleFormViewModel model, CancellationToken cancellationToken) 
    { 
        model.Id = id; var anchor = ParseOptional(model.AnchorDate, ModelState, nameof(model.AnchorDate)); 
        if (!ModelState.IsValid) return View("ScheduleForm", model); 
        var result = await _service.UpdateWorkScheduleAsync(id, new UpdateWorkScheduleDto { Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn, PatternType = model.PatternType, CycleLengthDays = model.CycleLengthDays, AnchorDate = anchor }, cancellationToken); 
        if (result.IsFailure) { result.AddToModelState(ModelState); 
            return View("ScheduleForm", model); } TempData["SuccessMessage"] = "برنامه کاری ویرایش شد"; 
        return RedirectToAction(nameof(ScheduleDetails), new { id }); 
    }
   
    [HttpGet("/scheduling/schedules/{id:long}")]
    public async Task<IActionResult> ScheduleDetails(long id, CancellationToken cancellationToken) 
    { 
        var result = await _service.GetWorkScheduleAsync(id, cancellationToken); 
        if (result.IsFailure) return NotFound(); ViewBag.Lookups = await _lookupService.GetSchedulingLookupsAsync(cancellationToken); 
        return View(result.Value); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/schedules/{id:long}/days")]
    public async Task<IActionResult> SetDay(long id, short dayIndex, long? shiftId, bool isRestDay, CancellationToken cancellationToken) 
    { 
        var result = await _service.SetWorkScheduleDayAsync(id, new SetWorkScheduleDayDto { DayIndex = dayIndex, ShiftId = isRestDay ? null : shiftId, IsRestDay = isRestDay }, cancellationToken); 
        Message(result, "روز برنامه ذخیره شد"); 
        return RedirectToAction(nameof(ScheduleDetails), new { id }); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/schedules/{id:long}/days/{dayIndex:int}/remove")]
    public async Task<IActionResult> RemoveDay(long id, short dayIndex, CancellationToken cancellationToken) 
    { 
        var result = await _service.RemoveWorkScheduleDayAsync(id, dayIndex, cancellationToken); 
        Message(result, "روز برنامه حذف شد"); return RedirectToAction(nameof(ScheduleDetails), new { id }); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/scheduling/schedules/{id:long}/status")]
    public async Task<IActionResult> ScheduleStatus(long id, bool isActive, CancellationToken cancellationToken) 
    {   var result = await _service.ChangeWorkScheduleStatusAsync(id, isActive, cancellationToken); 
        Message(result, "وضعیت برنامه کاری تغییر کرد"); return RedirectToAction(nameof(ScheduleDetails), new { id }); 
    }

    private static SaveShiftDto ToDto(ShiftFormViewModel x) => new() { Code = x.Code, NameFa = x.NameFa, NameEn = x.NameEn, StartTime = x.StartTime, EndTime = x.EndTime, WorkHours = x.WorkHours };
    private static DateOnly? ParseOptional(string? value, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state, string key) 
    { 
        if (string.IsNullOrWhiteSpace(value)) return null; 
        if (PersianDate.TryParse(value, out var date)) return date; 
        state.AddModelError(key, "تاریخ معتبر نیست"); return null; 
    }
    private void Message(Result r, string text) 
    { 
        if (r.IsSuccess) TempData["SuccessMessage"] = text; else r.SetFailureMessage(TempData); 
    }
}
