using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertRepository _repo;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public AlertsController(
        IAlertRepository repo,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _repo = repo;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    [Authorize(Policy = "VIEW_ALERTS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alerts>>> GetAll()
    {
        try
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var list = await IsAdministratorAsync(userId)
                ? await _repo.GetAllAsync()
                : await _repo.GetByUserIdAsync(userId);
            return Ok(list);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "VIEW_ALERTS")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Alerts>> Get(Guid id)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (!await IsAdministratorAsync(userId))
            {
                var allowed = (await _repo.GetByUserIdAsync(userId)).Any(alert => alert.Id == id);
                if (!allowed)
                    return NotFound();
            }
            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "VIEW_ALERTS")]
    [HttpGet("scheduled")]
    public async Task<ActionResult<IEnumerable<Alerts>>> GetScheduledAlerts([FromQuery] DateTime? fromDate = null)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var list = await IsAdministratorAsync(userId)
                ? await _repo.GetScheduledAlertsAsync(fromDate)
                : (await _repo.GetByUserIdAsync(userId))
                    .Where(alert => !fromDate.HasValue || alert.DueDate >= fromDate.Value)
                    .OrderBy(alert => alert.DueDate);
            return Ok(list);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "CREATE_ALERTS")]
    [HttpPost]
    public async Task<ActionResult<Alerts>> Create(Alerts alert)
    {
        try
        {
            if (!await IsCurrentUserAdministratorAsync())
                return Forbid();

            var created = await _repo.CreateAsync(alert);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "EDIT_ALERTS")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Alerts alert)
    {
        try
        {
            if (id != alert.Id)
                return BadRequest();
            if (!await IsCurrentUserAdministratorAsync())
                return Forbid();

            var updated = await _repo.UpdateAsync(alert);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "VIEW_ALERTS")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateAlertStatusDto request)
    {
        var allowedStatuses = new[] { "Pendiente", "Pagado", "Vencido" };
        if (!allowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Estado de alerta inválido." });

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (!await IsAdministratorAsync(userId))
        {
            var canAccess = (await _repo.GetByUserIdAsync(userId)).Any(alert => alert.Id == id);
            if (!canAccess)
                return NotFound();
        }

        var status = allowedStatuses.First(value => value.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        return await _repo.UpdateStatusAsync(id, status) is null ? NotFound() : NoContent();
    }

    [Authorize(Policy = "DELETE_ALERTS")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (!await IsCurrentUserAdministratorAsync())
                return Forbid();

            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    private async Task<bool> IsCurrentUserAdministratorAsync()
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            && await IsAdministratorAsync(userId);
    }

    private async Task<bool> IsAdministratorAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
            return false;

        var role = await _roleRepository.GetPermissionByUserAsync(user);
        return role.Role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)
            || role.Role.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
            || role.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}
