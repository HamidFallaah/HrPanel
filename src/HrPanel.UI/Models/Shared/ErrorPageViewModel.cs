namespace HrPanel.UI.Models.Shared;

public sealed record ErrorPageViewModel(int StatusCode,string Title,string Message,string Icon,bool CanRetry)
{
    public static ErrorPageViewModel FromStatusCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => new(
                statusCode,
                "درخواست نامعتبر است",
                "اطلاعات درخواست کامل یا معتبر نیست لطفاً ورودی‌ها را بررسی و دوباره تلاش کنید",
                "bi-exclamation-diamond",
                true),
            StatusCodes.Status401Unauthorized => new(
                statusCode,
                "نشست کاربری معتبر نیست",
                "برای ادامه لازم است دوباره وارد سامانه شوید",
                "bi-shield-lock",
                false),
            StatusCodes.Status403Forbidden => new(
                statusCode,
                "دسترسی مجاز نیست",
                "حساب کاربری شما اجازه مشاهده این بخش را ندارد",
                "bi-shield-exclamation",
                false),
            StatusCodes.Status404NotFound => new(
                statusCode,
                "صفحه پیدا نشد",
                "صفحه یا رکورد درخواستی وجود ندارد یا نشانی آن تغییر کرده است",
                "bi-search",
                false),
            _ => new(
                StatusCodes.Status500InternalServerError,
                "خطای پیش‌بینی‌نشده",
                "در انجام عملیات مشکلی رخ داد لطفاً چند لحظه دیگر دوباره تلاش کنید",
                "bi-exclamation-octagon",
                true)
        };
    }
}
