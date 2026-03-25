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
    private const string DemoTeacherEmail = "teacher@test.com";
    private const string DemoStudentEmail = "student@test.com";
    private const string DemoCourseName = "Demo Course";

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

        // // Uncomment to drop current database and re-seed with mock data
        // await context.Database.EnsureDeletedAsync(cancellationToken);
        // await context.Database.MigrateAsync(cancellationToken);

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            // Add initial ModuleActivityTypes if none exist
            if (!await context.ModuleActivityTypes.AnyAsync())
                await AddInitialModuleActivityTypesToDbAsync(cancellationToken);
            
            // Add roles, demo users and mock users if none exist
            if (!await context.Users.AnyAsync(cancellationToken))
            {
                await AddRolesAsync([TeacherRole, StudentRole]);
                await AddDemoUsersAsync();
                await AddUsersAsync(20);
            }

            // Add mock Courses if none exist
            if (!await context.Courses.AnyAsync())
                await AddMockCoursesToDbAsync(3, cancellationToken);

            // Add demo Course if it does not exist
            if (await context.Courses.FirstOrDefaultAsync(c => c.Name == DemoCourseName, cancellationToken) is null)
            {
                await AddDemoCourseToDbAsync(scope, cancellationToken);
            }

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
            UserName = DemoTeacherEmail,
            Email = DemoTeacherEmail
        };
        
        var student = new ApplicationUser
        {
            FirstName = "Yobayer",
            LastName = "Jobayer",
            UserName = DemoStudentEmail,
            Email = DemoStudentEmail
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

        var fakeUsers = faker.Generate(nrOfUsers);
        
        await AddUserToDb(fakeUsers);

        foreach (var user in fakeUsers)
        {
            await userManager.AddToRoleAsync(user, StudentRole);
        }
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

        await context.Courses.AddRangeAsync(courseGenerator.UseSeed(Seed).Generate(count));

        // ToDo: Use unitOfWork?
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddDemoCourseToDbAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var students = await userManager.GetUsersInRoleAsync(StudentRole);
        var demoTeacher = await userManager.FindByEmailAsync(DemoTeacherEmail) ?? throw new Exception("Demo Teacher was not found");
        var demoStudent = await userManager.FindByEmailAsync(DemoStudentEmail) ?? throw new Exception("Demo Student was not found");
        var moduleActivityTypes = await context.ModuleActivityTypes.ToListAsync(cancellationToken);

        var faker = new Faker();
        var courseStartDate = faker.Date.Soon(30);
        DateTime courseEndDate;

        var timeOffset = courseStartDate;
        var moduleActivityIteration = 0;
        var moduleActivityGenerator = new Faker<ModuleActivity>()
            .Rules((f, a) =>
            {
                var startTime = timeOffset;
                var endTime = startTime.AddHours(f.Random.Int(1,4));

                a.Name = f.Company.Bs().ApplyCase(LetterCasing.Title);
                a.Description = f.Hacker.Phrase().ApplyCase(LetterCasing.Sentence);
                a.StartDate = startTime;
                a.EndDate = f.Date.Soon(refDate: startTime);
                a.Type = f.PickRandom(moduleActivityTypes);

                timeOffset = endTime.AddDays(moduleActivityIteration++);
            });

        var moduleIteration = 0;
        var moduleGenerator = new Faker<Module>()
            .Rules((f, m) =>
            {
                var startDate = courseStartDate.AddMonths(moduleIteration++);
                courseEndDate = startDate.AddMonths(1);

                m.Name = $"{f.Hacker.Adjective()} {f.Hacker.IngVerb()}".ApplyCase(LetterCasing.Title);
                m.Description = f.Company.Bs().ApplyCase(LetterCasing.Sentence);
                m.StartDate = startDate;
                m.EndDate = courseEndDate;
                m.Activities = moduleActivityGenerator.UseSeed(Seed).Generate(f.Random.Int(3,8));
            });

        var courseGenerator = new Faker<Course>()
            .Rules((f, c) => {
                var startDate = courseStartDate;
                var endDate = startDate.AddMonths(6);
                var modules = moduleGenerator.UseSeed(Seed).Generate(4);

                c.Name = DemoCourseName;
                c.Description = "A demo course to help with development of Lexicon LMS";
                c.StartDate = startDate;
                c.Modules = modules;
                c.EndDate = modules.Last().EndDate;
            });

        var demoCourse = courseGenerator.UseSeed(Seed).Generate(1).First();
        demoCourse.Participants.Add(demoTeacher);

        foreach (var student in students)
        {
            demoCourse.Participants.Add(student);
        }

        await context.Courses.AddAsync(demoCourse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
