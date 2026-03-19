namespace Domain.Models.Entities;

public class CourseParticipant
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<CourseParticipant> Participants { get; set; }
}
