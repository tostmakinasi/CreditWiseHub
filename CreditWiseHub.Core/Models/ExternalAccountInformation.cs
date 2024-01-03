using CreditWiseHub.Core.Commons;

namespace CreditWiseHub.Core.Models
{
    public class ExternalAccountInformation : BaseEntity<long>
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string OwnerFullName { get; set; }

    }
}
