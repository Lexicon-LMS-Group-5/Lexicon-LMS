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
    public EditCourseCommandDto? Model { get; set; }

    [Parameter]
    public RenderFragment? FormContent { get; set; }

    [Parameter]
    public RenderFragment? FormActions { get; set; }

    [Parameter]
    public EditContext EditContext { get; set; } = default!;

    [Parameter]
    public CourseDetailsDto CourseDetails { get; set; } = default!;
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }
    private ErrorBoundary? EditCourseFormErrorBoundary { get; set; }

    private async Task OnSubmit()
    {
        Console.WriteLine($"### submit: {Model?.Name}");
    }
}
