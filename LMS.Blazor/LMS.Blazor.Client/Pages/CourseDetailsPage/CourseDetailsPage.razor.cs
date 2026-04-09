using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages.CourseDetailsPage;

public partial class CourseDetailsPage
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public string? CourseId { get; set; }

    [SupplyParameterFromQuery(Name = "ref")]
    public string? Referrer { get; set; }

    [SupplyParameterFromQuery(Name = "refTab")]
    public string? ReferrerTab { get; set; }

    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; } = null;
    private CourseDetailsDto? CourseDetails { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        if (!int.TryParse(CourseId, out int Id))
            Navigation.NotFound();

        try
        {
            var result = CourseDetails ?? await ApiService.GetAsync<CourseDetailsDto>($"api/courses/{Id}")
                ?? throw new Exception("Could not fetch course details");

            CourseDetails = result;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
