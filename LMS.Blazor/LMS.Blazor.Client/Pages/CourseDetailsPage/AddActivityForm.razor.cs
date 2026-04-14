using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace LMS.Blazor.Client.Pages.CourseDetailsPage;

public partial class AddActivityForm
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Parameter]
    public ActivityUpsertDto? Model { get; set; }

    [Parameter]
    public EditContext EditContext { get; set; } = default!;

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
    public EventCallback<ActivityReadDto> OnSubmissionSucceeded { get; set; }

    private string? ErrorMessage { get; set; }
    private ErrorBoundary? AddActivityFormErrorBoundary { get; set; }
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
            if (Model.TimeCond == null)
                throw new Exception("Module is missing scheduling information");
            Model.EndDate = Model.StartDate + Model.TimeCond.Duration;
            EditContext.Validate();
            var result = await ApiService.PostAsync<ActivityUpsertDto, ActivityReadDto>($"api/activities", Model)
                ?? throw new Exception("Updated activity was not received");

            if (OnSubmissionSucceeded.HasDelegate)
                await OnSubmissionSucceeded.InvokeAsync(result);
        }
        catch (Exception ex)
        {
            if (OnSubmissionFailed.HasDelegate)
                await OnSubmissionFailed.InvokeAsync();

            ErrorMessage = ex.Message;
            throw;
        }
    }
}
