using Microsoft.AspNetCore.HttpLogging;

namespace serilog_payload_demo.Configuration;

internal sealed class CustomHttpLoggingInterceptor: IHttpLoggingInterceptor
{
    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        if (logContext.HttpContext.Request.ContentType != null && logContext.HttpContext.Request.ContentType.Contains("json"))
        {
            logContext.LoggingFields = HttpLoggingFields.RequestBody
                | HttpLoggingFields.RequestHeaders
                | HttpLoggingFields.RequestProtocol
                | HttpLoggingFields.RequestQuery;
        }
        else
        {
            logContext.LoggingFields = HttpLoggingFields.None;
        }
        return default;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        if (logContext.HttpContext.Response.ContentType != null && logContext.HttpContext.Response.ContentType.Contains("json"))
        {
            logContext.LoggingFields = HttpLoggingFields.RequestBody
                                       | HttpLoggingFields.RequestHeaders
                                       | HttpLoggingFields.RequestProtocol
                                       | HttpLoggingFields.RequestQuery
                                       | HttpLoggingFields.ResponseBody
                                       | HttpLoggingFields.ResponseHeaders
                                       | HttpLoggingFields.Duration
                                       | HttpLoggingFields.ResponseStatusCode;
        }
        return default;
    }
}