using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public NotificationsController(
        INotificationRepository repo,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _repo = repo;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<Notifications>>> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var id))
            return Unauthorized();

        return Ok(await _repo.GetByUserIdAsync(id));
    }

    [HttpPatch("mine/{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        return await _repo.MarkAsReadAsync(id, userId) ? NoContent() : NotFound();
    }

    [Authorize(Policy = "EDIT_NOTIFICATIONS")]
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAnyAsRead(Guid id)
    {
        if (!await IsAdministratorAsync())
            return Forbid();

        return await _repo.MarkAsReadAsync(id) ? NoContent() : NotFound();
    }

    [HttpPatch("mine/read")]
    public async Task<ActionResult<object>> MarkAllAsRead()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var updated = await _repo.MarkAllAsReadAsync(userId);
        return Ok(new { updated });
    }

    [Authorize(Policy = "VIEW_NOTIFICATIONS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notifications>>> GetAll()
    {
        try
        {
            if (!await IsAdministratorAsync())
                return Forbid();

            var list = await _repo.GetAllAsync();
            return Ok(list);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    private async Task<bool> IsAdministratorAsync()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return false;

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
            return false;

        var role = await _roleRepository.GetPermissionByUserAsync(user);
        return role.Role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)
            || role.Role.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
            || role.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    [Authorize(Policy = "VIEW_NOTIFICATIONS")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Notifications>> Get(Guid id)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (item.UserId != userId && !await IsAdministratorAsync())
                return NotFound();

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_NOTIFICATIONS")]
    [HttpPost]
    public async Task<ActionResult<Notifications>> Create(CreateNotificationDto request)
    {
        try
        {
            if (!await IsAdministratorAsync())
                return Forbid();

            var notification = new Notifications
            {
                AlertId = request.AlertId,
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Channel = request.Channel
            };
            var created = await _repo.CreateAsync(notification);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_NOTIFICATIONS")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Notifications notification)
    {
        try
        {
            if (id != notification.Id)
                return BadRequest();
            if (!await IsAdministratorAsync())
                return Forbid();
            var updated = await _repo.UpdateAsync(notification);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_NOTIFICATIONS")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (!await IsAdministratorAsync())
                return Forbid();

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
