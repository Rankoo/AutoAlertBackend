using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserCatalogRepository _userCatalogRepository;

    public UsersController(
        IUserRepository userRepository,
        IUserCatalogRepository userCatalogRepository)
    {
        _userRepository = userRepository;
        _userCatalogRepository = userCatalogRepository;
    }

    [Authorize(Policy = "VIEW_USERS")]
    [HttpGet("catalogs")]
    public async Task<ActionResult<UserCatalogsDto>> GetUserCatalogs()
    {
        try
        {
            var catalogs = await _userCatalogRepository.GetUserCatalogsAsync();
            return Ok(catalogs);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_USERS")]
    [HttpGet]
    public async Task<ActionResult<PagedUsersDto>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? roleId = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        try {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("page debe ser mayor que 0 y pageSize debe estar entre 1 y 100.");

            var users = await _userRepository.GetAllUsersAsync(
                page,
                pageSize,
                roleId,
                search,
                isActive);
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_USERS")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetUser(Guid id)
    {
        try {

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_USERS")]
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser(CreateUserDto newUser)
    {
        try {
            var existingUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null)
                return BadRequest("El usuario ya se encuentra registrado");

            var createdUser = await _userRepository.CreateUserAsync(newUser);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_USERS")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto user)
    {
        try {
            var updatedUser = await _userRepository.UpdateUserAsync(id, user);
            if (updatedUser == null)
                return NotFound();

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_USERS")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try {
            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_USERS")]
    [HttpGet("email/{email}")]
    public async Task<ActionResult<Users>> GetUserByEmail(string email)
    {
        try {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_USERS")]
    [HttpGet("quantities")]
    public async Task<ActionResult<UserQuantitiesDto>> GetUsersQuantities()
    {
        try {
            var quantities = await _userRepository.GetUserQuantitiesAsync();
            return Ok(quantities);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}