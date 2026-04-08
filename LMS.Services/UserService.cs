using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly string defaultPassword;

    public UserService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.userManager = userManager;
        defaultPassword = configuration["password"]!;
    }

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

        await unitOfWork.CompleteAsync(ct);

        return mapper.Map<UserReadDto>(user);
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto, CancellationToken ct)
    {
        // Map DTO to ApplicationUser
        var user = mapper.Map<ApplicationUser>(dto);
        user.UserName = user.Email;
        var result = await userManager.CreateAsync(user, defaultPassword);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (dto.Roles != null && dto.Roles.Any())
        {
            foreach (var role in dto.Roles)
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                    throw new Exception(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        return mapper.Map<UserReadDto>(user);
    }
}
