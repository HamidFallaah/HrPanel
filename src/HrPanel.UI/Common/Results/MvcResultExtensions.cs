using HrPanel.Application.Common.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HrPanel.UI.Common.Results;
internal static class MvcResultExtensions
{
    public static void AddToModelState(this Result result, ModelStateDictionary modelState)
    {
        if (result.Error is ValidationError validation)
        {
            foreach (var (key, messages) in validation.Errors)
                foreach (var message in messages)
                    modelState.AddModelError(key, message);
            return;
        }

        modelState.AddModelError(string.Empty, result.Error.Description);
    }

    public static void SetFailureMessage(this Result result, ITempDataDictionary tempData) => tempData["ErrorMessage"] = result.Error.Description;
}
