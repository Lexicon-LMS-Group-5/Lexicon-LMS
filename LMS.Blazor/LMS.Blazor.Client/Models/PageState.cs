namespace LMS.Blazor.Client.Models;

public class PageState<TData>(bool isLoading, TData? data, string? error = null)
{
    public bool IsLoading { get; set; } = isLoading;
    public TData? Data { get; set; } = data;
    public string? Error { get; set; } = error;
}
