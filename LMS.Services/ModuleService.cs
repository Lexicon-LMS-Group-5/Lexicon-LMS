using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Domain.Models.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using System.Data;

namespace LMS.Services
{
    public class ModuleService : IModuleService
    {
        // Injected with AddScoped().
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ModuleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ModuleReadDto> CreateModuleAsync(ModuleUpsertDto dto, CancellationToken ct = default)
        {
            Course? course = await unitOfWork
                .Courses
                .GetCourseDetailsByIdAsync(dto.CourseId, true, ct);
            if (course == null) throw new CourseNotFoundException(dto.CourseId);

            if (dto.StartDate == null || dto.EndDate == null)
                throw new BadRequestException("Start and End dates are required for Module creation");

            if (dto.EndDate < dto.StartDate)
                throw new BadRequestException("The module's End date must be after the End date");

            if (course.Modules.Count > 0)
            {
                var lastModule = course.Modules.LastOrDefault();
                if (lastModule != null && lastModule.EndDate > dto.StartDate)
                    throw new BadRequestException("The new module must begin after the current last module in the course");
            }

            var module = mapper.Map<Module>(dto);

            unitOfWork.Modules.Create(module);
            course.Modules.Add(module);

            if (course.EndDate < module.EndDate)
                course.EndDate = module.EndDate;

            await unitOfWork.CompleteAsync(ct);
            return mapper.Map<ModuleReadDto>(module);
        }

        public async Task<ModuleReadDto> UpdateModuleAsync(
            int id,
            ModuleUpsertDto dto,
            CancellationToken ct = default)
        {
            Module? module = await unitOfWork
                .Modules
                .GetModuleDetailsByIdAsync(id, trackChanges: true, ct);
            if (module == null) throw new NotFoundException($"ModuleId={id}");
            if (module!.CourseId != dto.CourseId) throw new BadRequestException(
                $"No ModuleId={id} under CourseId={dto.CourseId}");
            DateRangeHelper courseDRH = new (module!.Course);
            DateRangeHelper moduleDRH = new(module!);
            StartEnd oldInt = new(module);
            mapper.Map(dto, module);
            StartEnd newInt = new(module, persistent: false);
            courseDRH.CheckIntervalChange(oldInt, newInt);
            moduleDRH.CheckNewBounds(newInt);
            await unitOfWork.CompleteAsync(ct);
            return mapper.Map<ModuleReadDto>(module);
        }
        public async Task<ModuleReadDto> GetModuleDetailsByIdAsync(
            int moduleId,
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            Module? module = await unitOfWork
                .Modules
                .GetModuleDetailsByIdAsync(moduleId, trackChanges, ct);
            if (module == null)
            {
                throw new NotFoundException($"ModuleId={moduleId}.");
            }

			return mapper.Map<ModuleReadDto>(module);
        }
        public async Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            var modules = await unitOfWork.Modules.GetModulesByCourseIdAsync(
                    courseId,
                    trackChanges,
                    ct);

            return mapper.Map<List<ModuleReadDto>>(modules);
        }

        public async Task DeleteModuleAsync(int moduleId, ModuleCourseIdDto dto, CancellationToken ct = default)
        {
            Module? module = await unitOfWork.Modules.GetModuleDetailsByIdAsync(moduleId, true, ct);
            if (module == null) throw new NotFoundException($"ModuleId={moduleId}");
            if (module!.CourseId != dto.CourseId) throw new BadRequestException(
                $"No ModuleId={moduleId} under CourseId0{dto.CourseId}");
            unitOfWork.Modules.Delete(module);
            await unitOfWork.CompleteAsync(ct);

        }
    }
}
