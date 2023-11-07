using System.Buffers;

namespace AOAISSEProxy;

public class SSELoggerProxyTerminalMiddleware : ITerminalMiddleware
{
    private readonly IBackendClientFactory _backendClientFactory;
    private readonly IAuditLog _auditLog;
    private static readonly string[] s_excludedBackendHeaders = new[] {
            "host",
            "method",
            "authority",
            "path",
            "accept-encoding" // We don't support compression passthrough
		};
    // Don't log keys!
    private static readonly string[] s_excludedAuditHeaders = new[] {
            "authorization",
            "api-key",
            "ocp-apim-subscription-key"
        };
    private static readonly string[] s_redactedHeaderValue = new[] { "<REDACTED>" };

    public SSELoggerProxyTerminalMiddleware(IBackendClientFactory backendClientFactory, IAuditLog auditLog)
    {
        _backendClientFactory = backendClientFactory;
        _auditLog = auditLog;
    }

    public async Task RequestDelegate(HttpContext context)
    {
        var correlationId = Guid.NewGuid();
        var request = context.Request;
        var response = context.Response;
        try
        {
            // Start by logging connection & request headers
            _auditLog.LogConnection(correlationId, context.Connection);
            _auditLog.LogRequestHeaders(correlationId, SanitisedHeadersToDictionary(request.Headers));

            var relativePath = request.GetRelativeUri();
            using var backendClient = _backendClientFactory.CreateClient();

            // Build a backend request from the inbound request path, headers, and content
            var backendRequest = new HttpRequestMessage(new HttpMethod(request.Method), relativePath);

            // Storing in memory is ok for a proof of concept, but consider mem utilization on high-throughput implementations
            using var reqMemStream = new MemoryStream();
            var requestInterceptor = new ReadStreamInterceptor(request.Body, reqMemStream);
            backendRequest.Content = new StreamContent(requestInterceptor);

            // Copy our headers excluding the ones we will change
            request.CopyHeadersToRequestMessage(backendRequest, s_excludedBackendHeaders);

            // Send the request, but don't wait for the whole response, only headers
            var backendResponse = await backendClient.SendAsync(backendRequest, HttpCompletionOption.ResponseHeadersRead);

            // At this point we have seen all of the request content, so log it
            _auditLog.LogRequestContent(correlationId, reqMemStream.ToArray());

            // Copy the response headers over
            response.AddHttpHeaders(backendResponse.Headers);
            response.AddHttpHeaders(backendResponse.Content.Headers);

            // And log the response headers
            _auditLog.LogResponseHeaders(correlationId, SanitisedHeadersToDictionary(response.Headers));

            // We don't allow chunking
            response.Headers.Remove("transfer-encoding");

            using var resStream = await backendResponse.Content.ReadAsStreamAsync();

            // Build our logger
            using var respMemStream = new MemoryStream();
            var responseInterceptor = new ReadStreamInterceptor(resStream, respMemStream);
            try
            {
                await responseInterceptor.CopyToAsync(response.Body);
            }
            finally
            {
                // Log the response
                _auditLog.LogResponseContent(correlationId, respMemStream.ToArray());
            }
        }
        catch (Exception ex)
        {
            // Log any errors thrown by middleware
            _auditLog.LogException(correlationId, ex);
            throw;
        }
    }

    private static Dictionary<string, string[]> SanitisedHeadersToDictionary(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string[]>();
        foreach (var header in headers)
        {
            if (s_excludedAuditHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                result.Add(header.Key, s_redactedHeaderValue);
            }
            else
            {
                var headerValues = header.Value.ToArray<string>();
                if (headerValues != null) result.Add(header.Key, headerValues);
            }
        }
        return result;
    }
}
