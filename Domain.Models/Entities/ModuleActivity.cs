namespace Domain.Models.Entities;

public class ModuleActivity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required int ModuleActivityTypeId { get; set; }
    public required ModuleActivityType Type { get; set; }
    public required int ModuleId { get; set; }
    public required Module Module { get; set; }
}