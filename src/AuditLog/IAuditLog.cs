namespace AOAISSEProxy;

// Super simple memory-based interface for proof of concept, could be switched to streams
public interface IAuditLog
{
    void LogRequestHeaders(Guid correlationId, Dictionary<string, string[]> headers);
    void LogRequestContent(Guid correlationId, byte[] content);
    void LogResponseHeaders(Guid correlationId, Dictionary<string, string[]> headers);
    void LogResponseContent(Guid correlationId, byte[] content);
    void LogException(Guid correlationId, Exception exception);
    void LogConnection(Guid correlationId, ConnectionInfo connection);
}
