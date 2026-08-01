namespace HrPanel.Application.Common.Results;

public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors): Error("Validation.General","یک یا چند خطای اعتبارسنجی رخ داده است",ErrorType.Validation);