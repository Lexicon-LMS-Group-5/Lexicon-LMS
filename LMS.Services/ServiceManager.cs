using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    public IAuthService AuthService => authService.Value;

    private Lazy<IActivityService> moduleActivityService;

    public IActivityService ModuleActivityService => moduleActivityService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<IActivityService> moduleActivityService)
    {
        this.authService = authService;
        this.moduleActivityService = moduleActivityService;
    }
}
