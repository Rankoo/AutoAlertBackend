using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/services")]
public class ServicesController(
    IServiceRepository repo,
    IUserRepository userRepository,
    IRoleRepository roleRepository) : ControllerBase
{
    [HttpGet("catalogs")]
    public async Task<ActionResult<ServiceCatalogsDto>> GetCatalogs()
    {
        if (!await CanViewServicesAsync()) return Forbid();
        return Ok(await repo.GetCatalogsAsync());
    }

    [HttpGet("quantities")]
    public async Task<ActionResult<ServiceQuantitiesDto>> GetQuantities()
    {
        if (!await CanViewServicesAsync()) return Forbid();
        return Ok(await repo.GetQuantitiesAsync());
    }

    [HttpGet]
    public async Task<ActionResult<PagedServicesDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] Guid? storeId = null)
    {
        if (!await CanViewServicesAsync()) return Forbid();
        if (page < 1 || pageSize is < 1 or > 100) return BadRequest("page debe ser mayor que 0 y pageSize debe estar entre 1 y 100.");
        return Ok(await repo.GetAllAsync(page, pageSize, search, storeId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> Get(Guid id)
    {
        if (!await CanViewServicesAsync()) return Forbid();
        var item = await repo.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [Authorize(Policy = "CREATE_SERVICES")]
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create(CreateServiceDto service)
    {
        try { var created = await repo.CreateAsync(service); return CreatedAtAction(nameof(Get), new { id = created.Id }, created); }
        catch (InvalidOperationException e) { return BadRequest(e.Message); }
    }

    [Authorize(Policy = "EDIT_SERVICES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateServiceDto service)
    {
        try { return await repo.UpdateAsync(id, service) is null ? NotFound() : NoContent(); }
        catch (InvalidOperationException e) { return BadRequest(e.Message); }
    }

    [Authorize(Policy = "DELETE_SERVICES")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { return await repo.DeleteAsync(id) ? NoContent() : NotFound(); }
        catch (InvalidOperationException e) { return BadRequest(e.Message); }
    }

    private async Task<bool> CanViewServicesAsync()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return false;

        var user = await userRepository.GetUserByIdAsync(userId);
        if (user is null)
            return false;

        var role = await roleRepository.GetPermissionByUserAsync(user);
        return role.SpecialPermissions?.Contains("VIEW_SERVICES") == true
            || role.Role.Equals("USER", StringComparison.OrdinalIgnoreCase)
            || role.Role.Equals("USUARIO", StringComparison.OrdinalIgnoreCase);
    }
}
