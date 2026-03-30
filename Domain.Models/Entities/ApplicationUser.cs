using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }

    public int? CourseId { get; set; }
    public Course? Course { get; set; }

    [NotMapped]
    public List<string> Roles { get; set; } = new();
}
