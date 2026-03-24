using System;
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

    private int? courseId { get; set; } = null;

    private bool IsLoading { get; set; }
    private string? Error { get; set; } = null;
    private CourseDetailsDto? CourseDetails { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        // ToDo: Get courseId from logged in user
        courseId = 4;
        if (courseId is null)
            Navigation.NotFound();

        try
        {
            CourseDetails = await ApiService.GetAsync<CourseDetailsDto>($"api/courses/{courseId}");
        } catch (Exception ex)
        {
            Error = ex.Message;
        } finally
        {
            IsLoading = false;
        }
    }
}
