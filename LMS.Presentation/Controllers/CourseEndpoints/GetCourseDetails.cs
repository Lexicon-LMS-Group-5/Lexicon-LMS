using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers.CourseEndpoints
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public partial class CourseController(IServiceManager serviceManager) : ControllerBase
    {
        private readonly ICourseService courseService = serviceManager.CourseService;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseDetails([FromRoute] int id)
        {
            var result = await courseService.GetCourseDetailsAsync(new CourseDetailsQueryDto(id));

            return Ok(result);
        }
    }
}
