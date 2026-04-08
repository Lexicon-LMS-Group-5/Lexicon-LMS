namespace Domain.Models.Entities;

public class ActivityType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool TimeExclusive { get; set; }
    public ICollection<Activity> Activities { get; set; } = [];
}
