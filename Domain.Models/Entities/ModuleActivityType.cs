namespace Domain.Models.Entities;

public class ModuleActivityType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool TimeExclusive { get; set; }
    public ICollection<ModuleActivity> Activities { get; set; }
}
