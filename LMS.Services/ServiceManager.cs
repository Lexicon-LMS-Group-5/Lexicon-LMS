using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> authService;
    private readonly Lazy<ICourseService> courseService;
    private readonly Lazy<IModuleService> moduleService;
    private readonly Lazy<IActivityService> activityService;
    private readonly Lazy<IActivityTypeService> activityTypeService;
    private readonly Lazy<IUserService> userService;

    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IModuleService ModuleService => moduleService.Value;
    public IActivityService ActivityService => activityService.Value;
    public IActivityTypeService ActivityTypeService => activityTypeService.Value;
    public IUserService UserService => userService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService, 
        Lazy<ICourseService> courseService,
        Lazy<IModuleService> moduleService,
        Lazy<IActivityService> activityService,
        Lazy<IActivityTypeService> activityTypeService,
        Lazy<IUserService> userService)
    {
        this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        this.moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
        this.activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        this.activityTypeService = activityTypeService ?? throw new ArgumentNullException(nameof(activityService));
        this.userService = userService ?? throw new ArgumentNullException(nameof(activityService));
    }
}
