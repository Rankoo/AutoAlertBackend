using System;
using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RolesController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [Authorize(Policy = "VIEW_ROLES")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Roles>>> GetRoles()
    {
        try {
            var roles = await _roleRepository.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_ROLES")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Roles>> GetRole(Guid id)
    {
        try {
            var role = await _roleRepository.GetRoleByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_ROLES")]
    [HttpPost]
    public async Task<ActionResult<Roles>> CreateRole(Roles role)
    {
        try {
            var existingRole = await _roleRepository.GetRoleByNameAsync(role.Name);
            if (existingRole != null)
                return BadRequest("A role with this name already exists");

            var createdRole = await _roleRepository.CreateRoleAsync(role);
            return CreatedAtAction(nameof(GetRole), new { id = createdRole.Id }, createdRole);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_ROLES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, Roles role)
    {
        try {
            if (id != role.Id)
                return BadRequest();

            var updatedRole = await _roleRepository.UpdateRoleAsync(role);
            if (updatedRole == null)
                return NotFound();

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_ROLES")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        try {
            var result = await _roleRepository.DeleteRoleAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_ROLES")]
    [HttpGet("name/{name}")]
    public async Task<ActionResult<Roles>> GetRoleByName(string name)
    {
        try {
            var role = await _roleRepository.GetRoleByNameAsync(name);

            if (role == null)
                return NotFound();

            return Ok(role);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}