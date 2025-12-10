# Entity Framework Core Migration Commands

This document contains all the commands needed to create and apply database migrations for the Duende IdentityServer project.

## Prerequisites

Ensure you have the Entity Framework Core tools installed:

```bash
dotnet tool install --global dotnet-ef
```

Verify installation:
```bash
dotnet ef
```

## Navigate to Project Directory

```bash
cd IdentityServer/src/IdentityServer.Host
```

## Create Initial Migrations

### 1. Application Identity Database (Users, Roles)

```bash
dotnet ef migrations add InitialIdentityServerMigration \
  -c ApplicationDbContext \
  -o Data/Migrations/ApplicationDb
```

### 2. IdentityServer Configuration Database (Clients, Resources, Scopes)

```bash
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration \
  -c ConfigurationDbContext \
  -o Data/Migrations/ConfigurationDb
```

### 3. IdentityServer Operational Database (Tokens, Grants, Consents)

```bash
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration \
  -c PersistedGrantDbContext \
  -o Data/Migrations/PersistedGrantDb
```

## Apply Migrations to Database

### Option 1: Apply All Migrations at Once

```bash
# Application Database
dotnet ef database update -c ApplicationDbContext

# Configuration Database
dotnet ef database update -c ConfigurationDbContext

# Operational Database
dotnet ef database update -c PersistedGrantDbContext
```

### Option 2: Let Application Auto-Migrate (Recommended for Development)

The application is configured to automatically apply migrations on startup in `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    await DatabaseInitializer.InitializeAsync(scope.ServiceProvider);
}
```

Just run the application:
```bash
dotnet run
```

## Additional Migration Commands

### List All Migrations

```bash
# ApplicationDbContext
dotnet ef migrations list -c ApplicationDbContext

# ConfigurationDbContext
dotnet ef migrations list -c ConfigurationDbContext

# PersistedGrantDbContext
dotnet ef migrations list -c PersistedGrantDbContext
```

### Remove Last Migration (if not applied to database)

```bash
# Remove last migration for ApplicationDbContext
dotnet ef migrations remove -c ApplicationDbContext

# Remove last migration for ConfigurationDbContext
dotnet ef migrations remove -c ConfigurationDbContext

# Remove last migration for PersistedGrantDbContext
dotnet ef migrations remove -c PersistedGrantDbContext
```

### Rollback to Specific Migration

```bash
# Rollback ApplicationDbContext to specific migration
dotnet ef database update MigrationName -c ApplicationDbContext

# Rollback to initial state (remove all)
dotnet ef database update 0 -c ApplicationDbContext
```

### Generate SQL Script (for production deployment)

```bash
# Generate SQL for ApplicationDbContext
dotnet ef migrations script -c ApplicationDbContext -o ApplicationDb.sql

# Generate SQL for ConfigurationDbContext
dotnet ef migrations script -c ConfigurationDbContext -o ConfigurationDb.sql

# Generate SQL for PersistedGrantDbContext
dotnet ef migrations script -c PersistedGrantDbContext -o PersistedGrantDb.sql
```

### Generate Idempotent SQL Script (safe to run multiple times)

```bash
dotnet ef migrations script -c ApplicationDbContext -o ApplicationDb.sql --idempotent
dotnet ef migrations script -c ConfigurationDbContext -o ConfigurationDb.sql --idempotent
dotnet ef migrations script -c PersistedGrantDbContext -o PersistedGrantDb.sql --idempotent
```

## Drop and Recreate Database (Development Only)

**⚠️ WARNING: This will delete all data!**

```bash
# Drop ApplicationDbContext database
dotnet ef database drop -c ApplicationDbContext --force

# Drop ConfigurationDbContext database (usually same as ApplicationDbContext)
dotnet ef database drop -c ConfigurationDbContext --force

# Drop PersistedGrantDbContext database (usually same as ApplicationDbContext)
dotnet ef database drop -c PersistedGrantDbContext --force

# Recreate and apply migrations
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

## Adding New Migrations (After Model Changes)

### 1. Modify Your Model

Example: Add a new property to `ApplicationUser`:

```csharp
public class ApplicationUser : IdentityUser
{
    // New property
    public string Department { get; set; }
}
```

### 2. Create Migration

```bash
dotnet ef migrations add AddDepartmentToUser -c ApplicationDbContext -o Data/Migrations/ApplicationDb
```

### 3. Apply Migration

```bash
dotnet ef database update -c ApplicationDbContext
```

## Connection String Configuration

Ensure your `appsettings.json` has the correct connection string:

### Development (LocalDB)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DuendeIdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Docker SQL Server

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DuendeIdentityServer;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
}
```

