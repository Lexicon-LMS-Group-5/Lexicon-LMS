using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _serviceManager.CourseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _serviceManager.CourseService.GetCourseAsync(id);
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseUpsertDto courseDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var createdCourse = await _serviceManager.CourseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpsertDto courseDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _serviceManager.CourseService.UpdateCourseAsync(id, courseDto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await _serviceManager.CourseService.DeleteCourseAsync(id);
            return NoContent();
        }
    }
}
