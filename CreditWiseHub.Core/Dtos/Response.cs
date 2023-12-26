using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Dtos
{
    public class Response <T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get; private set; }

        [JsonIgnore]
        public bool IsSuccess { get; private set; }

        public static Response<T> Success (T data, int statusCode)  => new Response<T> { Data = data, StatusCode = statusCode, IsSuccess = true };

        public static Response<T> Success(int statusCode) => new Response<T> { Data = default, StatusCode = statusCode, IsSuccess = true };

        public static Response<T> Fail( ErrorDto error, int statusCode ) => new Response<T> {  Error = error, StatusCode = statusCode, IsSuccess = false };

        public static Response<T> Fail(string error, int statusCode, bool isShow) => new Response<T> { Error = new ErrorDto(error, isShow), StatusCode = statusCode, IsSuccess = false };
    }
}
