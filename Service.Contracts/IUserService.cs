using LMS.Shared.DTOs;

namespace Service.Contracts;

public interface IUserService
{
    //Task<UserReadDto> CreateUser(UserCreateDto user, CancellationToken ct);
    Task<UserReadDto> GetCurrentUserAsync(string id, CancellationToken ct);
    Task<UserReadDto> GetUserbyIdAsync(string id, CancellationToken ct);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct);
    Task<UserReadDto> UpdateUserAsync(UpdateUserContext request, string id, UserUpdateDto dto, CancellationToken ct);
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto, CancellationToken ct);
    Task<bool> DeleteUserByIdAsync(string id, CancellationToken ct);
}