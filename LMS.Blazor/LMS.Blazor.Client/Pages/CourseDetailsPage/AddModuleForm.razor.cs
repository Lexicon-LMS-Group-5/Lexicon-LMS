using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace LMS.Blazor.Client.Pages.CourseDetailsPage;

public partial class AddModuleForm
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Parameter]
    public ModuleUpsertDto? Model { get; set; }

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
    public EventCallback<CourseModuleListItemDto> OnSubmissionSucceeded { get; set; }

    private string? ErrorMessage { get; set; }
    private ErrorBoundary? AddModuleFormErrorBoundary { get; set; }
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
            Console.WriteLine($"### duration: {Model.TimeCond?.Duration}");
            if (Model.TimeCond == null)
                throw new Exception("Module is missing scheduling information");
            Model.EndDate = Model.StartDate + Model.TimeCond.Duration;
            EditContext.Validate();
            var result = await ApiService.PostAsync<ModuleUpsertDto, ModuleReadDto>($"api/modules/{Model.CourseId}", Model)
                ?? throw new Exception("Updated course was not received");

            var newModule = new CourseModuleListItemDto
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                Activities = result.Activities,
            };

            if (OnSubmissionSucceeded.HasDelegate)
                await OnSubmissionSucceeded.InvokeAsync(newModule);
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
