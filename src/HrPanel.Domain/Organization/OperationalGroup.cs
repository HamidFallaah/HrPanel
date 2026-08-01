using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization;

public sealed class OperationalGroup : AuditableEntity<long>
{
    private OperationalGroup()
    {

    }

    private OperationalGroup(string code,string name,OperationalGroupType type)
    {
        Code = NormalizeCode(code);
        Name = NormalizeRequiredText(name, nameof(name));
        Type = type;
        IsActive = true;
    }

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public OperationalGroupType Type { get; private set; }
    public bool IsActive { get; private set; }
    public static OperationalGroup Create(string code,string name,OperationalGroupType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new DomainRuleException("The operational group type is invalid.");
        }

        return new OperationalGroup(code, name, type);
    }
    public void Rename(string name)
    {
        Name = NormalizeRequiredText(name, nameof(name));
    }

    public void Update(string name,OperationalGroupType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new DomainRuleException("نوع گروه عملیاتی معتبر نیست");
        }

        Name = NormalizeRequiredText(name,nameof(name));
        Type = type;
    }
    public void Activate()
    {
        IsActive = true;
    }
    public void Deactivate()
    {
        IsActive = false;
    }
    private static string NormalizeCode(string value)
    {
        var normalized = NormalizeRequiredText(value, nameof(value)).Replace('_', ' ').Trim().ToUpperInvariant();

        while (normalized.Contains("  "))
        {
            normalized = normalized.Replace("  ", " ");
        }

        return normalized.Replace(' ', '_');
    }

    private static string NormalizeRequiredText(string? value,string parameterName)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainRuleException($"{parameterName} cannot be empty.");
        }

        return normalized;
    }
}
