namespace HrPanel.Domain.Common;

// Because transaction tables use bigint, while lookup tables may use smallint, we keep it generic
public abstract class BaseEntity<TKey> where TKey : notnull
{
    public TKey Id { get; protected set; } = default!;
}

