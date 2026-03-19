namespace Domain.Models.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<CourseParticipant> Participants { get; set; } = [];
    public ICollection<Module> Modules { get; set; } = [];

}
