namespace Domain.Models.Entities;

public class ModuleParticipant
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public Module Module { get; set; }
    public string ParticipantId { get; set; }
    public ApplicationUser Participant { get; set; }
}
