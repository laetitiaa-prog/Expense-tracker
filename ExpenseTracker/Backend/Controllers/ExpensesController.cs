using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.DTOs;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _svc;
    public ExpensesController(IExpenseService svc) => _svc = svc;

    // POST api/expenses
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET api/expenses?month=5&year=2026&category=Food
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? category)
    {
        var results = await _svc.GetAllAsync(month, year, category);
        return Ok(results);
    }

    // GET api/expenses/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // PUT api/expenses/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateExpenseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE api/expenses/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _svc.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // GET api/expenses/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _svc.GetSummaryAsync();
        return Ok(summary);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IExpenseService _svc;
    public BudgetsController(IExpenseService svc) => _svc = svc;

    // POST api/budgets
    [HttpPost]
    public async Task<IActionResult> SetBudget([FromBody] CreateBudgetDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.SetBudgetAsync(dto);
        return Ok(result);
    }

    // GET api/budgets?month=5&year=2026
    [HttpGet]
    public async Task<IActionResult> GetBudgets([FromQuery] int? month, [FromQuery] int? year)
    {
        var now = DateTime.UtcNow;
        var result = await _svc.GetBudgetsAsync(month ?? now.Month, year ?? now.Year);
        return Ok(result);
    }
}
