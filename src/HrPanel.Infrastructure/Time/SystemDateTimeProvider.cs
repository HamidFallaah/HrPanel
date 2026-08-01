using HrPanel.Application.Common.Abstractions.Services;

namespace HrPanel.Infrastructure.Time;

internal sealed class SystemDateTimeProvider: IDateTimeProvider
{
    private static readonly TimeZoneInfo IranTimeZone = FindIranTimeZone();
    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,IranTimeZone);
    public DateOnly CurrentDate => DateOnly.FromDateTime(Now);
    private static TimeZoneInfo FindIranTimeZone()
    {
        var timeZoneId = OperatingSystem.IsWindows()? "Iran Standard Time": "Asia/Tehran";

        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
