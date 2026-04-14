using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Domain.Models.Entities;

using Service.Contracts;

namespace LMS.Services;

public class ActivityService : IActivityService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public ActivityService(
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
        Module? module = await unitOfWork.Modules.GetModuleDetailsByIdAsync(
            activityUpsertDto.ModuleId, true, ct);
        if (module == null) throw new NotFoundException($"ModuleId={activityUpsertDto.ModuleId}");

        DateRangeHelper drh = new(module);
        if (DateRangeHelper.Absent(activityUpsertDto.StartDate)
            || DateRangeHelper.Absent(activityUpsertDto.EndDate))
        {
            if (activityUpsertDto.TimeCond == null) throw new BadRequestException("time parameters");
        }
        var timeResp = drh.GetDateRange(activityUpsertDto.TimeCond!);
        if (timeResp != null)
        {
            activityUpsertDto.StartDate = DateRangeHelper.OneOf(activityUpsertDto.StartDate, timeResp.Start);
            activityUpsertDto.EndDate = DateRangeHelper.OneOf(activityUpsertDto.EndDate, timeResp.End);
        }
        Activity activity = mapper.Map<Activity>(activityUpsertDto);
		activity.Module = module;

		StartEnd newStartEnd = new(activity);
        drh.CheckNew(newStartEnd);

        //var course = await unitOfWork.Courses.GetCourseDetailsByIdAsync(
        //    module.CourseId, trackChanges: true, ct) ?? throw new CourseNotFoundException(module.CourseId);

        //if (activityUpsertDto.EndDate < activityUpsertDto.StartDate)
        //    throw new BadRequestException("The module's End date must be after the End date");

        //if (module.Activities.Count > 0)
        //{
        //    var lastModule = module.Activities.LastOrDefault();
        //    if (lastModule != null && lastModule.EndDate > activityUpsertDto.StartDate)
        //        throw new BadRequestException("The new module must begin after the current last module in the course");
        //}

        //var activity = mapper.Map<Activity>(activityUpsertDto);

        //unitOfWork.Activities.Create(activity);
        //module.Activities.Add(activity);

        //if (module.EndDate < activityUpsertDto.EndDate)
        //    module.EndDate = activityUpsertDto.EndDate;

        //var moduleIndex = course.Modules.ToList().IndexOf(module);

        //foreach (var item in course.Modules.Select((module, index) => (module, index)))
        //{

        //    if (item.module != module && moduleIndex < item.index)
        //    {

        //    }
        //}


        //if (course.EndDate < module.EndDate)
        //    course.EndDate = module.EndDate;

        await unitOfWork.CompleteAsync(ct);
        return mapper.Map<ActivityReadDto>(activity);
    }

    public async Task<ActivityReadDto> UpdateActivityAsync(int id, ActivityUpsertDto dto, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(id, trackChanges: true, ct);
        if (activity == null) throw new NotFoundException($"ActivityId={id}");
        if (activity.ModuleId != dto.ModuleId) throw new BadRequestException(
            $"No ActivityId={id} under ModuleId={dto.ModuleId}");
        Module? module = await unitOfWork.Modules.GetModuleDetailsByIdAsync(
            dto.ModuleId, true, ct);
        if (module == null) throw new NotFoundException($"ModuleId={dto.ModuleId}");
        StartEnd oldInt = new(activity);
        DateRangeHelper drh = new(module);
        mapper.Map(dto, activity);
        StartEnd newInt = new(activity, persistent: false);
        drh.CheckIntervalChange(oldInt, newInt);
        await unitOfWork.CompleteAsync(ct);

        return mapper.Map<ActivityReadDto>(activity);
    }

    public async Task DeleteActivityAsync(int id, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(id, trackChanges: true, ct);

        if (activity == null) throw new NotFoundException($"Activity {id} not found");

        unitOfWork.Activities.Delete(activity);

        await unitOfWork.CompleteAsync(ct);
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