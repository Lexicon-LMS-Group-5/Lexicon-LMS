using LMS.Shared.DTOs;
using System.Security.Claims;

namespace Service.Contracts;

public interface IUserService
{
    Task<UserReadDto> GetCurrentUserAsync(string id, CancellationToken ct);
    Task<UserReadDto> GetUserbyIdAsync(string id, CancellationToken ct);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserReadDto> UpdateUserAsync(UpdateUserRequest request, string id, UserUpdateDto dto, CancellationToken ct);
}