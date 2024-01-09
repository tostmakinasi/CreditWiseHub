using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CreditWiseHub.Core.Responses
{
    public class ErrorDto
    {
        [JsonPropertyName("errors")]
        public List<string> Errors { get; private set; }

        [JsonPropertyName("isShow")]
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors = new List<string>
            {
                error
            };
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }

        public ErrorDto(IEnumerable<IdentityError> ıdentityErrors, bool isShow)
        {
            Errors = ıdentityErrors.Select(x => x.Description).ToList();
            IsShow = isShow;
        }
    }
}
