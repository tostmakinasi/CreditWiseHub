using CreditWiseHub.Core.Commons;

namespace CreditWiseHub.Core.Models
{
    public class UserRefreshToken : IEntity
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
