namespace AOAISSEProxy;

public static class TerminalMiddlewareExtensions
{
    public static IApplicationBuilder UseTerminatingMiddleware<T>(
        this IApplicationBuilder builder) where T : ITerminalMiddleware
        => builder.Use(_ => builder.ApplicationServices.GetRequiredService<T>().RequestDelegate);
}
