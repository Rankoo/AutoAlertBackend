using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAlertBackEnd.Controllers;

[Authorize]
[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogRepository _repo;

    public LogsController(ILogRepository repo)
    {
        _repo = repo;
    }

    [Authorize(Policy = "VIEW_LOGS")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Logs>>> GetAll([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                var list = await _repo.GetByDateRangeAsync(startDate.Value, endDate.Value);
                return Ok(list);
            }
            
            var allLogs = await _repo.GetAllAsync();
            return Ok(allLogs);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "VIEW_LOGS")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Logs>> Get(Guid id)
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

    [Authorize(Policy = "VIEW_LOGS")]
    [HttpGet("table/{tableName}")]
    public async Task<ActionResult<IEnumerable<Logs>>> GetByTableName(string tableName)
    {
        try
        {
            var list = await _repo.GetByTableNameAsync(tableName);
            return Ok(list);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    // NOTA: CreateLog y DeleteLog NO tienen políticas porque los logs
    // son gestionados automáticamente por el sistema, no por usuarios
}
