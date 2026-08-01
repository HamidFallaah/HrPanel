namespace HrPanel.Persistence.Identity;

public sealed class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";
    public bool CreateAdministrator { get; init; }
    public string? AdministratorUserName { get; init; }
    public string? AdministratorPassword { get; init; }
    public string? AdministratorEmail { get; init; }
    public string? AdministratorDisplayName { get; init; }
}