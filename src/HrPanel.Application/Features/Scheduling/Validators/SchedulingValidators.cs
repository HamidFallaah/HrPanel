using FluentValidation;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Features.Scheduling.Validators;

public sealed class GetSchedulingItemsDtoValidator : AbstractValidator<GetSchedulingItemsDto>
{
    public GetSchedulingItemsDtoValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1,100);
    }
}

public sealed class SaveShiftDtoValidator : AbstractValidator<SaveShiftDto>
{
    public SaveShiftDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).MaximumLength(100);
        RuleFor(x => x.WorkHours).GreaterThan(0).LessThanOrEqualTo(24);
        RuleFor(x => x).Must(x => x.WorkHours <= CalculateDuration(x.StartTime,x.EndTime))
            .WithMessage("ساعات کاری نمی‌تواند بیشتر از مدت شیفت باشد");
    }

    private static decimal CalculateDuration(TimeOnly startTime,TimeOnly endTime)
    {
        var start = startTime.ToTimeSpan();
        var end = endTime.ToTimeSpan();
        var duration = end > start ? end - start : TimeSpan.FromDays(1) - start + end;
        return (decimal)duration.TotalHours;
    }
}

public sealed class CreateWorkScheduleDtoValidator : AbstractValidator<CreateWorkScheduleDto>
{
    public CreateWorkScheduleDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameEn).MaximumLength(150);
        RuleFor(x => x.PatternType).IsInEnum();
        RuleFor(x => x.CycleLengthDays).InclusiveBetween((short)1,(short)366);
        RuleFor(x => x.AnchorDate).NotNull().When(x => x.PatternType == WorkSchedulePatternType.Rotating);
        RuleFor(x => x.CycleLengthDays).Equal((short)7).When(x => x.PatternType == WorkSchedulePatternType.Weekly);
        RuleForEach(x => x.Days).SetValidator(new SetWorkScheduleDayDtoValidator());
        RuleFor(x => x.Days).Must(days => days.Select(day => day.DayIndex).Distinct().Count() == days.Count)
            .WithMessage("شاخص روزهای برنامه کاری نباید تکراری باشد");
        RuleFor(x => x).Must(request => request.Days.All(day => day.DayIndex < request.CycleLengthDays))
            .WithMessage("شاخص روز باید از طول چرخه کمتر باشد");
    }
}

public sealed class UpdateWorkScheduleDtoValidator : AbstractValidator<UpdateWorkScheduleDto>
{
    public UpdateWorkScheduleDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.NameFa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameEn).MaximumLength(150);
        RuleFor(x => x.PatternType).IsInEnum();
        RuleFor(x => x.CycleLengthDays).InclusiveBetween((short)1,(short)366);
        RuleFor(x => x.AnchorDate).NotNull().When(x => x.PatternType == WorkSchedulePatternType.Rotating);
        RuleFor(x => x.CycleLengthDays).Equal((short)7).When(x => x.PatternType == WorkSchedulePatternType.Weekly);
    }
}

public sealed class SetWorkScheduleDayDtoValidator : AbstractValidator<SetWorkScheduleDayDto>
{
    public SetWorkScheduleDayDtoValidator()
    {
        RuleFor(x => x.DayIndex).InclusiveBetween((short)0,(short)365);
        RuleFor(x => x.ShiftId).Null().When(x => x.IsRestDay);
        RuleFor(x => x.ShiftId).NotNull().GreaterThan(0L).When(x => !x.IsRestDay);
    }
}

public sealed class AssignWorkScheduleDtoValidator : AbstractValidator<AssignWorkScheduleDto>
{
    public AssignWorkScheduleDtoValidator()
    {
        RuleFor(x => x.EmploymentId).GreaterThan(0);
        RuleFor(x => x.WorkScheduleId).GreaterThan(0);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x.RotationOffsetDays).InclusiveBetween((short)0,(short)365);
    }
}

public sealed class EndScheduleAssignmentDtoValidator : AbstractValidator<EndScheduleAssignmentDto>
{
    public EndScheduleAssignmentDtoValidator()
    {
        RuleFor(x => x.EffectiveTo).NotEmpty();
    }
}
