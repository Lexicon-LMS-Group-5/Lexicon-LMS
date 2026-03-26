namespace LMS.Shared.DTOs
{
    public class ActivityTypeUpsertDto
    {
        public string Name { get; set; }
        public bool TimeExclusive { get; set; }
    }

    public class ActivityTypeReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool TimeExclusive { get; set; }
    }
}
