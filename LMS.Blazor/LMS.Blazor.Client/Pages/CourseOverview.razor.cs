using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class CourseOverview
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool IsLoading { get; set; }
    private string? Error { get; set; } = null;
    private CourseDetailsDto? CourseDetails { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        try
        {
            var result = await ApiService.GetAsync<CourseDetailsDto>($"api/courses/my-course");

            if (result == null && !IsLoading)
                Navigation.NotFound();

            CourseDetails = result;
        } catch (Exception ex)
        {
            Error = ex.Message;
        } finally
        {
            IsLoading = false;
        }
    }
}
