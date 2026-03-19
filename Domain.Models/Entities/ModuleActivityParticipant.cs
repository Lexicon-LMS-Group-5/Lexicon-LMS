namespace Domain.Models.Entities;

public class ModuleActivityParticipant
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public ModuleActivity Activity { get; set; }
    public string ParticipantId { get; set; }
    public ApplicationUser Participant { get; set; }
}