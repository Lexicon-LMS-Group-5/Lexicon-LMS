using LMS.Blazor.Client.Services;
using LMS.Shared;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class CourseOverview
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool IsLoading { get; set; }
    private string? Error { get; set; } = null;
    private CourseDetailsDto? CourseDetails { get; set; } = null;

    private readonly List<CourseParticipantDto> students = [];

    private readonly List<CourseParticipantDto> teachers = [];

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        students.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));

        try
        {
            var result = await ApiService.GetAsync<CourseDetailsDto>($"api/courses/my-course");

            if (result == null) { 
            Navigation.NotFound();
                return;
            }

            CourseDetails = result;

            foreach (var participant in CourseDetails.Participants)
            {
                if (participant.Roles == null) continue;

                if (participant.Roles.Contains(Roles.Student))
                    students.Add(participant);

                if (participant.Roles.Contains(Roles.Teacher))
                    teachers.Add(participant);
            }

        } catch (Exception ex)
        {
            Error = ex.Message;
        } finally
        {
            IsLoading = false;
        }
    }
}
