namespace CreditWiseHub.Core.Dtos.User
{
    public class CreateUserDto
    {
        public string TCKN { get; set; }//Username mapping
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
