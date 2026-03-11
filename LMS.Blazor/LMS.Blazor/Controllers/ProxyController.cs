using LMS.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

[Route("api/proxy")]
[ApiController]
[Authorize]
public class ApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenStorage _tokenStorage;
    private readonly ILogger<ApiProxyController> _logger;

    public ApiProxyController(
        IHttpClientFactory httpClientFactory,
        ITokenStorage tokenStorage,
        ILogger<ApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenStorage = tokenStorage;
        _logger = logger;
    }

    [Route("{**endpoint}")]
    [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
    public async Task<IActionResult> ProxyRequest(string endpoint, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrEmpty(endpoint);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User ID not found");

        var accessToken = await _tokenStorage.GetAccessTokenAsync(userId);
        if (string.IsNullOrWhiteSpace(accessToken))
            return Unauthorized("Unable to obtain valid access token");

        try
        {
            using var response = await ForwardRequestToApiAsync(endpoint, accessToken, ct);

            // Retry approach:
            // If we get 401, try refreshing token once and retry
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // ToDo: Implement token refresh logic
                // Ask token storage for tokens
                // Call refresh endpoint
                // Store new tokens
                // Retry ForwardRequestToApiAsync(...)
                //If no success remove tokens
            }

            return await ConvertHttpResponseToActionResultAsync(response, ct);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                "Service unavailable could not reach API");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    private async Task<IActionResult> ConvertHttpResponseToActionResultAsync(HttpResponseMessage response, CancellationToken ct)
    {
        Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
            Response.Headers[header.Key] = header.Value.ToArray();

        foreach (var header in response.Content.Headers)
            Response.Headers[header.Key] = header.Value.ToArray();

        Response.Headers.Remove("transfer-encoding");

        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("API request failed. Status: {StatusCode}", response.StatusCode);

        await using var responseStream = await response.Content.ReadAsStreamAsync(ct);
        await responseStream.CopyToAsync(Response.Body, ct);

        return new EmptyResult();
    }

    private async Task<HttpResponseMessage> ForwardRequestToApiAsync(string endpoint, string accessToken, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("LmsApiClient");
        var targetUri = BuildTargetUri(client.BaseAddress!, endpoint);

        var requestMessage = new HttpRequestMessage(new HttpMethod(Request.Method), targetUri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (Request.ContentLength > 0 || Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            requestMessage.Content = new StreamContent(Request.Body);

            if (!string.IsNullOrWhiteSpace(Request.ContentType))
            {
                requestMessage.Content.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(Request.ContentType);
            }
        }

        foreach (var header in Request.Headers)
        {
            if (ShouldSkipHeader(header.Key))
                continue;

            var addedToRequestHeaders = requestMessage.Headers.TryAddWithoutValidation(
                header.Key,
                header.Value.ToArray());

            if (!addedToRequestHeaders && requestMessage.Content is not null)
            {
                requestMessage.Content.Headers.TryAddWithoutValidation(
                    header.Key,
                    header.Value.ToArray());
            }
        }

        return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct);
    }

    private Uri BuildTargetUri(Uri baseAddress, string endpoint)
    {
        var uri = new Uri(baseAddress, endpoint);
        var builder = new UriBuilder(uri);

        if (Request.QueryString.HasValue)
        {
            builder.Query = Request.QueryString.Value!.TrimStart('?');
        }

        return builder.Uri;
    }

    private static bool ShouldSkipHeader(string headerName)
    {
        var skipHeaders = new[]
        {
            "Host",
            "Authorization",
            "Connection",
            "Content-Length",
            "Transfer-Encoding"
        };

        return skipHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }
}
