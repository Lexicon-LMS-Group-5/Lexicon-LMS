using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IActivityService> activityService;
    private Lazy<IUserService> userService;

    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IActivityService ActivityService => activityService.Value;
    public IUserService UserService => userService.Value;

    public ServiceManager(
        Lazy<IAuthService> authService, 
        Lazy<ICourseService> courseService,
        Lazy<IActivityService> activityService,
        Lazy<IUserService> userService)
    {
        this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        this.activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        this.userService = userService ?? throw new ArgumentNullException(nameof(activityService));
    }
}
