namespace CreditWiseHub.Core.Commons
{
    public abstract class BaseEntity<T> : IEntity, IEntityWithId<T>
    {
        public T Id { get; set; }
    }
}
