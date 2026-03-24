using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Service.Contracts;
public interface IModuleActivityService
{
    Task<List<ActivityReadDto>> GetAllActivitiesAsync(CancellationToken ct);
    Task<ActivityReadDto?> GetActivityAsync(int id, CancellationToken ct);
    Task<ActivityReadDto> CreateActivityAsync(ActivityUpsertDto activityUpsertDto, CancellationToken ct);
    Task<ActivityReadDto> UpdateActivityAsync(int id, ActivityUpsertDto activityUpsertDto, CancellationToken ct);
    Task DeleteActivityAsync(int id, CancellationToken ct);
    Task<List<ActivityReadDto>> GetActivitiesByModuleIdAsync(int moduleId, CancellationToken ct);
    Task<List<ActivityReadDto>> GetActivitiesByTypeIdAsync(int typeId, CancellationToken ct);
    Task<List<ActivityReadDto>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}
