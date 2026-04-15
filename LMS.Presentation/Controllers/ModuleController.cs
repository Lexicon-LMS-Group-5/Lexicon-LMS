using LMS.Shared;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers
{
    [Authorize]
    [Route("api/modules")]
    [ApiController]
    [Produces("application/json")]

    public class ModuleController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public ModuleController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }
        [HttpGet("{courseId:int}")]
        [ProducesResponseType<List<ModuleReadDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModulesByCourseId([FromRoute] int courseId)
        {
            // TODO: CancellationToken ?!
            List<ModuleReadDto> module_dtos = await serviceManager
                .ModuleService.GetModulesByCourseIdAsync(courseId);

            return Ok(module_dtos);
        }
        [HttpGet("{courseId:int}/{moduleId:int}")]
        [ProducesResponseType<ModuleReadDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModuleDetails(
            [FromRoute] int courseId,
            [FromRoute] int moduleId)
        {
            ModuleReadDto module_dto = await serviceManager
                .ModuleService.GetModuleDetailsByIdAsync(moduleId, false);
            if (module_dto.CourseId != courseId)
            {
                return BadRequest(
                    $"No module with id={moduleId} in course with id={courseId}.");
            }
            return Ok(module_dto);
        }
        [Authorize(Roles = Roles.Teacher)]
        [HttpPost("{courseId:int}")]
        [ProducesResponseType<ModuleReadDto>(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateModule(
            [FromRoute] int courseId,
            [FromBody] ModuleUpsertDto dto)
        {
            if (dto.CourseId != courseId)
            {
                return BadRequest(
                    $"CourseId in body ({dto.CourseId}) does not match course id in route ({courseId}).");
            }

            ModuleReadDto createdModule = await serviceManager
                .ModuleService.CreateModuleAsync(dto);
            return CreatedAtAction(
                nameof(GetModuleDetails),
                new { courseId = createdModule.CourseId, moduleId = createdModule.Id },
                createdModule);
        }
        // PUT: api/modules/33/17
        [Authorize(Roles = Roles.Teacher)]
        [HttpPut("{courseId:int}/{moduleId:int}")]
        [ProducesResponseType<ModuleReadDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<ModuleReadDto>> Update(
            [FromRoute] int courseId,
            [FromRoute] int moduleId,
            [FromBody] ModuleUpsertDto dto,
            CancellationToken ct)
        {
            if (dto.CourseId != courseId) return BadRequest(
                $"CourseId in body ({dto.CourseId}) does not match course id in route ({courseId}).");
            ModuleReadDto updatedModule = await serviceManager
                .ModuleService.UpdateModuleAsync(moduleId, dto, ct);
            return Ok(updatedModule);
        }
        // DELETE: /api/modules/33/17
        [Authorize(Roles = Roles.Teacher)]

        [HttpDelete("{courseId:int}/{moduleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            [FromRoute] int courseId,
            [FromRoute] int moduleId,
            CancellationToken ct)
        {
            await serviceManager.ModuleService.DeleteModuleAsync(moduleId, new ModuleCourseIdDto(courseId), ct);
            return NoContent();
        }
    }
}
