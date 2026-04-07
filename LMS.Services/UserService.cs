using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Service.Contracts;

namespace LMS.Services;

public class UserService(IMapper mapper, IUnitOfWork unitOfWork) : IUserService
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct)
    {
        var users = await unitOfWork.Users.GetAllAsync(trackChanges: false, ct);

        return mapper.Map<List<UserReadDto>>(users);
    }

    public async Task<UserReadDto> GetCurrentUserAsync(string id, CancellationToken ct)
    {
        return await GetUserbyIdAsync(id, ct);
    }

    public async Task<UserReadDto> GetUserbyIdAsync(string id, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id, trackChanges: false, ct);

        return user == null ? throw new NotFoundException($"User {id} not found") : mapper.Map<UserReadDto>(user);
    }

    public async Task<UserReadDto> UpdateUserAsync(
    UpdateUserRequest request,
    string id,
    UserUpdateDto dto,
    CancellationToken ct)
    {
        if (!request.IsTeacher )//&& request.CurrentUserId != id) Later maybe a user should be able to change themselves?
            throw new UnauthorizedAccessException("You are not allowed to update this user");

        var user = await unitOfWork.Users.GetByIdAsync(id, trackChanges: true, ct) ?? throw new NotFoundException($"User {id} not found");

        mapper.Map(dto, user);

        await unitOfWork.CompleteAsync();

        return mapper.Map<UserReadDto>(user);
    }

}
