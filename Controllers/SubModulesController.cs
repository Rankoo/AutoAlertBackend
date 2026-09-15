using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/submodules")]
public class SubModulesController : ControllerBase
{
    private readonly ISubModuleRepository _repo;

    public SubModulesController(ISubModuleRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_SUBMODULES")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubModules>>> GetAll()
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

    [Authorize(Policy = "VIEW_SUBMODULES")]
    [HttpGet("{id}")]
    public async Task<ActionResult<SubModules>> Get(Guid id)
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

    [Authorize(Policy = "CREATE_SUBMODULES")]
    [HttpPost]
    public async Task<ActionResult<SubModules>> Create(SubModules subModule)
    {
        try
        {
            var created = await _repo.CreateAsync(subModule);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_SUBMODULES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, SubModules subModule)
    {
        try
        {
            if (id != subModule.Id)
                return BadRequest();
            var updated = await _repo.UpdateAsync(subModule);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_SUBMODULES")]
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
