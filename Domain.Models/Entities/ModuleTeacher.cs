namespace Domain.Models.Entities;

public class ModuleTeacher
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    public string TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; }
}