using FluentValidation;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Domain.Organization;

namespace HrPanel.Application.Features.Organization;

public sealed class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;
    private readonly IValidator<GetOrganizationUnitsDto> _unitsQueryValidator;
    private readonly IValidator<GetOrganizationItemsDto> _itemsQueryValidator;
    private readonly IValidator<CreateOrganizationUnitDto> _createUnitValidator;
    private readonly IValidator<UpdateOrganizationUnitDto> _updateUnitValidator;
    private readonly IValidator<MoveOrganizationUnitDto> _moveUnitValidator;
    private readonly IValidator<SavePositionDto> _positionValidator;
    private readonly IValidator<SaveWorkLocationDto> _locationValidator;
    private readonly IValidator<CreateOperationalGroupDto> _createGroupValidator;
    private readonly IValidator<UpdateOperationalGroupDto> _updateGroupValidator;

    public OrganizationService(
        IOrganizationRepository repository,
        IValidator<GetOrganizationUnitsDto> unitsQueryValidator,
        IValidator<GetOrganizationItemsDto> itemsQueryValidator,
        IValidator<CreateOrganizationUnitDto> createUnitValidator,
        IValidator<UpdateOrganizationUnitDto> updateUnitValidator,
        IValidator<MoveOrganizationUnitDto> moveUnitValidator,
        IValidator<SavePositionDto> positionValidator,
        IValidator<SaveWorkLocationDto> locationValidator,
        IValidator<CreateOperationalGroupDto> createGroupValidator,
        IValidator<UpdateOperationalGroupDto> updateGroupValidator)
    {
        _repository = repository;
        _unitsQueryValidator = unitsQueryValidator;
        _itemsQueryValidator = itemsQueryValidator;
        _createUnitValidator = createUnitValidator;
        _updateUnitValidator = updateUnitValidator;
        _moveUnitValidator = moveUnitValidator;
        _positionValidator = positionValidator;
        _locationValidator = locationValidator;
        _createGroupValidator = createGroupValidator;
        _updateGroupValidator = updateGroupValidator;
    }

    public async Task<Result<PagedResult<OrganizationUnitDto>>> GetOrganizationUnitsAsync(GetOrganizationUnitsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _unitsQueryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<OrganizationUnitDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<OrganizationUnitDto>>.Success(await _repository.GetOrganizationUnitsAsync(request,cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<OrganizationUnitTreeDto>>> GetOrganizationTreeAsync(bool includeInactive,CancellationToken cancellationToken = default)
    {
        var tree = await _repository.GetOrganizationTreeAsync(includeInactive,cancellationToken);
        return Result<IReadOnlyCollection<OrganizationUnitTreeDto>>.Success(tree);
    }

    public async Task<Result<OrganizationUnitDto>> GetOrganizationUnitAsync(long id,CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetOrganizationUnitAsync(id,cancellationToken);
        return unit is null? Result<OrganizationUnitDto>.Failure(OrganizationErrors.UnitNotFound(id)): Result<OrganizationUnitDto>.Success(Map(unit));
    }

    public async Task<Result<long>> CreateOrganizationUnitAsync(CreateOrganizationUnitDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _createUnitValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        if (!await _repository.OrganizationUnitTypeExistsAsync(request.OrganizationUnitTypeId,cancellationToken)) return Result<long>.Failure(OrganizationErrors.ReferenceNotFound("نوع واحد سازمانی"));
        if (request.ParentOrganizationUnitId.HasValue && await _repository.GetOrganizationUnitAsync(request.ParentOrganizationUnitId.Value,cancellationToken) is null) return Result<long>.Failure(OrganizationErrors.InvalidParent());

        var code = request.Code.Trim();
        if (await _repository.OrganizationUnitCodeExistsAsync(code,cancellationToken: cancellationToken)) return Result<long>.Failure(OrganizationErrors.CodeExists(code));

        var unit = OrganizationUnit.Create(request.OrganizationUnitTypeId,code,request.NameFa,request.NameEn,request.ParentOrganizationUnitId);
        _repository.Add(unit);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(unit.Id);
    }

    public async Task<Result> UpdateOrganizationUnitAsync(long id,UpdateOrganizationUnitDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _updateUnitValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var unit = await _repository.GetOrganizationUnitAsync(id,cancellationToken);
        if (unit is null) return Result.Failure(OrganizationErrors.UnitNotFound(id));
        if (!await _repository.OrganizationUnitTypeExistsAsync(request.OrganizationUnitTypeId,cancellationToken)) return Result.Failure(OrganizationErrors.ReferenceNotFound("نوع واحد سازمانی"));

        var code = request.Code.Trim();
        if (await _repository.OrganizationUnitCodeExistsAsync(code,id,cancellationToken)) return Result.Failure(OrganizationErrors.CodeExists(code));

        unit.Update(request.OrganizationUnitTypeId,code,request.NameFa,request.NameEn);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> MoveOrganizationUnitAsync(long id,MoveOrganizationUnitDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _moveUnitValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var unit = await _repository.GetOrganizationUnitAsync(id,cancellationToken);
        if (unit is null) return Result.Failure(OrganizationErrors.UnitNotFound(id));

        if (request.ParentOrganizationUnitId.HasValue)
        {
            var parentId = request.ParentOrganizationUnitId.Value;
            if (await _repository.GetOrganizationUnitAsync(parentId,cancellationToken) is null) return Result.Failure(OrganizationErrors.InvalidParent());
            if (await _repository.WouldCreateOrganizationCycleAsync(id,parentId,cancellationToken)) return Result.Failure(OrganizationErrors.ParentCycle());
        }

        unit.MoveTo(request.ParentOrganizationUnitId);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeOrganizationUnitStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetOrganizationUnitAsync(id,cancellationToken);
        if (unit is null) return Result.Failure(OrganizationErrors.UnitNotFound(id));
        if (isActive) unit.Activate(); else unit.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<PagedResult<PositionDto>>> GetPositionsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _itemsQueryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<PositionDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<PositionDto>>.Success(await _repository.GetPositionsAsync(request,cancellationToken));
    }
    public async Task<Result<PositionDto>> GetPositionAsync(long id,CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetPositionAsync(id,cancellationToken);
        return item is null
            ? Result<PositionDto>.Failure(OrganizationErrors.PositionNotFound(id))
            : Result<PositionDto>.Success(new PositionDto(item.Id,item.Code,item.TitleFa,item.TitleEn,item.IsActive,item.CreatedAt,item.ModifiedAt));
    }
    public async Task<Result<long>> CreatePositionAsync(SavePositionDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _positionValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var code = request.Code.Trim();
        if (await _repository.PositionCodeExistsAsync(code,cancellationToken: cancellationToken)) return Result<long>.Failure(OrganizationErrors.CodeExists(code));
        var position = Position.Create(code,request.TitleFa,request.TitleEn);
        _repository.Add(position);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(position.Id);
    }
    public async Task<Result> UpdatePositionAsync(long id,SavePositionDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _positionValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var position = await _repository.GetPositionAsync(id,cancellationToken);
        if (position is null) return Result.Failure(OrganizationErrors.PositionNotFound(id));
        var code = request.Code.Trim();
        if (await _repository.PositionCodeExistsAsync(code,id,cancellationToken)) return Result.Failure(OrganizationErrors.CodeExists(code));
        position.Update(code,request.TitleFa,request.TitleEn);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangePositionStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var position = await _repository.GetPositionAsync(id,cancellationToken);
        if (position is null) return Result.Failure(OrganizationErrors.PositionNotFound(id));
        if (isActive) position.Activate(); else position.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PagedResult<WorkLocationDto>>> GetWorkLocationsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _itemsQueryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<WorkLocationDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<WorkLocationDto>>.Success(await _repository.GetWorkLocationsAsync(request,cancellationToken));
    }
    public async Task<Result<WorkLocationDto>> GetWorkLocationAsync(long id,CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWorkLocationAsync(id,cancellationToken);
        return item is null
            ? Result<WorkLocationDto>.Failure(OrganizationErrors.WorkLocationNotFound(id))
            : Result<WorkLocationDto>.Success(new WorkLocationDto(item.Id,item.Code,item.NameFa,item.NameEn,item.Province,item.City,item.Address,item.IsActive,item.CreatedAt,item.ModifiedAt));
    }

    public async Task<Result<long>> CreateWorkLocationAsync(SaveWorkLocationDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _locationValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var code = request.Code.Trim();
        if (await _repository.WorkLocationCodeExistsAsync(code,cancellationToken: cancellationToken)) return Result<long>.Failure(OrganizationErrors.CodeExists(code));
        var location = WorkLocation.Create(code,request.NameFa,request.NameEn,request.Province,request.City,request.Address);
        _repository.Add(location);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(location.Id);
    }

    public async Task<Result> UpdateWorkLocationAsync(long id,SaveWorkLocationDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _locationValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var location = await _repository.GetWorkLocationAsync(id,cancellationToken);
        if (location is null) return Result.Failure(OrganizationErrors.WorkLocationNotFound(id));
        var code = request.Code.Trim();
        if (await _repository.WorkLocationCodeExistsAsync(code,id,cancellationToken)) return Result.Failure(OrganizationErrors.CodeExists(code));
        location.Update(code,request.NameFa,request.NameEn,request.Province,request.City,request.Address);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ChangeWorkLocationStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetWorkLocationAsync(id,cancellationToken);
        if (location is null) return Result.Failure(OrganizationErrors.WorkLocationNotFound(id));
        if (isActive) location.Activate(); else location.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<PagedResult<OperationalGroupDto>>> GetOperationalGroupsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _itemsQueryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<OperationalGroupDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<OperationalGroupDto>>.Success(await _repository.GetOperationalGroupsAsync(request,cancellationToken));
    }
    public async Task<Result<OperationalGroupDto>> GetOperationalGroupAsync(long id,CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetOperationalGroupAsync(id,cancellationToken);
        return item is null
            ? Result<OperationalGroupDto>.Failure(OrganizationErrors.OperationalGroupNotFound(id))
            : Result<OperationalGroupDto>.Success(new OperationalGroupDto(item.Id,item.Code,item.Name,item.Type,item.IsActive,item.CreatedAt,item.ModifiedAt));
    }
    public async Task<Result<long>> CreateOperationalGroupAsync(CreateOperationalGroupDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _createGroupValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _repository.OperationalGroupCodeExistsAsync(code,cancellationToken)) return Result<long>.Failure(OrganizationErrors.CodeExists(code));
        var group = OperationalGroup.Create(code,request.Name,request.Type);
        _repository.Add(group);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(group.Id);
    }
    public async Task<Result> UpdateOperationalGroupAsync(long id,UpdateOperationalGroupDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _updateGroupValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var group = await _repository.GetOperationalGroupAsync(id,cancellationToken);
        if (group is null) return Result.Failure(OrganizationErrors.OperationalGroupNotFound(id));
        group.Update(request.Name,request.Type);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ChangeOperationalGroupStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var group = await _repository.GetOperationalGroupAsync(id,cancellationToken);
        if (group is null) return Result.Failure(OrganizationErrors.OperationalGroupNotFound(id));
        if (isActive) group.Activate(); else group.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    private static OrganizationUnitDto Map(OrganizationUnit unit)
    {
        return new OrganizationUnitDto(
            unit.Id,
            unit.OrganizationUnitTypeId,
            unit.OrganizationUnitType.NameFa,
            unit.ParentOrganizationUnitId,
            unit.ParentOrganizationUnit?.NameFa,
            unit.Code,
            unit.NameFa,
            unit.NameEn,
            unit.IsActive,
            unit.CreatedAt,
            unit.ModifiedAt);
    }
}
