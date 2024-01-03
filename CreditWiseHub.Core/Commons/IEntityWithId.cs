namespace CreditWiseHub.Core.Commons
{
    public interface IEntityWithId<T>
    {
        T Id { get; set; }
    }
}
