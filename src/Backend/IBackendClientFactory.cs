namespace AOAISSEProxy;

public interface IBackendClientFactory
{
    HttpClient CreateClient();
}
