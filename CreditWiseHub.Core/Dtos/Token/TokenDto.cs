using System.Text.Json.Serialization;

namespace CreditWiseHub.Core.Dtos.Token
{
    public class TokenDto
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("accessTokenExpiration")]
        public DateTime AccessTokenExpiration { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("refreshTokenExpiration")]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
