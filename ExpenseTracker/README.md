# SPNDX — Expense Tracker

A full-stack expense tracker with a C# ASP.NET Core backend and a custom dark-mode frontend.

## Features
- Add, edit, delete expenses
- Category tagging with color coding
- Monthly budgets with progress bars and overspend warnings
- Dashboard with live bar chart and donut chart
- Filter expenses by month and category
- Works in demo mode when API is offline

## Project Structure
```
ExpenseTracker/
├── Backend/
│   ├── Controllers/ExpensesController.cs   ← API endpoints
│   ├── Data/AppDbContext.cs                ← Entity Framework + SQLite
│   ├── DTOs/ExpenseDtos.cs                 ← Request/Response shapes
│   ├── Models/Expense.cs                   ← DB models
│   ├── Services/ExpenseService.cs          ← Business logic
│   ├── Program.cs                          ← App entry point
│   └── ExpenseTracker.csproj
└── Frontend/
    └── index.html                          ← Single-file frontend
```

## API Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET    | /api/expenses | Get all (filter by ?month=&year=&category=) |
| POST   | /api/expenses | Add expense |
| GET    | /api/expenses/{id} | Get one |
| PUT    | /api/expenses/{id} | Update |
| DELETE | /api/expenses/{id} | Delete |
| GET    | /api/expenses/summary | Dashboard stats + charts data |
| GET    | /api/budgets | Get budgets (?month=&year=) |
| POST   | /api/budgets | Set/update a budget |

## How to Run

### Backend
```bash
cd Backend
dotnet restore
dotnet run
# Runs on http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Frontend
Open `Frontend/index.html` in your browser directly, or use VS Code Live Server.
The frontend auto-detects if the API is online and falls back to demo mode.
