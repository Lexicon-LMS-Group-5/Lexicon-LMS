using Bogus;
using Humanizer;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services;

//You need all this for JWT to work :) 
//User Secrets Json
//Important to have secretkey inside same key "JwtSettings" as used in appsettings.json for get both sections!!!!
//{
//     "password": "YourSecretPasswordHere",
//     "JwtSettings": {
//        "secretkey": "ThisMustBeReallyLong!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
//        }
//}
public class DataSeedHostingService : IHostedService
{
    private const int Seed = 89634;
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataSeedHostingService> logger;
    private UserManager<ApplicationUser> userManager = null!;
    private RoleManager<IdentityRole> roleManager = null!;
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            if (!await context.ModuleActivityTypes.AnyAsync())
                await AddInitialModuleActivityTypesToDbAsync(cancellationToken);
            
            if (!await context.Users.AnyAsync(cancellationToken))
            {
                await AddRolesAsync([TeacherRole, StudentRole]);
                await AddDemoUsersAsync();
                await AddUsersAsync(20);
            }

            if (!await context.Courses.AnyAsync())
                await AddMockCoursesToDbAsync(3, cancellationToken);
            
            logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            logger.LogError($"Data seed fail with error: {ex.Message}");
            throw;
        }
    }

    private async Task AddRolesAsync(string[] rolenames)
    {
        foreach (string rolename in rolenames)
        {
            if (await roleManager.RoleExistsAsync(rolename)) continue;
            var role = new IdentityRole { Name = rolename };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }
    private async Task AddDemoUsersAsync()
    {
        var teacher = new ApplicationUser
        {
            FirstName = "Tes",
            LastName = "Ting",
            UserName = "teacher@test.com",
            Email = "teacher@test.com"
        };
        
        var student = new ApplicationUser
        {
            FirstName = "Yobayer",
            LastName = "Jobayer",
            UserName = "student@test.com",
            Email = "student@test.com"
        };

        await AddUserToDb([teacher, student]);

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, TeacherRole);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, StudentRole);
        if (!studentRoleResult.Succeeded) throw new Exception(string.Join("\n", studentRoleResult.Errors));
    }

    private async Task AddUsersAsync(int nrOfUsers)
    {
        var faker = new Faker<ApplicationUser>("sv").Rules((f, e) =>
        {
            e.FirstName = f.Person.FirstName;
            e.LastName = f.Person.LastName;
            e.Email = f.Person.Email;
            e.UserName = f.Person.Email;
        });

        await AddUserToDb(faker.Generate(nrOfUsers));
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }

    private async Task AddInitialModuleActivityTypesToDbAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.ModuleActivityTypes.AddRangeAsync([
            new ModuleActivityType()
            {
                Name = "Assignment",
            },
            new ModuleActivityType()
            {
                Name = "E-learning",
            },
            new ModuleActivityType()
            {
                Name = "Lecture",
                TimeExclusive = true,
            }
        ]);

        // ToDo: Use unitOfWork?
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task AddMockCoursesToDbAsync(int count, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Randomizer.Seed = new Random(Seed);

        var courseGenerator = new Faker<Course>()
            .Rules((f, c) =>
            {
                var startDate = f.Date.Recent(30);
                var endDate = startDate.AddMonths(6);

                c.Name = $"{f.Hacker.Adjective()} {f.Hacker.IngVerb()}".ApplyCase(LetterCasing.Title);
                c.Description = f.Company.Bs().ApplyCase(LetterCasing.Sentence);
                c.StartDate = startDate;
                c.EndDate = endDate;
            });

        await context.Courses.AddRangeAsync(courseGenerator.Generate(count));

        // ToDo: Use unitOfWork?
        await context.SaveChangesAsync(cancellationToken);
    }
}
