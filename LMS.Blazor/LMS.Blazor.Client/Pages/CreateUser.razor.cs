using LMS.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Pages;

public partial class CreateUser
{
    private UserCreateDto model = new()
    {
        Roles = new List<string>()
    };

    private string? SelectedRole;

    // Mock data (replace later)
    private List<CourseReadDto> Courses = new()
    {
    new CourseReadDto { Id = 1, Name = "Math" },
    new CourseReadDto { Id = 2, Name = "Science" },
    new CourseReadDto { Id = 3, Name = "History" }
    };

    private List<string> AvailableRoles = new()
    {
    "Student",
    "Teacher"
    };

    private async Task HandleValidSubmit()
    {
        model.Roles = string.IsNullOrEmpty(SelectedRole)
            ? new List<string>()
            : new List<string> { SelectedRole };

        // TODO: Call your API here
        // await Http.PostAsJsonAsync("api/users", model);
    }

    private void OnRoleChanged(string role)
    {
        SelectedRole = role;
    }
}
