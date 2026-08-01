using FluentValidation;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Domain.Assets;
using HrPanel.Domain.Common;

namespace HrPanel.Application.Features.Assets;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;
    private readonly IValidator<GetAssetsDto> _queryValidator;
    private readonly IValidator<CreateAssetDto> _createValidator;
    private readonly IValidator<UpdateAssetDto> _updateValidator;
    private readonly IValidator<AssignAssetDto> _assignValidator;
    private readonly IValidator<ReturnAssetDto> _returnValidator;

    public AssetService(
        IAssetRepository repository,
        IValidator<GetAssetsDto> queryValidator,
        IValidator<CreateAssetDto> createValidator,
        IValidator<UpdateAssetDto> updateValidator,
        IValidator<AssignAssetDto> assignValidator,
        IValidator<ReturnAssetDto> returnValidator)
    {
        _repository = repository;
        _queryValidator = queryValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _assignValidator = assignValidator;
        _returnValidator = returnValidator;
    }

    public async Task<Result<PagedResult<AssetListItemDto>>> GetAssetsAsync(GetAssetsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _queryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<AssetListItemDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<AssetListItemDto>>.Success(await _repository.GetPagedAsync(request,cancellationToken));
    }

    public async Task<Result<AssetDetailsDto>> GetAssetAsync(long id,CancellationToken cancellationToken = default)
    {
        var asset = await _repository.GetDetailsAsync(id, cancellationToken);

        return asset is null ? Result<AssetDetailsDto>.Failure(AssetErrors.NotFound(id)) : Result<AssetDetailsDto>.Success(asset);
        
    }

    public async Task<Result<long>> CreateAssetAsync(CreateAssetDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());

        if (!await _repository.AssetTypeExistsAsync(request.AssetTypeId,cancellationToken)) 
            return Result<long>.Failure(AssetErrors.TypeNotFound(request.AssetTypeId));

        var uniqueCheck = await ValidateUniqueIdentifiersAsync(request.AssetTag,request.ServiceNumber,request.Imei,request.SerialNumber,null,cancellationToken);
        if (uniqueCheck.IsFailure) return Result<long>.Failure(uniqueCheck.Error);

        var asset = Asset.Create(request.AssetTypeId,request.AssetTag,request.ServiceNumber,request.Imei,request.SerialNumber);
        asset.Update(request.AssetTypeId,request.AssetTag,request.ServiceNumber,request.Imei,request.SerialNumber,request.Notes);
        _repository.Add(asset);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(asset.Id);
    }

    public async Task<Result> UpdateAssetAsync(long id,UpdateAssetDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var asset = await _repository.GetByIdAsync(id,cancellationToken);
        if (asset is null) return Result.Failure(AssetErrors.NotFound(id));
        if (!await _repository.AssetTypeExistsAsync(request.AssetTypeId,cancellationToken)) return Result.Failure(AssetErrors.TypeNotFound(request.AssetTypeId));
        var uniqueCheck = await ValidateUniqueIdentifiersAsync(request.AssetTag,request.ServiceNumber,request.Imei,request.SerialNumber,id,cancellationToken);
        if (uniqueCheck.IsFailure) return uniqueCheck;
        asset.Update(request.AssetTypeId,request.AssetTag,request.ServiceNumber,request.Imei,request.SerialNumber,request.Notes);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<long>> AssignAssetAsync(long id,AssignAssetDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _assignValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var asset = await _repository.GetByIdAsync(id,cancellationToken);
        if (asset is null) return Result<long>.Failure(AssetErrors.NotFound(id));
        if (asset.Status != AssetStatus.Available) return Result<long>.Failure(AssetErrors.NotAvailable());
        if (!await _repository.EmployeeExistsAsync(request.EmployeeId,cancellationToken)) return Result<long>.Failure(AssetErrors.EmployeeNotFound(request.EmployeeId));

        var assignment = EmployeeAssetAssignment.Create(id,request.EmployeeId,request.AssignedAt,request.Notes);
        asset.MarkAsAssigned();
        _repository.Add(assignment);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(assignment.Id);
    }

    public async Task<Result> ReturnAssetAsync(long id,ReturnAssetDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _returnValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var asset = await _repository.GetByIdAsync(id,cancellationToken);
        if (asset is null) return Result.Failure(AssetErrors.NotFound(id));
        var assignment = await _repository.GetCurrentAssignmentAsync(id,cancellationToken);
        if (assignment is null) return Result.Failure(AssetErrors.NoActiveAssignment());
        if (request.ReturnedAt < assignment.AssignedAt) return Result.Failure(Error.Failure("Assets.InvalidReturnDate","تاریخ بازگشت نمی‌تواند قبل از تاریخ واگذاری باشد"));
        assignment.Return(request.ReturnedAt);
        asset.MarkAsReturned();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public Task<Result> SendToMaintenanceAsync(long id,CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(id,asset => asset.SendToMaintenance(),cancellationToken);
    }
    public Task<Result> RetireAssetAsync(long id,CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(id,asset => asset.Retire(),cancellationToken);
    }
    public Task<Result> MarkAssetAsLostAsync(long id,CancellationToken cancellationToken = default)
    {
        return ChangeStatusAsync(id,asset => asset.MarkAsLost(),cancellationToken);
    }
    private async Task<Result> ChangeStatusAsync(long id,Action<Asset> changeStatus,CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(id,cancellationToken);
        if (asset is null) return Result.Failure(AssetErrors.NotFound(id));
        if (asset.Status == AssetStatus.Assigned && await _repository.GetCurrentAssignmentAsync(id,cancellationToken) is not null)
        {
            return Result.Failure(Error.Conflict("Assets.ActiveAssignment","دارایی واگذارشده را ابتدا بازگردانید"));
        }
        try
        {
            changeStatus(asset);
        }
        catch (DomainRuleException exception)
        {
            return Result.Failure(Error.Conflict("Assets.InvalidStatusChange",exception.Message));
        }
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> ValidateUniqueIdentifiersAsync(string? assetTag,string? serviceNumber,string? imei,string? serialNumber,long? excludingId,CancellationToken cancellationToken)
    {
        var values = new[]
        {
            (Property: nameof(Asset.AssetTag),Display: "کد دارایی",Value: Clean(assetTag)),
            (Property: nameof(Asset.ServiceNumber),Display: "شماره سرویس",Value: Clean(serviceNumber)),
            (Property: nameof(Asset.Imei),Display: "IMEI",Value: Clean(imei)),
            (Property: nameof(Asset.SerialNumber),Display: "شماره سریال",Value: Clean(serialNumber))
        };

        foreach (var item in values.Where(item => item.Value is not null))
        {
            if (await _repository.IdentifierExistsAsync(item.Property,item.Value!,excludingId,cancellationToken))
            {
                return Result.Failure(AssetErrors.IdentifierExists(item.Display,item.Value!));
            }
        }

        return Result.Success();
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
