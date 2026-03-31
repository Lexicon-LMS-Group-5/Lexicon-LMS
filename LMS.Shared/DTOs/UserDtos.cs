namespace LMS.Shared.DTOs;

public class UserReadDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int CourseId { get; set; }
    public CourseReadDto Course { get; set; }
    public List<string> Roles { get; set; }
}

public class UserUpsertDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int? CourseId { get; set; }
    public List<string> Roles { get; set; }
}