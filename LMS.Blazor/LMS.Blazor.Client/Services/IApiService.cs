namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);

    Task<TResponse?> PutAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data,
    CancellationToken ct = default);

    Task<TResponse?> CreateAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data,
    CancellationToken ct = default);
}