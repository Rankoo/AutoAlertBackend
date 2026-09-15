using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/user-companies")]
public class UserCompaniesController : ControllerBase
{
    private readonly IUserCompanyRepository _repo;

    public UserCompaniesController(IUserCompanyRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_USER_COMPANIES")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCompanies>>> GetAll()
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

    [Authorize(Policy = "VIEW_USER_COMPANIES")]
    [HttpGet("{userId}/{companyId}")]
    public async Task<ActionResult<UserCompanies>> Get(Guid userId, Guid companyId)
    {
        try
        {
            var item = await _repo.GetByIdAsync(userId, companyId);
            if (item == null) return NotFound();
            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_USER_COMPANIES")]
    [HttpPost]
    public async Task<ActionResult<UserCompanies>> Create(UserCompanies userCompany)
    {
        try
        {
            var created = await _repo.CreateAsync(userCompany);
            return CreatedAtAction(nameof(Get), new { userId = created.UserId, companyId = created.CompanyId }, created);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "CREATE_USER_COMPANIES")]
    [HttpPut("{userId}/{companyId}")]
    public async Task<IActionResult> Update(Guid userId, Guid companyId, UserCompanies userCompany)
    {
        try
        {
            if (userId != userCompany.UserId || companyId != userCompany.CompanyId)
                return BadRequest();
            var updated = await _repo.UpdateAsync(userCompany);
            if (updated == null)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "DELETE_USER_COMPANIES")]
    [HttpDelete("{userId}/{companyId}")]
    public async Task<IActionResult> Delete(Guid userId, Guid companyId)
    {
        try
        {
            var ok = await _repo.DeleteAsync(userId, companyId);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
