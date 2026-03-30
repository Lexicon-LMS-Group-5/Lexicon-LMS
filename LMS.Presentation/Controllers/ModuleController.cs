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
            var result = await serviceManager.ModuleService.GetModulesByCourseIdAsync(new ModulesByCourseIdQueryDto(cid));
            return Ok(result);


        }
}
