using System.Net;
using System.Text.Json.Serialization;

namespace CreditWiseHub.Core.Dtos.Responses
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get; private set; }

        [JsonIgnore]
        public bool IsSuccess { get; private set; }

        public static Response<T> Success(T data, HttpStatusCode statusCode) => new Response<T> { Data = data, StatusCode = (int)statusCode, IsSuccess = true };

        public static Response<T> Success(HttpStatusCode statusCode) => new Response<T> { Data = default, StatusCode = (int)statusCode, IsSuccess = true };

        public static Response<T> Fail(ErrorDto error, HttpStatusCode statusCode) => new Response<T> { Error = error, StatusCode = (int)statusCode, IsSuccess = false };

        public static Response<T> Fail(string error, HttpStatusCode statusCode, bool isShow) => new Response<T> { Error = new ErrorDto(error, isShow), StatusCode = (int)statusCode, IsSuccess = false };
    }
}
