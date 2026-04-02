using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Domain.Models.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

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
                throw new NotFoundException($"Module not found for moduleId={moduleId}.");
            }
            return mapper.Map<ModuleReadDto>(module);
        }
        public async Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            return mapper.Map<List<ModuleReadDto>>(
                await unitOfWork.Modules.GetModulesByCourseIdAsync(
                    courseId, 
                    trackChanges,
                    ct));
        }
    }
}