### Azure SQL Database

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=DuendeIdentityServer;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

### Production SQL Server

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your-production-server;Database=DuendeIdentityServer;User Id=identityserver_user;Password=SecurePassword;Encrypt=true;TrustServerCertificate=false"
}
```

## Troubleshooting

### Error: "No DbContext was found"

**Solution**: Ensure you're in the correct directory and the project file exists:
```bash
cd IdentityServer/src/IdentityServer.Host
ls *.csproj
```

### Error: "Build failed"

**Solution**: Build the project first:
```bash
dotnet build
dotnet ef migrations add MigrationName -c ApplicationDbContext
```

### Error: "A network-related or instance-specific error occurred"

**Solution**:
1. Verify SQL Server is running
2. Check connection string
3. Test connection with SQL Server Management Studio or Azure Data Studio

### Error: "Login failed for user"

**Solution**: Check SQL Server authentication:
```bash
# For Windows Authentication, ensure current user has access
# For SQL Authentication, verify username/password in connection string
```

### Error: "The specified table does not exist"

**Solution**: Apply migrations:
```bash
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

## Production Deployment

For production, it's recommended to use SQL scripts rather than auto-migration:

### 1. Generate Scripts

```bash
dotnet ef migrations script -c ApplicationDbContext -o ApplicationDb.sql --idempotent
dotnet ef migrations script -c ConfigurationDbContext -o ConfigurationDb.sql --idempotent
dotnet ef migrations script -c PersistedGrantDbContext -o PersistedGrantDb.sql --idempotent
```

### 2. Review Scripts

Review the generated SQL scripts for security and correctness.

### 3. Apply to Production Database

Use SQL Server Management Studio, Azure Portal, or command-line tools:

```bash
sqlcmd -S yourserver -d DuendeIdentityServer -U username -P password -i ApplicationDb.sql
sqlcmd -S yourserver -d DuendeIdentityServer -U username -P password -i ConfigurationDb.sql
sqlcmd -S yourserver -d DuendeIdentityServer -U username -P password -i PersistedGrantDb.sql
```

### 4. Disable Auto-Migration in Production

In `Program.cs`, wrap auto-migration in development check:

```csharp
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        await DatabaseInitializer.InitializeAsync(scope.ServiceProvider);
    }
}
```

## Backup Before Migration (Production)

Always backup before running migrations in production:

```sql
-- SQL Server backup
BACKUP DATABASE DuendeIdentityServer
TO DISK = 'C:\Backups\DuendeIdentityServer_PreMigration.bak'
WITH FORMAT, COMPRESSION;
```

## Best Practices

1. ✅ Always backup production databases before migration
2. ✅ Test migrations in staging environment first
3. ✅ Use idempotent SQL scripts for production
4. ✅ Review generated SQL before applying to production
5. ✅ Keep migration names descriptive
6. ✅ Commit migrations to source control
7. ✅ Document breaking changes
8. ✅ Use separate databases for dev, staging, and production
9. ✅ Never use `database drop` in production
10. ✅ Monitor database performance after migrations

## Quick Reference

```bash
# Most common commands

# Create migration
dotnet ef migrations add MigrationName -c ApplicationDbContext

# Apply migrations
dotnet ef database update -c ApplicationDbContext

# Remove last migration
dotnet ef migrations remove -c ApplicationDbContext

# Generate SQL script
dotnet ef migrations script -c ApplicationDbContext -o script.sql

# List migrations
dotnet ef migrations list -c ApplicationDbContext

# Drop database (DEV ONLY!)
dotnet ef database drop -c ApplicationDbContext --force
```

## Additional Resources

- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core CLI Reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Duende IdentityServer EF Integration](https://docs.duendesoftware.com/identityserver/v7/data/ef/)
