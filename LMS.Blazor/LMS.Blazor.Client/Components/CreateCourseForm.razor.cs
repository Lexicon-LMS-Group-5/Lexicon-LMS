using LMS.Blazor.Client.Models;
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
    private CreateCourseCommandDto? FormData { get; set; }

    [Parameter]
    public string? ModalId { get; set; }

    [Parameter]
    public RenderFragment? ModalCloseButton { get; set; }

    [Parameter]
    public EventCallback<Func<IReadOnlyList<CourseListItemDto>, IReadOnlyList<CourseListItemDto>>> OnCourseListsUpdated { get; set; }

    private EditContext? editContext;

    private ErrorBoundaryBase? CreateCourseFormErrorBoundary { get; set; }

    private PageState<CreateCourseResultDto> LoadingState { get; set; } = default!;

    private string? Notification { get; set; } = null;

    private bool IsSaveEnabled { get; set; }

    protected override async Task OnInitializedAsync()
    {
        LoadingState = new(isLoading: false, data: null);
        FormData ??= new();
        editContext = new(FormData);
        editContext.OnFieldChanged += HandleFieldChanged;
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (editContext != null)
        {
            IsSaveEnabled = editContext.Validate();
            StateHasChanged();
        }


    }

    private async Task SubmitAsync()
    {
        if (FormData == null) return;

        LoadingState = new(isLoading: true, data: null);

        try
        {
            var response = await ApiService.PostAsync<CreateCourseResultDto, CreateCourseCommandDto>("api/courses", FormData)
                ?? throw new Exception("No course returned");
            LoadingState = new(isLoading: false, data: response);

            var newListItem = new CourseListItemDto
            {
                Id = response.Id,
                Name = response.Name,
                Description = response.Description,
                StartDate = response.StartDate,
                EndDate = response.EndDate
            };

            FormData = new();
            IsSaveEnabled = false;

            if (OnCourseListsUpdated.HasDelegate)
            {
                await OnCourseListsUpdated.InvokeAsync((list) => [newListItem, .. list]);
            }
        }
        catch (Exception ex)
        {
            LoadingState = new(isLoading: false, data: null, error: ex.Message);
            throw ex;
        }
    }
}
