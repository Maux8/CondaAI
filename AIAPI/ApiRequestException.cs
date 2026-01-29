using System.Net;

namespace Conda_AI.AIAPI;

public class ApiRequestException(string data, HttpStatusCode statusCode): Exception
{
    public HttpStatusCode StatusCode = statusCode;
    public string ExceptionData = data;
}