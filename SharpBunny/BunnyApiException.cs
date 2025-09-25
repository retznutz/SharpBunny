using System.Net;
using SharpBunny.Models;

namespace SharpBunny;

public class BunnyApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ApiError? ApiError { get; }

    public BunnyApiException(string message, HttpStatusCode statusCode, ApiError? apiError = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ApiError = apiError;
    }

    public BunnyApiException(string message, HttpStatusCode statusCode, Exception innerException, ApiError? apiError = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ApiError = apiError;
    }
}