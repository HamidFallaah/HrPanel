using HrPanel.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Common.Results;

public static class ApiResultExtensions
{
    public static IActionResult ToActionResult<TValue>(this Result<TValue> result,ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return CreateErrorResult(result.Error, controller);
    }
    public static IActionResult ToActionResult(this Result result,ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return CreateErrorResult(result.Error, controller);
    }
    private static IActionResult CreateErrorResult(Error error,ControllerBase controller)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.None => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(error.Type),
            Detail = error.Description
        };

        problemDetails.Extensions["code"] = error.Code;

        if (error is ValidationError validationError)
        {
            problemDetails.Extensions["errors"] = validationError.Errors;
        }

        return controller.StatusCode(statusCode, problemDetails);
    }
    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Failure => "درخواست نامعتبر است",
            ErrorType.Validation => "خطای اعتبارسنجی",
            ErrorType.NotFound => "اطلاعات موردنظر پیدا نشد",
            ErrorType.Conflict => "تداخل اطلاعات",
            ErrorType.Unauthorized => "احراز هویت انجام نشده است",
            ErrorType.Forbidden => "دسترسی مجاز نیست",
            _ => "خطایی رخ داده است"
        };
    }
}
