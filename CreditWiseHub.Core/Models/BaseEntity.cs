namespace CreditWiseHub.Core.Models
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
