namespace AOAISSEProxy;

public interface ITerminalMiddleware
{
    Task RequestDelegate(HttpContext context);
}
