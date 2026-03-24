using System.Diagnostics;

namespace LMS.Blazor.Client.Models
{
	public class Module
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public int CourseId { get; set; }

		public List<Activity> Activities { get; set; } = new();
	}
}