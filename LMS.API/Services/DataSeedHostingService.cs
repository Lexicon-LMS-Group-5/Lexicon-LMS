using Bogus;
using Humanizer;
using LMS.Infractructure.Data;
using LMS.Shared;
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
    private const string DemoTeacherEmail = "teacher@test.com";
    private const string DemoStudentEmail = "student@test.com";
    private const string DemoCourseName = "Demo Course";
    private readonly List<ActivityType> ActivityTypes;
    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;

        ActivityTypes = [
            new ActivityType()
            {
                Name = "Assignment",
            },
            new ActivityType()
            {
                Name = "E-learning",
            },
            new ActivityType()
            {
                Name = "Lecture",
                TimeExclusive = true,
            }
        ];
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Uncomment to drop current database and re-seed with mock data
        // await context.Database.EnsureDeletedAsync(cancellationToken);
        // await context.Database.MigrateAsync(cancellationToken);

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            // Add initial ActivityTypes if none exist
            if (!await context.ActivityTypes.AnyAsync())
                await AddInitialActivityTypesToDbAsync(cancellationToken);
            
            // Add roles, demo users and mock users if none exist
            if (!await context.Users.AnyAsync(cancellationToken))
            {
                await AddRolesAsync([Roles.Teacher, Roles.Student]);
                await AddDemoUsersAsync();
                await AddUsersAsync(20);
            }

            // Add mock Courses if none exist
            if (!await context.Courses.AnyAsync())
                await AddMockCoursesToDbAsync(3, scope, cancellationToken);

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

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, Roles.Teacher);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, Roles.Student);
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
            await userManager.AddToRoleAsync(user, Roles.Student);
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

    private async Task AddInitialActivityTypesToDbAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.ActivityTypes.AddRangeAsync([
            new ActivityType()
            {
                Name = "Assignment",
            },
            new ActivityType()
            {
                Name = "E-learning",
            },
            new ActivityType()
            {
                Name = "Lecture",
                TimeExclusive = true,
            }
        ]);

        // ToDo: Use unitOfWork?
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task AddMockCoursesToDbAsync(int count, IServiceScope scope, CancellationToken cancellationToken)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var faker = new Faker();
        var courseGenerator = CreateCourseGenerator();

        var courses = courseGenerator.UseSeed(Seed).Generate(count);

        foreach (var course in courses)
        {
            var moduleGenerator = await CreateModuleGeneratorAsync(scope);
            course.Modules = moduleGenerator.Generate(faker.Random.Int(3, 8));
            SetCourseIntervals(course);
        }

        await context.Courses.AddRangeAsync(courses, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Faker<Activity>> CreateActivityGeneratorAsync(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var activityTypes = await context.ActivityTypes.ToListAsync();
        var activityGenerator = new Faker<Activity>()
            .Rules((f, a) =>
            {
                a.Name = f.Company.Bs().ApplyCase(LetterCasing.Title);
                a.Description = f.Hacker.Phrase().ApplyCase(LetterCasing.Sentence);
                a.ActivityType = f.PickRandom(activityTypes);
            });
        return activityGenerator;
    }

    private static async Task<Faker<Module>> CreateModuleGeneratorAsync(IServiceScope scope)
    {
        var activityGenerator = await CreateActivityGeneratorAsync(scope);

        var moduleGenerator = new Faker<Module>()
            .Rules((f, m) =>
            {
                m.Name = $"{f.Hacker.Adjective()} {f.Hacker.IngVerb()}".ApplyCase(LetterCasing.Title);
                m.Description = f.Company.Bs().ApplyCase(LetterCasing.Sentence);
                m.Activities = activityGenerator.UseSeed(Seed).Generate(f.Random.Int(3, 8));
            });

        return moduleGenerator;
    }

    private static  Faker<Course> CreateCourseGenerator()
    {
        var courseGenerator = new Faker<Course>()
            .Rules((f, c) =>
            {
                c.Name = $"{f.Hacker.Adjective()} {f.Hacker.IngVerb()}".ApplyCase(LetterCasing.Title);
                c.Description = f.Company.Bs().ApplyCase(LetterCasing.Sentence);
                c.StartDate = f.Date.Recent(30);
            });

        return courseGenerator;
    }
    private async Task AddDemoCourseToDbAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var students = await userManager.GetUsersInRoleAsync(Roles.Student);
        var demoTeacher = await userManager.FindByEmailAsync(DemoTeacherEmail) ?? throw new Exception("Demo Teacher was not found");
        var demoStudent = await userManager.FindByEmailAsync(DemoStudentEmail) ?? throw new Exception("Demo Student was not found");

        var faker = new Faker();
        var courseStartDate = faker.Date.Soon(30);
        var courseGenerator = CreateCourseGenerator();
        courseGenerator.Rules((f, c) => {
            c.Name = DemoCourseName;
            c.Description = "A demo course to help with development of Lexicon LMS";
            c.StartDate = courseStartDate;
        });

        var demoCourse = courseGenerator.UseSeed(Seed).Generate(1).First();

        var moduleGenerator = await CreateModuleGeneratorAsync(scope);
        demoCourse.Modules = moduleGenerator.UseSeed(Seed).Generate(4);

        SetCourseIntervals(demoCourse);

        demoCourse.Participants.Add(demoTeacher);

        foreach (var student in students)
        {
            demoCourse.Participants.Add(student);
        }

        await context.Courses.AddAsync(demoCourse, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void SetCourseIntervals(Course course)
    {
        var faker = new Faker();
        var start = course.StartDate;

        foreach (var module in course.Modules)
        {
            module.StartDate = start;
            TimeSpan activityTimeOffest = new();

            foreach (var activity in module.Activities)
            {
                activity.StartDate = start.Add(activityTimeOffest);
                activity.EndDate = activity.StartDate.AddHours(faker.Random.Int(1, 4));
                start = activity.EndDate.AddDays(faker.Random.Int(0, 14));
                System.Diagnostics.Debug.Assert(activity.EndDate > activity.StartDate);
            }
            activityTimeOffest = faker.Date.Timespan();

            module.EndDate = module.Activities.LastOrDefault()?.EndDate ?? module.StartDate;
            System.Diagnostics.Debug.Assert(module.EndDate > module.StartDate);
            start = module.EndDate.AddMonths(1);
        }

        course.EndDate = course.Modules.LastOrDefault()?.EndDate ?? course.StartDate;
        System.Diagnostics.Debug.Assert(course.EndDate > course.StartDate);
    }
}
