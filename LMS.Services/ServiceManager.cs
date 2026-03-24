using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    public IAuthService AuthService => authService.Value;

    private Lazy<IModuleActivityService> moduleActivityService;

    public IModuleActivityService ModuleActivityService => moduleActivityService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<IModuleActivityService> moduleActivityService)
    {
        this.authService = authService;
        this.moduleActivityService = moduleActivityService;
    }
}
