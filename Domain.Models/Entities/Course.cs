namespace Domain.Models.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; } = default;
    public DateTime EndDate { get; set; } = default;
    public ICollection<ApplicationUser> Participants { get; set; } = [];
    public ICollection<Module> Modules { get; set; } = [];
    public IEnumerable<int> ModuleIds => Modules.Select(m => m.Id);
}
