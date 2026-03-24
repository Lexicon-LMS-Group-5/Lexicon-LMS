namespace Domain.Models.Entities;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    public required int CourseId { get; set; }
    public required Course Course { get; set; }
    public ICollection<ModuleActivity> Activities { get; set; } = [];
}
