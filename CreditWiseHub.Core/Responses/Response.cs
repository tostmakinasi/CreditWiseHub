using System.Net;
using System.Text.Json.Serialization;

namespace CreditWiseHub.Core.Responses
{
    public class Response<T> where T : class
    {
        [JsonPropertyName("data")]
        public T? data { get; private set; }

        [JsonPropertyName("statusCode")]
        public int statusCode { get; private set; }

        [JsonPropertyName("error")]
        public ErrorDto? error { get; private set; }

        [JsonPropertyName("isSuccess")]
        public bool isSuccess { get; private set; }

        public static Response<T> Success(T data, HttpStatusCode statusCode) => new Response<T> { data = data, statusCode = (int)statusCode, isSuccess = true };

        public static Response<T> Success(HttpStatusCode statusCode) => new Response<T> { data = default, statusCode = (int)statusCode, isSuccess = true };

        public static Response<T> Fail(ErrorDto error, HttpStatusCode statusCode) => new Response<T> { error = error, statusCode = (int)statusCode, isSuccess = false };

        public static Response<T> Fail(string error, HttpStatusCode statusCode, bool isShow) => new Response<T> { error = new ErrorDto(error, isShow), statusCode = (int)statusCode, isSuccess = false };
    }
}
