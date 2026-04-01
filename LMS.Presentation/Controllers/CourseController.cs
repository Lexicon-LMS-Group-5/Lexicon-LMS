using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[Authorize]
[Route("api/courses")]
[ApiController]
[Produces("application/json")]
public class CourseController : ControllerBase
{
    private readonly IServiceManager serviceManager;
    private readonly ILogger<CourseController> logger;

    public CourseController(
        IServiceManager serviceManager, 
        ILogger<CourseController> logger)
    {
        this.serviceManager = serviceManager;
        this.logger = logger;
    }

    [HttpGet()]
    [ProducesResponseType<CoursesQueryResultDto>(StatusCodes.Status200OK)]
    public async Task <IActionResult> GetAllCourses([FromQuery] CoursesQueryDto query)
    {
        var result = await serviceManager.CourseService.GetCoursesAsync(query);

        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "GetCourseDetails")]
    [ProducesResponseType<CourseDetailsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseDetails([FromRoute] int id)
    {
        var result = await serviceManager.CourseService.GetCourseDetailsAsync(new CourseDetailsQueryDto(id));

        return Ok(result);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost()]
    [ProducesResponseType<CreateCourseResultDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateCourseCommandDto>> CreateCourse([FromBody] CreateCourseCommandDto command)
    {
        // Get the user ID and add it to the DTO
        command.CreatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        var result = await serviceManager.CourseService.CreateCourseAsync(command);

        return CreatedAtRoute("GetCourseDetails", new { id = result.Id }, result);
    }
}
