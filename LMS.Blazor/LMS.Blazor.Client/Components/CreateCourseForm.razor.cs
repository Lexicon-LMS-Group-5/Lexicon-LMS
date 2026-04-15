using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LMS.Blazor.Client.Components;

public partial class CreateCourseForm
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [SupplyParameterFromForm]
    private CreateCourseDto? Model { get; set; }

    [Parameter]
    public string? ModalId { get; set; }

    [Parameter]
    public RenderFragment? ModalCloseButton { get; set; }

    [Parameter]
    public EventCallback<Func<IReadOnlyList<CourseListItemDto>, IReadOnlyList<CourseListItemDto>>> OnCourseListsUpdated { get; set; }

    private EditContext? editContext;

    private ErrorBoundaryBase? CreateCourseFormErrorBoundary { get; set; }

    private bool IsSaveEnabled { get; set; }

    private bool IsLoading { get; set; }

    private string? ErrorMessage;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = false;
        Model ??= new();
        editContext = new(Model);
        editContext.OnFieldChanged += HandleFieldChanged;
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (editContext != null)
        {
            IsSaveEnabled = editContext.Validate();
        }
    }

    private async Task SubmitAsync()
    {

        if (Model == null) return;

        if (editContext != null && !editContext.Validate())
        {
            throw new Exception("Form contains invalid or missing data");
        }

        IsLoading = true;

        try
        {
            var response = await ApiService.PostAsync<CreateCourseDto, CreateCourseResultDto>("api/courses", Model)
                ?? throw new Exception("No course returned");

            var newListItem = new CourseListItemDto
            {
                Id = response.Id,
                Name = response.Name,
                Description = response.Description,
                StartDate = response.StartDate,
                EndDate = response.EndDate
            };

            if (OnCourseListsUpdated.HasDelegate)
            {
                await OnCourseListsUpdated.InvokeAsync((list) => [newListItem, ..list]);
            }

            Model = new();
            editContext = new(Model);
            editContext.OnFieldChanged += HandleFieldChanged;
            IsSaveEnabled = false;
            IsLoading = false;
        }
        catch(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
