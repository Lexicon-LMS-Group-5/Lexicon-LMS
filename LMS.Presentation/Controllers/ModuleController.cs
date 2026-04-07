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
        [HttpGet("{cid:int}")]
        [ProducesResponseType<List<ModuleReadDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModulesByCourseId([FromRoute] int cid)
        {
            // TODO: CancellationToken ?!
            List<ModuleReadDto> module_dtos = await serviceManager
                .ModuleService.GetModulesByCourseIdAsync(cid);
            return Ok(module_dtos);
        }
        [HttpGet("{cid:int}/{mid:int}")]
        [ProducesResponseType<ModuleReadDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModuleDetails(
            [FromRoute] int cid,
            [FromRoute] int mid)
        {
            ModuleReadDto module_dto = await serviceManager
                .ModuleService.GetModuleDetailsByIdAsync(mid, false);
            if (module_dto.CourseId != cid)
            {
                return BadRequest(
                    $"No module with id={mid} in course with id={cid}.");
            }
            return Ok(module_dto);
        }
        [HttpPost("{cid:int}")]
        [ProducesResponseType<ModuleReadDto>(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateModule(
            [FromRoute] int cid,
            [FromBody] ModuleUpsertDto dto)
        {
            if (dto.CourseId != cid)
            {
                return BadRequest(
                    $"CourseId in body ({dto.CourseId}) does not match course id in route ({cid}).");
            }
            ModuleReadDto createdModule = await serviceManager
                .ModuleService.CreateModuleAsync(dto);
            return CreatedAtAction(
                nameof(GetModuleDetails),
                new { cid = createdModule.CourseId, mid = createdModule.Id },
                createdModule);
        }
    }
}