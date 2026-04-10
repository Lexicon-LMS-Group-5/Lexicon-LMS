using LMS.Shared.DTOs;

namespace Service.Contracts;

public interface IActivityTypeService
{
    Task<List<ActivityTypeReadDto>> GetAllActivityTypesAsync(CancellationToken ct);
    Task<ActivityTypeReadDto> GetActivityTypeAsync(int id, CancellationToken ct);
}
