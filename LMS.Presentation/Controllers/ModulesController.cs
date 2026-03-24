using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Presentation.Controllers
{
    internal class ModulesController : ControllerBase
    {
        private IModuleRepository? _repo;

        static IEnumerable<ModuleReadDto> GetModuleDTOs(IQueryable<Module> modules)
        {
            foreach (var m in modules.ToList())
            {
                yield return new ModuleReadDto(m);
            }
        }

        [Route("api/modules/{cid?}/{mid?}")]
        public async Task<IActionResult> GetModules(int? cid, int? mid)
        {
            if (_repo == null || cid == null) {  return NotFound(); }
            int courseId = cid.Value;
            var modules = _repo.GetModules(courseId, false);
            if (mid == null)
            {
                return Ok(GetModuleDTOs(modules));
            }
            return Ok(GetModuleDTOs(modules.Where(m => m.Id == mid.Value)));
        }
    }
}
