using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[Authorize]
[Route("api/courses")]
[ApiController]
[Produces("application/json")]
public class CourseController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public CourseController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet()]
    [ProducesResponseType<CoursesQueryResultDto>(StatusCodes.Status200OK)]
    public async Task <IActionResult> GetAllCourses([FromQuery] CoursesQueryDto query)
    {
        var result = await serviceManager.CourseService.GetCoursesAsync(query);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<CourseDetailsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseDetails([FromRoute] int id)
    {
        var result = await serviceManager.CourseService.GetCourseDetailsAsync(new CourseDetailsQueryDto(id));

        return Ok(result);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost()]
    [ProducesResponseType<CreateCourseResultDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommandDto command)
    {
        var result = await serviceManager.CourseService.CreateCourseAsync(command);

        return Ok(result);
    }
}
