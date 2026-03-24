using Domain.Models.Entities;
namespace LMS.Shared.DTOs
{
    public class ModuleUpsertDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    }

    public class ModuleReadDto
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
        public List<int> ActivityIds { get; set; } = new();

        public ModuleReadDto(Module module)
        {
            Id = module.Id;
            Name = module.Name;
            Description = module.Description;
            StartDate = module.StartDate;
            EndDate = module.EndDate;
            ActivityIds = module.ActivityIds.ToList();
        }
    }
}
