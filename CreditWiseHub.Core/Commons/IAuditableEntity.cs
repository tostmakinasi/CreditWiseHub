namespace CreditWiseHub.Core.Commons
{
    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
        bool IsActive { get; set; }
    }
}
