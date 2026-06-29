using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTOs;

public class CreateExpenseDto
{
    [Required] [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required] [Range(0.01, 1_000_000)]
    public decimal Amount { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBudgetDto
{
    [Required]
    public string Category { get; set; } = string.Empty;

    [Required] [Range(1, 10_000_000)]
    public decimal Limit { get; set; }

    [Required] [Range(1, 12)]
    public int Month { get; set; }

    [Required] [Range(2000, 2100)]
    public int Year { get; set; }
}

public class BudgetResponseDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining => Limit - Spent;
    public double PercentUsed => Limit > 0 ? (double)(Spent / Limit) * 100 : 0;
    public int Month { get; set; }
    public int Year { get; set; }
}

public class SummaryDto
{
    public decimal TotalThisMonth { get; set; }
    public decimal TotalLastMonth { get; set; }
    public decimal TotalThisYear { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
    public List<DailySpendDto> DailyTrend { get; set; } = new();
}

public class DailySpendDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
