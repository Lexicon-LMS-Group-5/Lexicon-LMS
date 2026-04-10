using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs
{
    public class DateRangeRequestDto
    {
        public TimeSpan Duration { get; set; }
        public DateTime? StartingAfter { get; set; }
        public DateTime? EndingBefore { get; set; }
    }
    public class DateRangeResponseDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
