namespace LMS.Shared.DTOs
{
    public class ActivityUpsertDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ModuleId { get; set; }  // Required for Activity creation.
        public DateRangeRequestDto? TimeCond { get; set; }

        //public int ActivityTypeId { get; set; }
        //If you can change the type, we need to make sure that this is not possible when switching from a time-exclusive type to a non-time-exclusive type, or vice versa.
        //If we allow changing the activity type without restrictions, it could lead to scheduling conflicts.
        //Therefore, we should implement validation logic to prevent changing the activity type in a way that would violate the time exclusivity.
    }

    public class ActivityReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; } = "";
        public bool ActivityTypeTimeExclusive { get; set; }
    }
}
