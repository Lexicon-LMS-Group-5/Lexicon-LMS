using Domain.Contracts.Repositories;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Domain.Models.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    public class ModuleService : IModuleService
    {
        // Injected with AddScoped().
        private readonly IUnitOfWork unitOfWork;
        public async Task<ModuleReadDto> GetModuleDetailsByIdAsync(
            int moduleId,
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            Module? module = await unitOfWork.ModuleRepository.GetModuleDetailsByIdAsync(moduleId, trackChanges, ct);
            if (module == null)
            {
                throw new NotFoundException($"Module not found for moduleId={moduleId}.");
            }
            return module.Dto;
        }
        static IEnumerable<ModuleReadDto> MapModulesToModuleReadDtos(List<Module> modules)
        {
            foreach (Module module in modules)
            {
                yield return module.Dto;
            }
        }
        public async Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false,
            CancellationToken ct = default)
        {
            return MapModulesToModuleReadDtos(
                await unitOfWork.ModuleRepository.GetModulesByCourseIdAsync(
                    courseId, 
                    trackChanges,
                    ct)).ToList();
        }
    }
}
