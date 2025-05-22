using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BeautySalon.Infrastructure;

public class OperationResponseBase
{
    protected HttpStatusCode StatusCode { get; set; }

    protected object? Result { get; set; }

    protected string? FileName { get; set; }

    public IActionResult GetResponse(HttpRequest request, HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(response);

        response.StatusCode = (int)StatusCode;

        if (Result is null)
        {
            return new StatusCodeResult((int)StatusCode);
        }
        if (Result is Stream stream)
        {
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = FileName
            };
        }

        return new ObjectResult(Result);
    }

    public static TResult OK<TResult, TData>(TData data) where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.OK, Result = data };

    protected static TResult OK<TResult, TData>(TData data, string fileName) where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.OK, Result = data, FileName = fileName };

    protected static TResult NoContent<TResult>() where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.NoContent };

    protected static TResult BadRequest<TResult>(string? errorMessage = null) where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.BadRequest, Result = errorMessage };

    protected static TResult NotFound<TResult>(string? errorMessage = null) where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.NotFound, Result = errorMessage };

    protected static TResult InternalServerError<TResult>(string? errorMessage = null) where TResult : OperationResponseBase, new() => new() { StatusCode = HttpStatusCode.InternalServerError, Result = errorMessage };
}
