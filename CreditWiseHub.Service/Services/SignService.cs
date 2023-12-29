using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CreditWiseHub.Service.Services
{
    static class SignService
    {
        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
            => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

    }
}
