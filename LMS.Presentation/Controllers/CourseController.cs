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
public partial class CourseController : ControllerBase
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
        var result = await serviceManager.CourseService.CreateCourseAsync(command);

        return CreatedAtRoute("GetCourseDetails", new { id = result.Id }, result);
    }
    // PUT api/courses/5
    [Authorize(Roles = "Teacher")]
    [HttpPut("{cid:int}")]
    [ProducesResponseType<CourseReadDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CourseReadDto>> Update(
        [FromRoute] int cid,
        [FromBody] CourseUpsertDto dto,
        CancellationToken ct)
    {
        CourseReadDto updatedCourse = await serviceManager
            .CourseService.UpdateCourseAsync(cid, dto, ct);
        return Ok(updatedCourse);
    }

}
