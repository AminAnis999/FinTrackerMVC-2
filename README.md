# FinTracker – Deliverable 3

ASP.NET Core MVC personal finance management website. This version extends Deliverable 2 while preserving its existing Bootstrap UI, pages, navigation style, dashboard cards, and transaction forms.

## Technology

- ASP.NET Core MVC – .NET 10
- SQL Server LocalDB – `FinTrackerDb`
- ASP.NET Core Identity – registration, login, logout, password hashing, and user accounts
- Entity Framework Core – Identity tables only
- Dapper – finance transaction queries
- Repository Pattern – `ITransactionRepository` and `TransactionRepository`
- Dependency Injection – repository registered as a scoped service
- Bootstrap 5.3

## Architecture

- `Controllers/` – MVC controllers.
- `Models/` – transaction and view models.
- `Repositories/ITransactionRepository.cs` – transaction repository contract.
- `Repositories/TransactionRepository.cs` – Dapper implementation.
- `Data/ApplicationDbContext.cs` – EF Core context used by ASP.NET Core Identity.
- `Data/IdentitySeeder.cs` – optionally creates the configured administrator and assigns an admin claim.
- `Views/` – existing views plus the admin-only registered-users view.
- `Database/FinanceTables.sql` – creates the finance transaction table.
- `Program.cs` – configures Identity, Dapper repository DI, authorization and middleware.

## How the architecture works

### Dapper and Repository Pattern

`ITransactionRepository` defines the CRUD operations. `TransactionRepository` implements these operations using Dapper and parameterized SQL queries. Controllers depend on the interface rather than creating repository objects themselves.

### Dependency Injection

`Program.cs` registers the repository:

```csharp
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
```

ASP.NET Core injects `ITransactionRepository` into `HomeController` and `TransactionsController`.

### Identity and user-specific data

ASP.NET Core Identity manages each user's account and authentication. EF Core is used only for Identity's built-in tables. Dapper handles the finance transaction table.

Every transaction query includes the authenticated user's Identity ID (`UserId`). A user cannot read, edit, or delete another user's transactions through the application.

### Registered users

The `Users` page is restricted by the `AdminOnly` authorization policy. An administrator account is created only when `AdminUser:Email` and `AdminUser:Password` are supplied through environment variables. The admin password is not stored in the source code or `appsettings.json`.

### Layout sections

The original `_Layout.cshtml` and Bootstrap styling are retained. The layout supports optional `Styles` and `Scripts` sections. The Users navigation link appears only for the configured administrator.

## Local deployment (Windows)

### 1. Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (or a SQL Server instance)
- Visual Studio 2026 or VS Code
- SQL Server Management Studio (SSMS) is recommended for running the SQL script

### 2. Open the project

Extract the ZIP, open the `FinTracker` folder, and open `FinTracker.csproj` or the solution file.

Open a terminal in the folder containing `FinTracker.csproj`.

### 3. Restore packages

```powershell
dotnet restore
```

### 4. Check the database connection

The default connection string in `appsettings.json` is:

```text
Server=(localdb)\MSSQLLocalDB;Database=FinTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

If your SQL Server instance is different, update `ConnectionStrings:DefaultConnection` in `appsettings.json`.

### 5. Create the Identity database tables

The project already contains the Identity migration. Run:

```powershell
dotnet ef database update
```

If the `dotnet-ef` command is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

Close and reopen the terminal if the command is not recognized, then run `dotnet ef database update`.

### 6. Create the finance table

In SSMS:

1. Connect to `(localdb)\MSSQLLocalDB`.
2. Select the `FinTrackerDb` database.
3. Open `Database/FinanceTables.sql`.
4. Execute the script once.

This creates `FinanceTransactions`, which Dapper uses for income and expense records.

### 7. Configure the local administrator (optional)

Open PowerShell in the project folder and set credentials for the local administrator. Use your own strong password:

```powershell
$env:AdminUser__Email = "admin@fintracker.local"
$env:AdminUser__Password = "ChooseYourOwnStrongPassword123"
```

These environment variables are read by ASP.NET Core configuration. On application startup, FinTracker creates the admin account if it does not exist and assigns the admin claim. The Users page is visible only to this account.

If these variables are not supplied, normal registration and login still work, but no admin account is seeded and the Users page is unavailable.

### 8. Run locally

```powershell
dotnet run
```

Open the local URL printed in the terminal, usually one of the URLs configured in `Properties/launchSettings.json`.

### 9. Test the application

1. Register a normal user.
2. Log in and add an income or expense.
3. Open the dashboard and transactions page.
4. Edit and delete a transaction.
5. Sign out and register a second user.
6. Confirm the second user cannot see the first user's transactions.
7. If you configured the admin environment variables, sign in as the admin and open the Users page.

## Important notes

- Keep the existing `FinanceTables.sql` schema and Identity migration.
- Do not run `FinanceTables.sql` more than once unless the existing table has been removed.
- The admin password should be supplied through environment variables or a secure secret store, never committed to source control.
- Bootstrap and validation libraries are still loaded using the same CDN references as Deliverable 2.
- This is local deployment guidance; production hosting requires a production database, HTTPS configuration, and secure secret management.
