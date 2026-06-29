namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Budget
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
