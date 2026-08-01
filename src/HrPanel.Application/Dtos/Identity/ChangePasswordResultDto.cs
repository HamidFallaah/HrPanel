namespace HrPanel.Application.Dtos.Identity;

public sealed record ChangePasswordResultDto(bool Succeeded,IReadOnlyCollection<string> Errors)
{
    public static ChangePasswordResultDto Success()
    {
        return new ChangePasswordResultDto(true,Array.Empty<string>());
    }
    public static ChangePasswordResultDto Failure(IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var normalizedErrors = errors.Where(error => !string.IsNullOrWhiteSpace(error)).Distinct().ToArray();
        return new ChangePasswordResultDto(false,normalizedErrors);
    }
}