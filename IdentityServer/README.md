# Duende IdentityServer v7 Setup

This is a complete setup for Duende IdentityServer v7 with .NET 8, configured for RSK Admin UI integration and Microsoft Entra (Azure AD) external authentication.

## Features

- ✅ **Duende IdentityServer v7** - Latest version of the leading OpenID Connect and OAuth 2.0 framework
- ✅ **.NET 8** - Latest LTS version of .NET
- ✅ **Entity Framework Core** - Database persistence for configuration and operational data
- ✅ **ASP.NET Core Identity** - User management and authentication
- ✅ **Microsoft Entra (Azure AD)** - External authentication provider with callback support
- ✅ **RSK Admin UI Ready** - Pre-configured for Rock Solid Knowledge Admin UI integration
- ✅ **CORS Support** - Configured for SPA and Admin UI clients
- ✅ **Serilog Logging** - Structured logging to console and file

## Prerequisites

- .NET 8 SDK - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 / VS Code / JetBrains Rider
- (Optional) RSK Admin UI License - [Rock Solid Knowledge](https://www.identityserver.com/products/admin-ui)

## Quick Start

### 1. Update Configuration

Edit `appsettings.json` and configure the following:

#### Database Connection String
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DuendeIdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

For production, use a proper SQL Server connection string.

#### Microsoft Entra (Azure AD) Configuration

1. Register an application in Azure Portal:
   - Go to Azure Active Directory → App registrations → New registration
   - Name: "IdentityServer Authentication"
   - Redirect URI: `https://localhost:5001/signin-azuread`
   - Create a client secret

2. Update `appsettings.json`:
```json
"AzureAd": {
  "Authority": "https://login.microsoftonline.com/{YOUR_TENANT_ID}/v2.0",
  "ClientId": "{YOUR_CLIENT_ID}",
  "ClientSecret": "{YOUR_CLIENT_SECRET}",
  "TenantId": "{YOUR_TENANT_ID}",
  "CallbackPath": "/signin-azuread"
}
```

3. In Azure Portal, add these redirect URIs:
   - `https://localhost:5001/signin-azuread` (sign-in)
   - `https://localhost:5001/signout-callback-azuread` (sign-out)

### 2. Initialize Database

```bash
cd IdentityServer/src/IdentityServer.Host

# Add initial migration
dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext

# Apply migrations
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext
```

The application will also auto-migrate and seed data on first run.

### 3. Run the Application

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet run
```

Navigate to: `https://localhost:5001`

### 4. Default Admin User

- **Username**: `admin`
- **Password**: `Admin@123!`

**⚠️ IMPORTANT**: Change this password immediately in production!

## RSK Admin UI Integration

### Option 1: Cloud-Hosted Admin UI

Rock Solid Knowledge offers a cloud-hosted Admin UI solution. Contact them for licensing.

### Option 2: Self-Hosted Admin UI

1. Purchase a license from [Rock Solid Knowledge](https://www.identityserver.com/products/admin-ui)

2. Add the NuGet package to your project:
```bash
dotnet add package Rsk.IdentityServer.AdminUI
```

3. Uncomment the Admin UI package reference in `IdentityServer.Host.csproj`:
```xml
<PackageReference Include="Rsk.IdentityServer.AdminUI" Version="3.0.0" />
```

4. Configure in `Program.cs`:
```csharp
builder.Services.AddIdentityServerAdmin(options =>
{
    options.LicenseKey = builder.Configuration["AdminUI:LicenseKey"];
    options.RequireHttps = true;
});
```

5. Update `appsettings.json`:
```json
"AdminUI": {
  "LicenseKey": "{YOUR_LICENSE_KEY}",
  "AllowedOrigins": [
    "https://localhost:5001"
  ],
  "AdminUsers": [
    "admin@yourdomain.com"
  ]
}
```

### Managing Clients via RSK Admin UI

Once RSK Admin UI is configured, you can:

1. Navigate to `https://localhost:5001/admin`
2. Login with admin credentials
3. Manage:
   - **Clients** - OAuth/OIDC clients
   - **API Resources** - Protected APIs
   - **Identity Resources** - User claims
   - **API Scopes** - Permission scopes
   - **Users** - User accounts

## Client Configuration Examples

### Interactive Web Application (MVC/Razor Pages)

```csharp
new Client
{
    ClientId = "mvc-client",
    ClientSecrets = { new Secret("secret".Sha256()) },
    AllowedGrantTypes = GrantTypes.Code,
    RedirectUris = { "https://localhost:5002/signin-oidc" },
    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
    AllowedScopes = { "openid", "profile", "email", "api1" },
    RequirePkce = true
}
```

### Single Page Application (React/Angular/Vue)

```csharp
new Client
{
    ClientId = "spa-client",
    AllowedGrantTypes = GrantTypes.Code,
    RequirePkce = true,
    RequireClientSecret = false,
    RedirectUris = { "https://localhost:3000/callback" },
    PostLogoutRedirectUris = { "https://localhost:3000/" },
    AllowedCorsOrigins = { "https://localhost:3000" },
    AllowedScopes = { "openid", "profile", "email", "api1" },
    AllowAccessTokensViaBrowser = true
}
```

### Machine-to-Machine (Service)

```csharp
new Client
{
    ClientId = "service-client",
    ClientSecrets = { new Secret("secret".Sha256()) },
    AllowedGrantTypes = GrantTypes.ClientCredentials,
    AllowedScopes = { "api1" }
}
```

## Microsoft Entra Integration Details

### How It Works

1. Users can choose to login with Microsoft Entra on the login page
2. IdentityServer redirects to Microsoft's login page
3. After successful authentication, Microsoft redirects back to IdentityServer
4. IdentityServer creates a local session and issues tokens
5. Users can be automatically provisioned in the local database

### Callback Configuration

The callback URL in Azure AD must match:
- **Redirect URI**: `https://localhost:5001/signin-azuread`
- **Logout URI**: `https://localhost:5001/signout-callback-azuread`

### Customizing Claims

Edit `Program.cs` to map additional claims from Azure AD:

```csharp
options.ClaimActions.MapJsonKey("department", "department");
options.ClaimActions.MapJsonKey("employee_id", "employee_id");
```

## Database Schema

The solution uses three DbContexts:

1. **ApplicationDbContext** - ASP.NET Core Identity tables (Users, Roles, etc.)
2. **ConfigurationDbContext** - IdentityServer configuration (Clients, Resources, Scopes)
3. **PersistedGrantDbContext** - Operational data (Tokens, Grants, Consents)

## Security Considerations

### Production Checklist

- [ ] Change default admin password
- [ ] Use strong client secrets (use cryptographically random values)
- [ ] Enable HTTPS (obtain proper SSL certificate)
- [ ] Configure proper CORS origins
- [ ] Use production database with proper connection string security
- [ ] Enable rate limiting
- [ ] Configure proper logging and monitoring
- [ ] Review and adjust token lifetimes
- [ ] Enable consent screen where appropriate
- [ ] Configure proper password policies
- [ ] Enable two-factor authentication
- [ ] Regular security updates for NuGet packages

### Recommended Settings

```csharp
// In Program.cs - Identity options
options.Password.RequiredLength = 12;
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.SignIn.RequireConfirmedEmail = true;
```

## Troubleshooting

### Database Migration Issues

```bash
# Drop and recreate database (DEV ONLY!)
dotnet ef database drop -c ApplicationDbContext
dotnet ef database update -c ApplicationDbContext
```

### CORS Errors

Ensure your client origin is in the `AllowedCorsOrigins` in the client configuration.

### Azure AD Login Issues

1. Verify Tenant ID, Client ID, and Client Secret
2. Check redirect URIs in Azure Portal
3. Ensure proper API permissions are granted
4. Review Serilog logs in `logs/` folder

### Discovery Document

View the OpenID Connect discovery document:
```
https://localhost:5001/.well-known/openid-configuration
```

## Project Structure

```
IdentityServer/
├── src/
│   └── IdentityServer.Host/
│       ├── Controllers/          # MVC Controllers
│       ├── Data/                 # DbContexts and configuration
│       │   ├── ApplicationDbContext.cs
│       │   ├── Config.cs         # Client/Resource definitions
│       │   └── DatabaseInitializer.cs
│       ├── Models/               # Domain models
│       │   └── ApplicationUser.cs
│       ├── Properties/           # Launch settings
│       ├── Views/                # Razor views
│       ├── Program.cs            # Application entry point
│       ├── appsettings.json      # Configuration
│       └── IdentityServer.Host.csproj
└── README.md
```

## Useful Commands

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Watch mode (auto-reload)
dotnet watch run

# Create migration
dotnet ef migrations add MigrationName -c ApplicationDbContext

# Update database
dotnet ef database update -c ApplicationDbContext

# List migrations
dotnet ef migrations list -c ApplicationDbContext
```

## Resources

- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7)
- [RSK Admin UI Documentation](https://www.identityserver.com/documentation/admin-ui)
- [Microsoft Entra Documentation](https://learn.microsoft.com/en-us/entra/identity/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)

## License

This setup uses:
- **Duende IdentityServer** - Requires a license for production use. Free for development and testing.
- **RSK Admin UI** - Requires a separate license from Rock Solid Knowledge.

See [Duende Software Licensing](https://duendesoftware.com/products/identityserver) for details.

## Support

For issues and questions:
- Duende IdentityServer: https://github.com/DuendeSoftware/IdentityServer
- RSK Admin UI: https://www.identityserver.com/support

## Next Steps

1. ✅ Configure Azure AD application
2. ✅ Run database migrations
3. ✅ Start the application
4. ✅ Test login with local account (admin/Admin@123!)
5. ✅ Test login with Microsoft Entra
6. ✅ Purchase and configure RSK Admin UI license
7. ✅ Create your first client application
8. ✅ Configure production settings
9. ✅ Deploy to production environment
