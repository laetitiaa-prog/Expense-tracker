using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto);
    Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(int? month, int? year, string? category);
    Task<ExpenseResponseDto?> GetByIdAsync(int id);
    Task<ExpenseResponseDto?> UpdateAsync(int id, CreateExpenseDto dto);
    Task<bool> DeleteAsync(int id);
    Task<SummaryDto> GetSummaryAsync();
    Task<BudgetResponseDto> SetBudgetAsync(CreateBudgetDto dto);
    Task<IEnumerable<BudgetResponseDto>> GetBudgetsAsync(int month, int year);
}

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;

    public ExpenseService(AppDbContext db) => _db = db;

    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Title     = dto.Title.Trim(),
            Amount    = dto.Amount,
            Category  = dto.Category,
            Date      = dto.Date.ToUniversalTime(),
            Note      = dto.Note?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return Map(expense);
    }

    public async Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(int? month, int? year, string? category)
    {
        var query = _db.Expenses.AsQueryable();

        if (month.HasValue) query = query.Where(e => e.Date.Month == month.Value);
        if (year.HasValue)  query = query.Where(e => e.Date.Year  == year.Value);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(e => e.Category == category);

        return (await query.OrderByDescending(e => e.Date).ToListAsync()).Select(Map);
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(int id)
    {
        var e = await _db.Expenses.FindAsync(id);
        return e is null ? null : Map(e);
    }

    public async Task<ExpenseResponseDto?> UpdateAsync(int id, CreateExpenseDto dto)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense is null) return null;

        expense.Title    = dto.Title.Trim();
        expense.Amount   = dto.Amount;
        expense.Category = dto.Category;
        expense.Date     = dto.Date.ToUniversalTime();
        expense.Note     = dto.Note?.Trim();

        await _db.SaveChangesAsync();
        return Map(expense);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense is null) return false;
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<SummaryDto> GetSummaryAsync()
    {
        var now       = DateTime.UtcNow;
        var allExpenses = await _db.Expenses.ToListAsync();

        var thisMonth = allExpenses.Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year);
        var lastMonth = allExpenses.Where(e => e.Date.Month == now.AddMonths(-1).Month
                                            && e.Date.Year  == now.AddMonths(-1).Year);
        var thisYear  = allExpenses.Where(e => e.Date.Year  == now.Year);

        // Daily trend for current month (last 30 days)
        var dailyTrend = thisMonth
            .GroupBy(e => e.Date.Date)
            .Select(g => new DailySpendDto
            {
                Date   = g.Key.ToString("MMM dd"),
                Amount = g.Sum(e => e.Amount)
            })
            .OrderBy(d => d.Date)
            .ToList();

        return new SummaryDto
        {
            TotalThisMonth = thisMonth.Sum(e => e.Amount),
            TotalLastMonth = lastMonth.Sum(e => e.Amount),
            TotalThisYear  = thisYear.Sum(e => e.Amount),
            ByCategory     = thisMonth
                                .GroupBy(e => e.Category)
                                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount)),
            DailyTrend     = dailyTrend
        };
    }

    public async Task<BudgetResponseDto> SetBudgetAsync(CreateBudgetDto dto)
    {
        var existing = await _db.Budgets.FirstOrDefaultAsync(b =>
            b.Category == dto.Category && b.Month == dto.Month && b.Year == dto.Year);

        if (existing is null)
        {
            existing = new Budget
            {
                Category = dto.Category,
                Limit    = dto.Limit,
                Month    = dto.Month,
                Year     = dto.Year
            };
            _db.Budgets.Add(existing);
        }
        else
        {
            existing.Limit = dto.Limit;
        }

        await _db.SaveChangesAsync();
        return await BuildBudgetResponse(existing);
    }

    public async Task<IEnumerable<BudgetResponseDto>> GetBudgetsAsync(int month, int year)
    {
        var budgets = await _db.Budgets
            .Where(b => b.Month == month && b.Year == year)
            .ToListAsync();

        var results = new List<BudgetResponseDto>();
        foreach (var b in budgets)
            results.Add(await BuildBudgetResponse(b));

        return results;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private async Task<BudgetResponseDto> BuildBudgetResponse(Budget b)
    {
        var spent = await _db.Expenses
            .Where(e => e.Category == b.Category
                     && e.Date.Month == b.Month
                     && e.Date.Year  == b.Year)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;

        return new BudgetResponseDto
        {
            Id       = b.Id,
            Category = b.Category,
            Limit    = b.Limit,
            Spent    = spent,
            Month    = b.Month,
            Year     = b.Year
        };
    }

    private static ExpenseResponseDto Map(Expense e) => new()
    {
        Id        = e.Id,
        Title     = e.Title,
        Amount    = e.Amount,
        Category  = e.Category,
        Date      = e.Date,
        Note      = e.Note,
        CreatedAt = e.CreatedAt
    };
}
