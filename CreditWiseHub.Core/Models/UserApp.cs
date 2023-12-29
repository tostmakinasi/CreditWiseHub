using Microsoft.AspNetCore.Identity;

namespace CreditWiseHub.Core.Models
{
    public class UserApp : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual List<Account> Accounts { get; set; }
    }
}
