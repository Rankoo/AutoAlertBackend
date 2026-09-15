using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/user-groups")]
public class UserGroupsController : ControllerBase
{
    private readonly IUserGroupRepository _repo;

    public UserGroupsController(IUserGroupRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_USER_GROUPS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserGroups>>> GetAll()
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

    [Authorize(Policy = "VIEW_USER_GROUPS")]
    [HttpGet("{userId}/{groupId}")]
    public async Task<ActionResult<UserGroups>> Get(Guid userId, Guid groupId)
    {
        try
        {
            var item = await _repo.GetByIdAsync(userId, groupId);
            if (item == null) return NotFound();
            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_USER_GROUPS")]
    [HttpPost]
    public async Task<ActionResult<UserGroups>> Create(UserGroups userGroup)
    {
        try
        {
            var created = await _repo.CreateAsync(userGroup);
            return CreatedAtAction(nameof(Get), new { userId = created.UserId, groupId = created.GroupId }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_USER_GROUPS")]
    [HttpPut("{userId}/{groupId}")]
    public async Task<IActionResult> Update(Guid userId, Guid groupId, UserGroups userGroup)
    {
        try
        {
            if (userId != userGroup.UserId || groupId != userGroup.GroupId)
                return BadRequest();
            var updated = await _repo.UpdateAsync(userGroup);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_USER_GROUPS")]
    [HttpDelete("{userId}/{groupId}")]
    public async Task<IActionResult> Delete(Guid userId, Guid groupId)
    {
        try
        {
            var ok = await _repo.DeleteAsync(userId, groupId);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
