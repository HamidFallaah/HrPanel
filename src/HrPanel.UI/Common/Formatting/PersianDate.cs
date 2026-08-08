using System.Globalization;

namespace HrPanel.UI.Common.Formatting;

public static class PersianDate
{
    private static readonly PersianCalendar Calendar = new();
    public static string Format(DateOnly? value) => value.HasValue ? Format(value.Value) : "—";
    public static string Format(DateOnly value) =>
        $"{Calendar.GetYear(value.ToDateTime(TimeOnly.MinValue)):0000}/{Calendar.GetMonth(value.ToDateTime(TimeOnly.MinValue)):00}/{Calendar.GetDayOfMonth(value.ToDateTime(TimeOnly.MinValue)):00}";
    public static string Format(DateTime? value) => value.HasValue ? Format(value.Value) : "—";
    public static string Format(DateTime value) =>
        $"{Calendar.GetYear(value):0000}/{Calendar.GetMonth(value):00}/{Calendar.GetDayOfMonth(value):00}";

    public static bool TryParse(string? value, out DateOnly date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var normalized = NormalizeDigits(value.Trim()).Replace('-', '/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month) || !int.TryParse(parts[2], out var day)) return false;
        try
        {
            date = DateOnly.FromDateTime(Calendar.ToDateTime(year, month, day, 0, 0, 0, 0));
            return true;
        }
        catch (ArgumentOutOfRangeException) { return false; }
    }

    private static string NormalizeDigits(string value)
    {
        const string fa = "۰۱۲۳۴۵۶۷۸۹";
        const string ar = "٠١٢٣٤٥٦٧٨٩";
        for (var i = 0; i < 10; i++) value = value.Replace(fa[i], (char)('0' + i)).Replace(ar[i], (char)('0' + i));
        return value;
    }
}
