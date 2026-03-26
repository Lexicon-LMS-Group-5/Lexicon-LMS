using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public interface IUserService
{
    Task<UserReadDto> GetCurrentUserAsync(ClaimsPrincipal user, CancellationToken ct);
    Task<UserReadDto> GetUserAsync(string id, CancellationToken ct);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserReadDto> UpdateUserAsync(string id, UserUpsertDto dto, CancellationToken ct);
}