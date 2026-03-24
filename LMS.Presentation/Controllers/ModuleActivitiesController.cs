/*
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.API;


[Route("api/[controller]")]
[ApiController]
public class ModuleActivitiesController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public ModuleActivitiesController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    // GET: api/ModuleActivities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleActivity>>> GetModuleActivities()
    {
        return await _context.ModuleActivities.ToListAsync();
    }

    // GET: api/ModuleActivities/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ModuleActivity>> GetModuleActivity(int id)
    {
        var moduleActivity = await _context.ModuleActivities.FindAsync(id);

        if (moduleActivity == null)
        {
            return NotFound();
        }

        return moduleActivity;
    }

    // PUT: api/ModuleActivities/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutModuleActivity(int id, ModuleActivity moduleActivity)
    {
        if (id != moduleActivity.Id)
        {
            return BadRequest();
        }

        _context.Entry(moduleActivity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModuleActivityExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ModuleActivities
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ModuleActivity>> PostModuleActivity(ModuleActivity moduleActivity)
    {
        _context.ModuleActivities.Add(moduleActivity);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetModuleActivity", new { id = moduleActivity.Id }, moduleActivity);
    }

    // DELETE: api/ModuleActivities/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModuleActivity(int id)
    {
        var moduleActivity = await _context.ModuleActivities.FindAsync(id);
        if (moduleActivity == null)
        {
            return NotFound();
        }

        _context.ModuleActivities.Remove(moduleActivity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModuleActivityExists(int id)
    {
        return _context.ModuleActivities.Any(e => e.Id == id);
    }
}
*/