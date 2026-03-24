using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LMS.Infractructure.Repositories;

public class ModuleActivityRepository : RepositoryBase<ModuleActivity>, IModuleActivityRepository
{
    public ModuleActivityRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<ModuleActivity>> GetAllAsync(bool trackChanges, CancellationToken ct) =>
        await FindAll(trackChanges).ToListAsync();

    public async Task<ModuleActivity?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(x => x.Id == id, trackChanges)
            .FirstOrDefaultAsync();
    public async Task<List<ModuleActivity>> GetActivitiesByModuleIdAsync(int moduleId, bool trackChanges, CancellationToken ct) =>
       await FindByCondition(a => a.ModuleId == moduleId, trackChanges).ToListAsync();
    public async Task<List<ModuleActivity>> GetActivitiesByTypeIdAsync(int typeId, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.ModuleActivityTypeId == typeId).ToListAsync();
    public async Task<List<ModuleActivity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.StartDate >= startDate && a.EndDate <= endDate).ToListAsync();
}
