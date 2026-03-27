using LMS.Shared.DTOs;
namespace Domain.Models.Entities;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<Activity> Activities { get; set; } = [];
    public IEnumerable<ActivityReadDto> ActivityDtos => Activities.Select(a => new ActivityReadDto
    {
        Id = a.Id,
        Name = a.Name,
        Description = a.Description,
        StartDate = a.StartDate,
        EndDate = a.EndDate,
        ActivityTypeId = a.ActivityTypeId,
        ActivityTypeName = a.Type.Name,
        ActivityTypeTimeExclusive = a.Type.TimeExclusive
    });
    public ModuleReadDto Dto => new ModuleReadDto
    {
        Id = Id,
        Name = Name,
        Description = Description,
        StartDate = StartDate,
        EndDate = EndDate,
        Activities = ActivityDtos.ToList(),
        CourseId = CourseId,
        CourseName = Course.Name
    };
}
