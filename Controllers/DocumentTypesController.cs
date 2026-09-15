using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/document-types")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeRepository _repo;

    public DocumentTypesController(IDocumentTypeRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_DOCUMENT_TYPES")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentTypes>>> GetAll()
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

    [Authorize(Policy = "VIEW_DOCUMENT_TYPES")]
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentTypes>> Get(Guid id)
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

    [Authorize(Policy = "CREATE_DOCUMENT_TYPES")]
    [HttpPost]
    public async Task<ActionResult<DocumentTypes>> Create(DocumentTypes documentType)
    {
        try
        {
            var created = await _repo.CreateAsync(documentType);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "EDIT_DOCUMENT_TYPES")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, DocumentTypes documentType)
    {
        try
        {
            if (id != documentType.Id)
                return BadRequest();
            var updated = await _repo.UpdateAsync(documentType);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_DOCUMENT_TYPES")]
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
