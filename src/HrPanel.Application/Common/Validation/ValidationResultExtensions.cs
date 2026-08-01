using FluentValidation.Results;
using HrPanel.Application.Common.Results;

namespace HrPanel.Application.Common.Validation;

public static class ValidationResultExtensions
{
    public static ValidationError ToValidationError(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Where(failure => failure is not null)
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(failure => failure.ErrorMessage)
                    .Distinct()
                    .ToArray());

        return new ValidationError(errors);
    }
}
