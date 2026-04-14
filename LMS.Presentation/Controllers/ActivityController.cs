using LMS.Shared;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.API.Controllers;
// TODO: FIX THIS
[ApiController]
[Route("api/activities")]
public class ActivityController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public ActivityController(IServiceManager service)
    {
        serviceManager = service;
    }

    //This one will probably never be used.
    // GET: api/activities
    [HttpGet]
    public async Task<ActionResult<List<ActivityReadDto>>> GetAll(CancellationToken ct)
    {
        var activities = await serviceManager.ActivityService.GetAllActivitiesAsync(ct);
        return Ok(activities);
    }

    // GET: api/activities/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActivityReadDto>> GetById(int id, CancellationToken ct)
    {
        var activity = await serviceManager.ActivityService.GetActivityAsync(id, ct);
        return Ok(activity);
    }

    // This should probably be in the ModuleController.
    //// GET: api/activities/module/3
    //[HttpGet("module/{moduleId:int}")]
    //public async Task<ActionResult<List<ActivityReadDto>>> GetByModuleId(int moduleId, CancellationToken ct)
    //{
    //    var activities = await _service.GetActivitiesByModuleIdAsync(moduleId, ct);
    //    return Ok(activities);
    //}

    // GET: api/activities/type/2
    [HttpGet("type/{typeId:int}")]
    public async Task<ActionResult<List<ActivityReadDto>>> GetByTypeId(int typeId, CancellationToken ct)
    {
        var activities = await serviceManager.ActivityService.GetActivitiesByTypeIdAsync(typeId, ct);
        return Ok(activities);
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<ActivityTypeReadDto>>> GetAllTypes(int typeId, CancellationToken ct)
    {
        var types = await serviceManager.ActivityTypeService.GetAllActivityTypesAsync(ct);
        return Ok(types);
    }

    // GET: api/activities/daterange?startDate=...&endDate=...
    [HttpGet("daterange")]
    public async Task<ActionResult<List<ActivityReadDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct)
    {
        var activities = await serviceManager.ActivityService.GetActivitiesByDateRangeAsync(startDate, endDate, ct);
        return Ok(activities);
    }

    // POST: api/activities
    [Authorize(Roles = Roles.Teacher)]
    [HttpPost]
    public async Task<ActionResult<ActivityReadDto>> Create(
        [FromBody] ActivityUpsertDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await serviceManager.ActivityService.CreateActivityAsync(dto, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // PUT: api/activities/5
    [Authorize(Roles = Roles.Teacher)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ActivityReadDto>> Update(
        int id,
        [FromBody] ActivityUpsertDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await serviceManager.ActivityService.UpdateActivityAsync(id, dto, ct);
        return Ok(updated);
    }

    // DELETE: api/activities/5
    [Authorize(Roles = Roles.Teacher)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await serviceManager.ActivityService.DeleteActivityAsync(id, ct);
        return NoContent();
    }
}