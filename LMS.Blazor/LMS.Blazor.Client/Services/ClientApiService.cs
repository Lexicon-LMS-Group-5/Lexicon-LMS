using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientApiService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;

        _navigationManager = navigationManager;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/proxy/{endpoint}", ct);

        await CheckForceLoginAsync(response);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    }

    public async Task<T?> PostAsync<T, TData>(string endpoint, TData body, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/proxy/{endpoint}", body, ct);

        await CheckForceLoginAsync(response);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    }
    
    private async Task CheckForceLoginAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data,
    CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/proxy/{endpoint}", data, _jsonOptions, ct);

        await CheckForceLoginAsync(response);

        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await JsonSerializer.DeserializeAsync<TResponse>(
            await response.Content.ReadAsStreamAsync(ct),
            _jsonOptions,
            ct);
    }
}
