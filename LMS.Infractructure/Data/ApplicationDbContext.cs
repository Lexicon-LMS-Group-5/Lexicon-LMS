using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseParticipant> CourseParticipants { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleActivity> ModuleActivities { get; set; }
        public DbSet<ModuleActivityParticipant> ActivityParticipants { get; set; }
        public DbSet<ModuleActivityType> ModuleActivityTypes { get; set; }
        public DbSet<ModuleParticipant> ModuleParticipants { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    }
}
