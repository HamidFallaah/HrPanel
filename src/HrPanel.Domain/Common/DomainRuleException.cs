namespace HrPanel.Domain.Common
{
    // این فقط برای تخلفات ثابتی است که هرگز نباید پس از اعتبارسنجی برنامه به موجودیت برسند
    public sealed class DomainRuleException : Exception
    {
        public DomainRuleException(string message) : base(message)
        {
            
        }
    }
}
