using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;

using Service.Contracts;

namespace LMS.Services;

public class ActivityTypeService : IActivityTypeService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public ActivityTypeService(
        IMapper mapper, IUnitOfWork unitOfWork
        )
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public async Task<List<ActivityTypeReadDto>> GetAllActivityTypesAsync(CancellationToken ct)
    {
        var activityTypes = await unitOfWork.ActivityTypes.GetAllAsync(trackChanges: false, ct);

        return mapper.Map<List<ActivityTypeReadDto>>(activityTypes);
    }

    public async Task<ActivityTypeReadDto> GetActivityTypeAsync(int id, CancellationToken ct)
    {
        var activityType = await unitOfWork.ActivityTypes.GetByIdAsync(id, trackChanges: false, ct);

        if (activityType == null) throw new NotFoundException($"Activity {id} not found");

        return mapper.Map<ActivityTypeReadDto>(activityType);
    }
}