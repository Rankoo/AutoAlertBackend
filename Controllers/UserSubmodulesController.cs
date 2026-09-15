using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/permissions/users")]
public class UserSubmodulesController : ControllerBase
{
    private readonly IUserSubmoduleRepository _repo;

    public UserSubmodulesController(IUserSubmoduleRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_PERMISSIONS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSubmodules>>> GetAll()
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
    [HttpGet("{userId}/{subModuleId}")]
    public async Task<ActionResult<UserSubmodules>> Get(Guid userId, Guid subModuleId)
    {
        try
        {
            var item = await _repo.GetByIdAsync(userId, subModuleId);
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
    public async Task<ActionResult<UserSubmodules>> Create(UserSubmodules userSubmodule)
    {
        try
        {
            var created = await _repo.CreateAsync(userSubmodule);
            return CreatedAtAction(nameof(Get), new { userId = created.UserId, subModuleId = created.SubModuleId }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "UPDATE_PERMISSIONS")]
    [HttpPut("{userId}/{subModuleId}")]
    public async Task<IActionResult> Update(Guid userId, Guid subModuleId, UserSubmodules userSubmodule)
    {
        try
        {
            if (userId != userSubmodule.UserId || subModuleId != userSubmodule.SubModuleId)
                return BadRequest();
            var updated = await _repo.UpdateAsync(userSubmodule);
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
    [HttpDelete("{userId}/{subModuleId}")]
    public async Task<IActionResult> Delete(Guid userId, Guid subModuleId)
    {
        try
        {
            var ok = await _repo.DeleteAsync(userId, subModuleId);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
