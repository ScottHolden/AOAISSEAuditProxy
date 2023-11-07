using System.Text;

namespace AOAISSEProxy;

// A basic example of logging to stdout via ILogger
public class LoggerAuditLog : IAuditLog
{
    private readonly ILogger _logger;
    public LoggerAuditLog(ILogger<LoggerAuditLog> logger)
    {
        _logger = logger;
    }
    public void LogConnection(Guid correlationId, ConnectionInfo connection)
        => _logger.LogInformation("[{correlationId}] Connection from {ipAddress}", correlationId, connection.RemoteIpAddress);

    public void LogException(Guid correlationId, Exception exception)
        => _logger.LogError(exception, "[{correlationId}] Error whilst proxying request; {errorMessage}", correlationId, exception.Message);

    public void LogRequestContent(Guid correlationId, byte[] content)
        => _logger.LogInformation("[{correlationId}] Request content -----\n{requestContent}\n-----", correlationId, Encoding.UTF8.GetString(content));

    public void LogRequestHeaders(Guid correlationId, Dictionary<string, string[]> headers)
        => _logger.LogInformation("[{correlationId}] Request headers -----\n{requestHeaders}\n-----", correlationId, HeadersToString(headers));

    public void LogResponseContent(Guid correlationId, byte[] content)
        => _logger.LogInformation("[{correlationId}] Response content -----\n{responseContent}\n-----", correlationId, Encoding.UTF8.GetString(content));

    public void LogResponseHeaders(Guid correlationId, Dictionary<string, string[]> headers)
        => _logger.LogInformation("[{correlationId}] Response headers -----\n{responseHeaders}\n-----", correlationId, HeadersToString(headers));

    private static string HeadersToString(Dictionary<string, string[]> headers)
    {
        var sb = new StringBuilder();
        foreach (var header in headers)
        {
            foreach (var value in header.Value)
            {
                sb.Append(header.Key);
                sb.Append(": ");
                sb.AppendLine(value);
            }
        }
        return sb.ToString();
    }
}
