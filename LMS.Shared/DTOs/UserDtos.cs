using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs;

public class UserReadDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public CourseReadDto? Course { get; set; }
    public List<string> Roles { get; set; } = [];
}

public class UserCreateDto
{
    [Required]
    [MinLength(1)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public int? CourseId { get; set; }

    public List<string> Roles { get; set; } = null!;
}

public class UserUpdateDto
{
    [Required]
    [MinLength(1)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

public class UpdateUserContext
{
    [Required]
    public string CurrentUserId { get; set; } = string.Empty;
    public bool IsTeacher { get; set; }
}