using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace LMS.Blazor.Client.Components;

public partial class EditCourseForm
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Parameter]
    public CourseUpdateDto? Model { get; set; }

    [Parameter]
    public RenderFragment? FormContent { get; set; }

    [Parameter]
    public RenderFragment? FormActions { get; set; }

    [Parameter]
    public EventCallback OnSubmissionRequested { get; set; }
    
    [Parameter]
    public EventCallback OnSubmissionFailed { get; set; }
    
    [Parameter]
    public EventCallback OnSubmissionCanceled { get; set; }
    
    [Parameter]
    public EventCallback<CourseDetailsDto> OnSubmissionSucceeded { get; set; }

    [Parameter]
    public EditContext EditContext { get; set; } = default!;

    [Parameter]
    public CourseDetailsDto CourseDetails { get; set; } = default!;
    
    private string? ErrorMessage { get; set; }
    private ErrorBoundary? EditCourseFormErrorBoundary { get; set; }
    private async Task SubmitAsync()
    {
        if (Model == null || !EditContext.IsModified())
        {
            if (OnSubmissionCanceled.HasDelegate)
                await OnSubmissionCanceled.InvokeAsync();
            return;
        }

        if (OnSubmissionRequested.HasDelegate)
            await OnSubmissionRequested.InvokeAsync();

        try
        {
            EditContext.Validate();
            var result = await ApiService.PutAsync<CourseUpdateDto, CourseDetailsDto>($"api/courses/{Model.Id}", Model)
                ?? throw new Exception("Updated course was not received");

            
            if (OnSubmissionSucceeded.HasDelegate)
                await OnSubmissionSucceeded.InvokeAsync(result);            
        } catch(Exception ex)
        {
            ErrorMessage = ex.Message;
            if (OnSubmissionFailed.HasDelegate)
                await OnSubmissionFailed.InvokeAsync();
        }
    }
}
