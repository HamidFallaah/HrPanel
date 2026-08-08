using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Application.Features.Lookups;
using HrPanel.Application.Features.Organization;
using HrPanel.UI.Common.Results;
using HrPanel.UI.Models.Organization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class OrganizationController : Controller
{
    private readonly IOrganizationService _service;
    private readonly ILookupService _lookupService;

    public OrganizationController(IOrganizationService service,ILookupService lookupService)
    {
        _service = service;
        _lookupService = lookupService;
    }

    [HttpGet("/organization/units")]
    public async Task<IActionResult> Units(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetOrganizationUnitsAsync(new GetOrganizationUnitsDto(search, isActive, PageNumber: page), cancellationToken);
        if (result.IsFailure) return View("ErrorState", result.Error.Description);
        var tree = await _service.GetOrganizationTreeAsync(true, cancellationToken);
        ViewBag.Search = search; ViewBag.IsActive = isActive;
        ViewBag.Tree = tree.IsSuccess ? tree.Value : Array.Empty<OrganizationUnitTreeDto>();
        return View(result.Value);
    }

    [HttpGet("/organization/units/create")]
    public async Task<IActionResult> CreateUnitFromView(CancellationToken cancellationToken) 
    {
        await UnitLookups(cancellationToken);
        return View("UnitForm", new OrganizationUnitFormViewModel());
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/units/create")]
    public async Task<IActionResult> CreateUnitFromView(OrganizationUnitFormViewModel model, CancellationToken cancellationToken) 
    { 
        if (!ModelState.IsValid) 
        {
            await UnitLookups(cancellationToken); return View("UnitForm", model);
        } 
        var result = await _service.CreateOrganizationUnitAsync(new CreateOrganizationUnitDto { OrganizationUnitTypeId = model.OrganizationUnitTypeId, Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn, ParentOrganizationUnitId = model.ParentOrganizationUnitId }, cancellationToken);
        if (result.IsFailure) 
        {
            result.AddToModelState(ModelState); 
            await UnitLookups(cancellationToken); 
            return View("UnitForm", model); 
        } 
        TempData["SuccessMessage"] = "واحد سازمانی ثبت شد";
        return RedirectToAction(nameof(Units));
    }

    [HttpGet("/organization/units/{id:long}/edit")]
    public async Task<IActionResult> EditUnit(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetOrganizationUnitAsync(id, cancellationToken); if (result.IsFailure) 
            return NotFound(); 
        await UnitLookups(cancellationToken);
        var x = result.Value; 
        return View("UnitForm", new OrganizationUnitFormViewModel { Id = id, OrganizationUnitTypeId = x.OrganizationUnitTypeId, Code = x.Code, NameFa = x.NameFa, NameEn = x.NameEn, ParentOrganizationUnitId = x.ParentOrganizationUnitId });
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/units/{id:long}/edit")]
    public async Task<IActionResult> EditUnit(long id, OrganizationUnitFormViewModel model, CancellationToken cancellationToken) { model.Id = id; if (!ModelState.IsValid)
        { 
            await UnitLookups(cancellationToken); return View("UnitForm", model);
        } 
        var result = await _service.UpdateOrganizationUnitAsync(id, new UpdateOrganizationUnitDto { OrganizationUnitTypeId = model.OrganizationUnitTypeId, Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn }, cancellationToken); 
        if (result.IsSuccess) 
            result = await _service.MoveOrganizationUnitAsync(id, new MoveOrganizationUnitDto { ParentOrganizationUnitId = model.ParentOrganizationUnitId }, cancellationToken); 
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); await UnitLookups(cancellationToken); 
            return View("UnitForm", model); 
        } 
        TempData["SuccessMessage"] = "واحد سازمانی ویرایش شد"; 
            return RedirectToAction(nameof(Units)); }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/units/{id:long}/move")]
    public async Task<IActionResult> MoveUnitFromView(long id, long? parentOrganizationUnitId, CancellationToken cancellationToken) 
    { 
        var result = await _service.MoveOrganizationUnitAsync(id, new MoveOrganizationUnitDto { ParentOrganizationUnitId = parentOrganizationUnitId }, cancellationToken);
        Message(result, "واحد جابه‌جا شد"); 
        return RedirectToAction(nameof(Units));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/units/{id:long}/status")]
    public async Task<IActionResult> UnitStatus(long id, bool isActive, CancellationToken cancellationToken) 
    { 
        var result = await _service.ChangeOrganizationUnitStatusAsync(id, isActive, cancellationToken); 
        Message(result, "وضعیت واحد تغییر کرد"); return RedirectToAction(nameof(Units)); 
    }

    [HttpGet("/organization/positions")]
    public async Task<IActionResult> Positions(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetPositionsAsync(new GetOrganizationItemsDto(search, isActive, page, 20), cancellationToken); 
        ViewBag.Search = search; ViewBag.IsActive = isActive; return result.IsSuccess ? View(result.Value) : View("ErrorState", result.Error.Description);
    }

    [HttpGet("/organization/locations")]
    public async Task<IActionResult> Locations(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetWorkLocationsAsync(new GetOrganizationItemsDto(search, isActive, page, 20), cancellationToken); 
        ViewBag.Search = search; ViewBag.IsActive = isActive; return result.IsSuccess ? View(result.Value) : View("ErrorState", result.Error.Description);
    }
    [HttpGet("/organization/groups")]
    public async Task<IActionResult> Groups(string? search, bool? isActive, int page = 1, CancellationToken cancellationToken = default) 
    { 
        var result = await _service.GetOperationalGroupsAsync(new GetOrganizationItemsDto(search, isActive, page, 20), cancellationToken); 
        ViewBag.Search = search; 
        ViewBag.IsActive = isActive;
        return result.IsSuccess ? View(result.Value) : View("ErrorState", result.Error.Description);
    }

    [HttpGet("/organization/{section:regex(^positions|locations|groups$)}/create")]
    public IActionResult CreateItem(string section) => View("ItemForm", new OrganizationItemFormViewModel { Section = section });

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/{section:regex(^positions|locations|groups$)}/create")]
    public async Task<IActionResult> CreateItem(string section, OrganizationItemFormViewModel model, CancellationToken cancellationToken) 
    { 
        model.Section = section; 
        if (!ModelState.IsValid) 
            return View("ItemForm", model); 
            Result<long> result = section switch { "positions" => await _service.CreatePositionAsync(new SavePositionDto { Code = model.Code, TitleFa = model.NameFa, TitleEn = model.NameEn }, cancellationToken), "locations" => 
            
            await _service.CreateWorkLocationAsync(new SaveWorkLocationDto { Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn, Province = model.Province, City = model.City, Address = model.Address }, cancellationToken), 
            
            _ => await _service.CreateOperationalGroupAsync(new CreateOperationalGroupDto { Code = model.Code, Name = model.NameFa, Type = model.GroupType }, cancellationToken) }; 
        if (result.IsFailure) 
        {
            result.AddToModelState(ModelState); 
            return View("ItemForm", model); 
        } 
        TempData["SuccessMessage"] = "رکورد سازمانی ثبت شد"; return RedirectToSection(section); 
    }
    
    [HttpGet("/organization/{section:regex(^positions|locations|groups$)}/{id:long}/edit")]
    public async Task<IActionResult> EditItem(string section, long id, CancellationToken cancellationToken) 
    { 
        var model = new OrganizationItemFormViewModel { Id = id, Section = section }; 
        if (section == "positions") 
        { 
            var r = await _service.GetPositionAsync(id, cancellationToken); 
            if (r.IsFailure) return NotFound(); 
            model.Code = r.Value.Code; model.NameFa = r.Value.TitleFa; model.NameEn = r.Value.TitleEn; 
        } 
        else if (section == "locations") 
        { 
            var r = await _service.GetWorkLocationAsync(id, cancellationToken); 
            if (r.IsFailure) return NotFound(); model.Code = r.Value.Code; model.NameFa = r.Value.NameFa; model.NameEn = r.Value.NameEn; model.Province = r.Value.Province; model.City = r.Value.City; model.Address = r.Value.Address; 
        } 
        else 
        { 
            var r = await _service.GetOperationalGroupAsync(id, cancellationToken); 
            if (r.IsFailure) return NotFound(); model.Code = r.Value.Code; model.NameFa = r.Value.Name; model.GroupType = r.Value.Type; 
        } 
        return View("ItemForm", model); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/{section:regex(^positions|locations|groups$)}/{id:long}/edit")]
    public async Task<IActionResult> EditItem(string section, long id, OrganizationItemFormViewModel model, CancellationToken cancellationToken) 
    { 
        model.Id = id; model.Section = section; 
        if (!ModelState.IsValid) 
            return View("ItemForm", model); 
        Result result = section switch 
        { 
            "positions" => await _service.UpdatePositionAsync(id, new SavePositionDto { Code = model.Code, TitleFa = model.NameFa, TitleEn = model.NameEn }, cancellationToken), 
            "locations" => await _service.UpdateWorkLocationAsync(id, new SaveWorkLocationDto { Code = model.Code, NameFa = model.NameFa, NameEn = model.NameEn, Province = model.Province, City = model.City, Address = model.Address }, cancellationToken), 
            _ =>  await _service.UpdateOperationalGroupAsync(id, new UpdateOperationalGroupDto { Name = model.NameFa, Type = model.GroupType }, cancellationToken) 
        }; 
        if (result.IsFailure) 
        { 
            result.AddToModelState(ModelState); return View("ItemForm", model); 
        } 
        TempData["SuccessMessage"] = "رکورد سازمانی ویرایش شد"; return RedirectToSection(section); 
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/organization/{section:regex(^positions|locations|groups$)}/{id:long}/status")]
    public async Task<IActionResult> ItemStatus(string section, long id, bool isActive, CancellationToken cancellationToken) 
    { 
        Result result = section switch 
        { 
            "positions" => await _service.ChangePositionStatusAsync(id, isActive, cancellationToken), 
            "locations" =>  await _service.ChangeWorkLocationStatusAsync(id, isActive, cancellationToken), 
            _ => await _service.ChangeOperationalGroupStatusAsync(id, isActive, cancellationToken) 
        }; 
            Message(result, "وضعیت تغییر کرد"); return RedirectToSection(section); 
    }

    private async Task UnitLookups(CancellationToken token) => ViewBag.Lookups = await _lookupService.GetOrganizationLookupsAsync(token);
    private RedirectToActionResult RedirectToSection(string section) => RedirectToAction(section switch { "positions" => nameof(Positions), "locations" => nameof(Locations), _ => nameof(Groups) });
    private void Message(Result result, string text) { if (result.IsSuccess) TempData["SuccessMessage"] = text; else result.SetFailureMessage(TempData); }
}
