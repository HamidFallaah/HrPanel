using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Features.Assets;
using HrPanel.Application.Features.Employees;
using HrPanel.Application.Features.Lookups;
using HrPanel.Domain.Assets;
using HrPanel.UI.Common.Results;
using HrPanel.UI.Models.Assets;
using HrPanel.UI.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class AssetsController : Controller
{
    private readonly IAssetService _service;
    private readonly IEmployeeService _employeeService;
    private readonly ILookupService _lookupService;

    public AssetsController(IAssetService service,IEmployeeService employeeService,ILookupService lookupService)
    {
        _service = service;
        _employeeService = employeeService;
        _lookupService = lookupService;
    }

    [HttpGet("/assets")]
    public async Task<IActionResult> Index(string? search, short? assetTypeId, AssetStatus? status, long? employeeId, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetAssetsAsync(new GetAssetsDto(search, assetTypeId, status, employeeId, page, 20), cancellationToken);
        if (result.IsFailure)
        {
            return View("ErrorState", result.Error.Description);
        }
        ViewBag.Search = search; ViewBag.AssetTypeId = assetTypeId; 
        ViewBag.Status = status; ViewBag.EmployeeId = employeeId; ViewBag.Lookups =  await _lookupService.GetAssetLookupsAsync(cancellationToken); 
        return View(result.Value); 
    }

    [HttpGet("/assets/create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) 
    { 
        ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
        return View("Form", new AssetViewModels());
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/create")]
    public async Task<IActionResult> Create(AssetViewModels model, CancellationToken cancellationToken) 
    { 
        if (!ModelState.IsValid) 
        { 
            ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
            return View("Form", model);
        } 
        var result = await _service.CreateAssetAsync(ToCreate(model), cancellationToken); 
        if (result.IsFailure) 
        {
            result.AddToModelState(ModelState); 
            ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
            return View("Form", model); } TempData["SuccessMessage"] = "دارایی ثبت شد"; 
        return RedirectToAction(nameof(Details), new { id = result.Value 
        });
    }

    [HttpGet("/assets/{id:long}/edit")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken) 
    { 
        var result = await _service.GetAssetAsync(id, cancellationToken); 
        if (result.IsFailure) return NotFound(); 
        var x = result.Value; ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
        return View("Form", new AssetViewModels { Id = id, AssetTypeId = x.AssetTypeId, AssetTag = x.AssetTag, ServiceNumber = x.ServiceNumber, Imei = x.Imei, SerialNumber = x.SerialNumber, Notes = x.Notes }); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/edit")]
    public async Task<IActionResult> Edit(long id, AssetViewModels model, CancellationToken cancellationToken) 
    { 
        model.Id = id; 
        if (!ModelState.IsValid) 
        { ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
            return View("Form", model); 
        } 
        var result = await _service.UpdateAssetAsync(id, new UpdateAssetDto { AssetTypeId = model.AssetTypeId, AssetTag = model.AssetTag, ServiceNumber = model.ServiceNumber, Imei = model.Imei, SerialNumber = model.SerialNumber, Notes = model.Notes }, cancellationToken); 
        if (result.IsFailure) 
        {
            result.AddToModelState(ModelState); ViewBag.Lookups = await _lookupService.GetAssetLookupsAsync(cancellationToken); 
            return View("Form", model); } TempData["SuccessMessage"] = "دارایی ویرایش شد"; 
            return RedirectToAction(nameof(Details), new { id }); 
    }
   
    [HttpGet("/assets/{id:long}")]
    public async Task<IActionResult> Details(long id, string? employeeSearch, CancellationToken cancellationToken) 
    { 
        var result = await _service.GetAssetAsync(id, cancellationToken); 
        if (result.IsFailure) return result.Error.Type == ErrorType.NotFound ? NotFound() : View("ErrorState", result.Error.Description); 
            var employees = await _employeeService.GetEmployeesAsync(new GetEmployeesDto(employeeSearch, true, 1, 100), cancellationToken); 
            ViewBag.Employees = employees.IsSuccess ? employees.Value.Items : Array.Empty<EmployeeListItemDto>(); 
            ViewBag.EmployeeSearch = employeeSearch; 
            return View(result.Value);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/assign")]
    public async Task<IActionResult> Assign(long id, long employeeId, string assignedAt, string? notes, CancellationToken cancellationToken) 
    { 
        if (!PersianDate.TryParse(assignedAt, out var date)) 
            return BadDate(id); 
            var result = await _service.AssignAssetAsync(id, new AssignAssetDto { EmployeeId = employeeId, AssignedAt = date, Notes = notes }, cancellationToken); 
            Message(result, "دارایی واگذار شد"); return Back(id);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/return")]
    public async Task<IActionResult> Return(long id, string returnedAt, CancellationToken cancellationToken) 
    { 
        if (!PersianDate.TryParse(returnedAt, out var date)) 
            return BadDate(id); var r = await _service.ReturnAssetAsync(id, new ReturnAssetDto { ReturnedAt = date }, cancellationToken); 
            Message(r, "دارایی بازگردانده شد"); 
            return Back(id); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/maintenance")]
    public async Task<IActionResult> Maintenance(long id, CancellationToken cancellationToken) 
    { 
        var result = await _service.SendToMaintenanceAsync(id, cancellationToken); 
        Message(result, "دارایی به وضعیت در تعمیر منتقل شد"); 
        return Back(id); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/retire")]
    public async Task<IActionResult> Retire(long id, CancellationToken cancellationToken) 
    { 
        var r = await _service.RetireAssetAsync(id, cancellationToken); Message(r, "دارایی از رده خارج شد"); 
        return Back(id);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/assets/{id:long}/lost")]
    public async Task<IActionResult> Lost(long id, CancellationToken cancellationToken) 
    { 
        var result = await _service.MarkAssetAsLostAsync(id, cancellationToken); 
        Message(result, "دارایی به‌عنوان مفقود ثبت شد"); 
        return Back(id); 
    }
    private static CreateAssetDto ToCreate(AssetViewModels x) => new() 
    { 
        AssetTypeId = x.AssetTypeId, AssetTag = x.AssetTag, ServiceNumber = x.ServiceNumber, Imei = x.Imei, SerialNumber = x.SerialNumber, Notes = x.Notes };
    private IActionResult BadDate(long id) 
    { 
        TempData["ErrorMessage"] = "تاریخ معتبر نیست"; 
        return Back(id); 
    }
    private RedirectToActionResult Back(long id) => RedirectToAction(nameof(Details), new { id });
    private void Message(Result result, string text) 
    { 
        if (result.IsSuccess) TempData["SuccessMessage"] = text; 
        else result.SetFailureMessage(TempData);
    }
}
