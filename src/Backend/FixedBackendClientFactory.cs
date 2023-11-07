namespace AOAISSEProxy;

public class FixedBackendClientFactory : IBackendClientFactory
{
    private const string BaseAddressConfigName = "AOAI_BASEADDRESS";
    private readonly Uri _baseAddress;
    private readonly IHttpClientFactory _httpClientFactory;
    public FixedBackendClientFactory(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        var baseAddressValue = config[BaseAddressConfigName];
        if (string.IsNullOrWhiteSpace(baseAddressValue)
            || !Uri.TryCreate(baseAddressValue, UriKind.Absolute, out Uri? baseAddress)
            || baseAddress == null)
        {
            throw new Exception($"Configuration value for '{BaseAddressConfigName}' was missing, empty, or an invalid absolute url.");
        }
        _baseAddress = baseAddress;
        _httpClientFactory = httpClientFactory;
    }
    public HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = _baseAddress;
        return client;
    }
}
