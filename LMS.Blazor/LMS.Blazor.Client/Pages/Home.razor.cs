
using LMS.Blazor.Client.Models;
using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LMS.Blazor.Client.Pages;

public partial class Home
{
    [CascadingParameter]
    private Task<AuthenticationState> authenticationStateTask { get; set; } = default!;

    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private PageState<CoursesQueryResultDto> PageState { get; set; } = new(isLoading: true, data: null);
    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await authenticationStateTask;

        if (!authenticationState.User.IsInRole("Teacher"))
        {
            Navigation.NavigateTo("my-course");
        } else
        {
            PageState = new(isLoading: true, data: null);

            try
            {
                var result = await ApiService.GetAsync<CoursesQueryResultDto>("api/courses")
                    ?? throw new Exception("Could not fetch Courses");
                PageState = new(isLoading: false, data: result);
            }
            catch (Exception ex) {
                PageState = new(isLoading: false, data: null, error: ex.Message);
            } 
        }
    }
}