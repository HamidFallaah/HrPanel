namespace HrPanel.Application.Common.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateOnly CurrentDate { get; }
}
