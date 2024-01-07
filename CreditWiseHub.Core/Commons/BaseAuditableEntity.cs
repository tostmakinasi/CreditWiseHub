namespace CreditWiseHub.Core.Commons
{
    public abstract class BaseAuditableEntity<TKey> : IEntity, IEntityWithId<TKey>, IAuditableEntity
    {
        public TKey Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
