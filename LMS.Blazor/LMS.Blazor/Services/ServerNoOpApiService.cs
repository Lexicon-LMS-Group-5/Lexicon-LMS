using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services;

public class ServerNoOpApiService(ILogger<ServerNoOpApiService> logger) : IApiService
{
    private readonly ILogger<ServerNoOpApiService> _logger = logger;

    public Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }
    public Task<TResponse?> PutAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data,
    CancellationToken ct = default)
    {
        _logger.LogWarning(
            "ServerNoOpApiService PUT called for: {Endpoint} with payload: {@Data}",
            endpoint,
            data);

        return Task.FromResult<TResponse?>(default);
    }
}
