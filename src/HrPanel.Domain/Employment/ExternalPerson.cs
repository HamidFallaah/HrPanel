using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class ExternalPerson : AuditableEntity<long>
{
    private ExternalPerson()
    {

    }

    private ExternalPerson(string displayName,string? legacyUsername)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainRuleException("نام شخص خارجی الزامی است");
        }

        DisplayName = displayName.Trim();
        LegacyUsername = legacyUsername?.Trim();
        IsActive = true;
    }

    public string DisplayName { get; private set; } = null!;
    public string? LegacyUsername { get; private set; }
    public bool IsActive { get; private set; }
    public static ExternalPerson Create(string displayName,string? legacyUsername = null)
    {
        return new ExternalPerson(displayName,legacyUsername);
    }

    public void Update(string displayName,string? legacyUsername)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainRuleException("نام شخص خارجی الزامی است");
        }

        DisplayName = displayName.Trim();
        LegacyUsername = string.IsNullOrWhiteSpace(legacyUsername)
            ? null
            : legacyUsername.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
