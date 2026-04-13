
using LMS.Blazor.Client.Models;
using LMS.Blazor.Client.Services;
using LMS.Shared;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LMS.Blazor.Client.Pages;

public partial class Dashboard
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private string? Notification { get; set; } = null;

    private PageState<CoursesQueryResultDto> PageState { get; set; } = new(isLoading: false, data: null);

    private CourseDetailsDto? MyCourse { get; set; } = null;
    private PageState<CourseDetailsDto> MyCourseState { get; set; } = new(isLoading: false, data: null);
    private IReadOnlyList<CourseListItemDto>? AllCourses { get; set; } = null;
    private IReadOnlyList<CourseListItemDto>? ActiveCourses { get; set; } = null;
    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask;

        if (!authenticationState.User.IsInRole(Roles.Teacher))
        {
            Navigation.NavigateTo("my-course");
            return;
        }

        PageState = new(isLoading: true, data: null);

        try
        {
            MyCourseState = new(isLoading: true, data: null);

            var result = await ApiService.GetAsync<CourseDetailsDto>("api/courses/my-course");
                
            if (result != null)
            {
                MyCourseState = new(isLoading: false, data: result);
                MyCourse = result;
            }
        } catch
        {
            MyCourseState = new(isLoading: false, data: null, error: "Could not load your Course");
        } finally
        {
            MyCourseState = new(isLoading: false, data: null);
        }

        try
        {
            var result = await ApiService.GetAsync<CoursesQueryResultDto>("api/courses")
                ?? throw new Exception("Could not fetch Courses");
            PageState = new(isLoading: false, data: result);

            AllCourses = [.. result.Items];
            ActiveCourses = [.. result.Items.Where(c => c.EndDate > DateTime.Now)];
        }
        catch (Exception ex) {
            PageState = new(isLoading: false, data: null, error: ex.Message);
        } 
    }

    private async Task UpdateCourseListsAsync(Func<IReadOnlyList<CourseListItemDto>, IReadOnlyList<CourseListItemDto>> updateList)
    {
        AllCourses = updateList(AllCourses ?? []);
        ActiveCourses = updateList(ActiveCourses ?? [])
            .Where(c => c.EndDate > DateTime.Now).ToList();

        Notification = "Courses successfully updated";

        await Task.Delay(3000);
        Notification = null;
    }
}