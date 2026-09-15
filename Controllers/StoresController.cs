using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/stores")]
public class StoresController : ControllerBase
{
    private readonly IStoreRepository _repo;

    public StoresController(IStoreRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_STORES")]
    [HttpGet("quantities")]
    public async Task<ActionResult<StoreQuantitiesDto>> GetQuantities()
    {
        try
        {
            var quantities = await _repo.GetQuantitiesAsync();
            return Ok(quantities);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "VIEW_STORES")]
    [HttpGet]
    public async Task<ActionResult<PagedStoresDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("page debe ser mayor que 0 y pageSize debe estar entre 1 y 100.");

            var stores = await _repo.GetAllAsync(page, pageSize, search);
            return Ok(stores);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "VIEW_STORES")]
    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDto>> Get(Guid id)
    {
        try
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "CREATE_STORES")]
    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create(CreateStoreDto store)
    {
        try
        {
            var created = await _repo.CreateAsync(store);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "EDIT_STORES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateStoreDto store)
    {
        try
        {
            var updated = await _repo.UpdateAsync(id, store);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "DELETE_STORES")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
