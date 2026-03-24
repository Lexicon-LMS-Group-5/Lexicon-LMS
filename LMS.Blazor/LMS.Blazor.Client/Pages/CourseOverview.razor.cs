using System;
using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class CourseOverview
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Parameter]
    public string CourseId { get; set; } = string.Empty;

    private int? courseId { get; set; } = null;

    private CourseDetailsDto? CourseDetails { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        CourseDetails = await ApiService.GetAsync<CourseDetailsDto>("/api/courses/4", new CancellationToken());
    }
}
