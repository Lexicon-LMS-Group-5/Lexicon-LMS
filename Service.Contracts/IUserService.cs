using LMS.Shared.DTOs;
using System.Security.Claims;

namespace Service.Contracts;

public interface IUserService
{
    Task<UserReadDto> GetCurrentUserAsync(ClaimsPrincipal user, CancellationToken ct);
    Task<UserReadDto> GetUserbyIdAsync(string id, CancellationToken ct);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserReadDto> UpdateUserAsync(ClaimsPrincipal currentUser, string id, UserUpsertDto dto, CancellationToken ct);
}