using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "ExpenseTracker API", Version = "v1" }));

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=expenses.db"));

builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddCors(opt => opt.AddPolicy("Dev", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Auto-create DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Dev");
app.UseAuthorization();
app.MapControllers();
app.Run();
