using System.ComponentModel.DataAnnotations;

namespace HrPanel.UI.Models.Authentication;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "رمز عبور فعلی الزامی است")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور فعلی")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [MinLength(8, ErrorMessage = "رمز عبور جدید حداقل ۸ تا باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور جدید")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [Compare(nameof(NewPassword), ErrorMessage = "تکرار رمز عبور یکسان نیست")]
    [DataType(DataType.Password)]
    [Display(Name = "تکرار رمز عبور جدید")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
