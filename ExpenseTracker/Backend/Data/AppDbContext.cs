using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Budget>  Budgets  => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Title).IsRequired().HasMaxLength(100);
            e.Property(x => x.Category).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Budget>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Limit).HasColumnType("decimal(18,2)");
            b.HasIndex(x => new { x.Category, x.Month, x.Year }).IsUnique();
        });
    }
}
