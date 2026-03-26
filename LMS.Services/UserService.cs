using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace LMS.Services;

internal class UserService : IUserService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public UserService(
        IMapper mapper, IUnitOfWork unitOfWork
        )
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync(CancellationToken ct)
    {
        var users = await unitOfWork.Users.GetAllAsync(trackChanges: false, ct);

        return mapper.Map<List<UserReadDto>>(users);
    }

    public async Task<UserReadDto> GetCurrentUserAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        if (user.FindFirst(ClaimTypes.NameIdentifier) == null) throw new NotFoundException($"Not logged in!");

        return await GetUserAsync(user.FindFirst(ClaimTypes.NameIdentifier)!.Value, ct);
    }

    public async Task<UserReadDto> GetUserAsync(string id, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id, trackChanges: false, ct);

        if (user == null) throw new NotFoundException($"Activity {id} not found");

        return mapper.Map<UserReadDto>(user);
    }

    public async Task<UserReadDto> UpdateUserAsync(string id, UserUpsertDto dto, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id, trackChanges: true, ct);

        if (user == null) throw new NotFoundException($"Activity {id} not found");

        mapper.Map(dto, user);

        await unitOfWork.CompleteAsync();

        return mapper.Map<UserReadDto>(user);
    }
}
