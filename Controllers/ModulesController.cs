using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleRepository _repo;

    public ModulesController(IModuleRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_MODULES")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Modules>>> GetAll()
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

    [Authorize(Policy = "VIEW_MODULES")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Modules>> Get(Guid id)
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

    [Authorize(Policy = "CREATE_MODULES")]
    [HttpPost]
    public async Task<ActionResult<Modules>> Create(Modules module)
    {
        try
        {
            var created = await _repo.CreateAsync(module);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_MODULES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Modules module)
    {
        try
        {
            if (id != module.Id)
                return BadRequest();
            var updated = await _repo.UpdateAsync(module);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_MODULES")]
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
