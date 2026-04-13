using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace LMS.Blazor.Client.Pages.CourseDetailsPage;

public partial class CourseDetailsPage
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter]
    public string? CourseId { get; set; }

    [SupplyParameterFromQuery(Name = "ref")]
    public string? Referrer { get; set; }

    [SupplyParameterFromQuery(Name = "refTab")]
    public string? ReferrerTab { get; set; }

    private bool IsLoading { get; set; } = true;
    private string? ErrorMessage { get; set; }
    private CourseDetailsDto? CourseDetails { get; set; }

    private EditContext? EditContext { get; set; }
    private UpdateCourseCommandDto? EditCourseModel { get; set; }

    private const string EditCourseModalId = "editCourseFormModal";

    private bool IsFormLoading { get; set; }
    private void OnEditCourseSubmissionRequested() => IsFormLoading = true;
    private void OnEditCourseSubmissionFailed() => IsFormLoading = false;
    private async Task OnEditCourseSubmissionCanceledAsync()
    {
        IsFormLoading = false;
        await HideModalAsync();
    }
    private async Task OnEditCourseSubmissionSucceededAsync(CourseDetailsDto updatedCourseDetails)
    {
        CourseDetails = updatedCourseDetails;
        IsFormLoading = false;
        await HideModalAsync();
    }

    private async Task ShowModalAsync()
    {
        await JsRuntime.InvokeVoidAsync("showModal", EditCourseModalId);
    }
    private async Task HideModalAsync()
    {
        await JsRuntime.InvokeVoidAsync("hideModal", EditCourseModalId);
        
        if (CourseDetails != null)
        {
            EditCourseModel = new(CourseDetails!);
            EditContext = new(EditCourseModel);  
        }
    }

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
            EditCourseModel ??= new(CourseDetails);
            EditContext ??= new(EditCourseModel);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
