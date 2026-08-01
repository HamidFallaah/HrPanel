namespace HrPanel.Domain.Common;

public abstract class AuditableEntity<TKey> : BaseEntity<TKey> where TKey : notnull
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public Guid? ModifiedByUserId { get; private set; }
}
