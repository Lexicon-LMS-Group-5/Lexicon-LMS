using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Service.Contracts;

namespace LMS.Services;

public class ModuleActivityService : IModuleActivityService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public ModuleActivityService(
        IMapper mapper, IUnitOfWork unitOfWork
        )
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public async Task<List<ActivityReadDto>> GetAllActivitiesAsync(CancellationToken ct)
    {
        var activities = await unitOfWork.Activities.GetAllAsync(trackChanges: false, ct);

        return mapper.Map<List<ActivityReadDto>>(activities);
    }

    public async Task<ActivityReadDto> GetActivityAsync(int id, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(id, trackChanges: false, ct);

        if (activity == null) throw new NotFoundException($"Activity {id} not found");

        return mapper.Map<ActivityReadDto>(activity);
    }

    public async Task<ActivityReadDto> CreateActivityAsync(ActivityUpsertDto activityUpsertDto, CancellationToken ct)
    {
        //This should probably be in the ModuleService instead.
        throw new NotImplementedException();
    }

    public async Task<ActivityReadDto> UpdateActivityAsync(int id, ActivityUpsertDto dto, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(id, trackChanges: true, ct);

        if (activity == null) throw new NotFoundException($"Activity {id} not found");

        mapper.Map(dto, activity);

        await unitOfWork.CompleteAsync();

        return mapper.Map<ActivityReadDto>(activity);
    }

    public async Task DeleteActivityAsync(int id, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(id, trackChanges: true, ct);

        if (activity == null) throw new NotFoundException($"Activity {id} not found");

        unitOfWork.Activities.Delete(activity);

        await unitOfWork.CompleteAsync();
    }

    public async Task<List<ActivityReadDto>> GetActivitiesByModuleIdAsync(int moduleId, CancellationToken ct)
    {
        var activities = await unitOfWork.Activities.GetActivitiesByModuleIdAsync(moduleId, trackChanges: false, ct);

        return mapper.Map<List<ActivityReadDto>>(activities);
    }

    public async Task<List<ActivityReadDto>> GetActivitiesByTypeIdAsync(int typeId, CancellationToken ct)
    {
        var activities = await unitOfWork.Activities.GetActivitiesByTypeIdAsync(typeId, trackChanges: false, ct);

        return mapper.Map<List<ActivityReadDto>>(activities);
    }

    public async Task<List<ActivityReadDto>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var activities = await unitOfWork.Activities.GetActivitiesByDateRangeAsync(startDate, endDate, trackChanges: false, ct);

        return mapper.Map<List<ActivityReadDto>>(activities);
    }
}