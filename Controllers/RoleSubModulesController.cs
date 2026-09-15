using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/permissions/roles")]
public class RoleSubModulesController : ControllerBase
{
    private readonly IRoleSubModuleRepository _repo;

    public RoleSubModulesController(IRoleSubModuleRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_PERMISSIONS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleSubModules>>> GetAll()
    {
        try
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_PERMISSIONS")]
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleSubModules>> Get(Guid id)
    {
        try
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "UPDATE_PERMISSIONS")]
    [HttpPost]
    public async Task<ActionResult<RoleSubModules>> Create(RoleSubModules roleSubModule)
    {
        try
        {
            var created = await _repo.CreateAsync(roleSubModule);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "UPDATE_PERMISSIONS")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, RoleSubModules roleSubModule)
    {
        try
        {
            if (id != roleSubModule.Id)
                return BadRequest();
            var updated = await _repo.UpdateAsync(roleSubModule);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "UPDATE_PERMISSIONS")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
