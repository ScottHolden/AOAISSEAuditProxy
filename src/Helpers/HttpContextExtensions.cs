using System.Buffers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace AOAISSEProxy;

public static class HttpContextExtensions
{
    public static Uri GetRelativeUri(this HttpRequest request)
        => new(request.GetEncodedPathAndQuery(), UriKind.Relative);

    public static void AddHttpHeaders(this HttpResponse response, HttpHeaders headers)
    {
        foreach (var header in headers)
        {
            response.Headers.TryAdd(header.Key, header.Value.ToStringValues());
        }
    }

    public static void CopyHeadersToRequestMessage(this HttpRequest source, HttpRequestMessage target, string[] excludedHeaders)
    {
        foreach (var header in source.Headers)
        {
            if (excludedHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase)) continue;

            if (!target.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && target.Content != null)
            {
                target.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }
    private static StringValues ToStringValues(this IEnumerable<string>? values)
        => new(values?.ToArray());
}
