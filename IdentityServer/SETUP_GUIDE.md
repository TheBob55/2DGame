# Step-by-Step Setup Guide

## Prerequisites Installation

### 1. Install .NET 8 SDK

**Windows:**
```powershell
# Download and install from
https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --version
```

**macOS:**
```bash
brew install dotnet@8
dotnet --version
```

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
dotnet --version
```

### 2. Install SQL Server

**Windows - SQL Server LocalDB (Recommended for Development):**
- Included with Visual Studio 2022
- Or download: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

**Windows - SQL Server Express:**
```
https://www.microsoft.com/en-us/sql-server/sql-server-downloads
```

**macOS/Linux - Use Docker:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string for Docker SQL Server:
# Server=localhost,1433;Database=DuendeIdentityServer;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
```

## Azure AD (Microsoft Entra) Configuration

### 1. Register Application in Azure Portal

1. Navigate to [Azure Portal](https://portal.azure.com)
2. Go to **Azure Active Directory** → **App registrations** → **New registration**

**Registration Details:**
- **Name**: `IdentityServer Authentication`
- **Supported account types**: Choose based on your needs:
  - Single tenant (your organization only)
  - Multi-tenant (any Azure AD directory)
  - Multi-tenant + personal Microsoft accounts
- **Redirect URI**:
  - Platform: `Web`
  - URI: `https://localhost:5001/signin-azuread`

3. Click **Register**

### 2. Create Client Secret

1. In your app registration, go to **Certificates & secrets**
2. Click **New client secret**
3. Description: `IdentityServer Secret`
4. Expires: Choose appropriate duration
5. Click **Add**
6. **⚠️ IMPORTANT**: Copy the secret value immediately (you won't see it again!)

### 3. Configure Redirect URIs

1. Go to **Authentication** in your app registration
2. Add these redirect URIs:
   - `https://localhost:5001/signin-azuread`
   - `https://localhost:5001/signout-callback-azuread`
3. Under **Front-channel logout URL**: `https://localhost:5001/signout-azuread`
4. Under **Implicit grant and hybrid flows**: (Leave unchecked - we use Authorization Code flow)
5. Click **Save**

### 4. Configure API Permissions (Optional)

For reading user profile information:
1. Go to **API permissions**
2. Click **Add a permission** → **Microsoft Graph** → **Delegated permissions**
3. Add:
   - `openid`
   - `profile`
   - `email`
   - `User.Read`
4. Click **Add permissions**
5. Click **Grant admin consent** (if you have admin rights)

### 5. Get Your Configuration Values

From your app registration **Overview** page, copy:
- **Application (client) ID** - This is your `ClientId`
- **Directory (tenant) ID** - This is your `TenantId`
- **Client secret** - You copied this earlier

## IdentityServer Configuration

### 1. Update appsettings.json

Navigate to `IdentityServer/src/IdentityServer.Host/appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DuendeIdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{PASTE_YOUR_TENANT_ID_HERE}/v2.0",
    "ClientId": "{PASTE_YOUR_CLIENT_ID_HERE}",
    "ClientSecret": "{PASTE_YOUR_CLIENT_SECRET_HERE}",
    "TenantId": "{PASTE_YOUR_TENANT_ID_HERE}",
    "CallbackPath": "/signin-azuread"
  }
}
```

**Example:**
```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/12345678-1234-1234-1234-123456789012/v2.0",
    "ClientId": "87654321-4321-4321-4321-210987654321",
    "ClientSecret": "abc123~DefGhI456JklMnoPqRsTuVwXyZ",
    "TenantId": "12345678-1234-1234-1234-123456789012",
    "CallbackPath": "/signin-azuread"
  }
}
```

### 2. Update Connection String (if needed)

**For Docker SQL Server:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DuendeIdentityServer;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
}
```

**For Remote SQL Server:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your-server.database.windows.net;Database=DuendeIdentityServer;User Id=yourusername;Password=yourpassword;Encrypt=true"
}
```

## Database Setup

### 1. Install Entity Framework Tools

```bash
dotnet tool install --global dotnet-ef
```

Verify:
```bash
dotnet ef
```

### 2. Navigate to Project Directory

```bash
cd IdentityServer/src/IdentityServer.Host
```

### 3. Create Migrations

```bash
# Application Identity Migration
dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext -o Data/Migrations/ApplicationDb

# IdentityServer Configuration Migration
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/ConfigurationDb

# IdentityServer Operational Migration
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDb
```

### 4. Apply Migrations

```bash
# Update Application Database
dotnet ef database update -c ApplicationDbContext

# Update Configuration Database
dotnet ef database update -c ConfigurationDbContext

# Update Operational Database
dotnet ef database update -c PersistedGrantDbContext
```

### 5. Verify Database Creation

**SQL Server Management Studio:**
- Connect to your server
- You should see `DuendeIdentityServer` database
- Tables should include:
  - `Users` (ApplicationDbContext)
  - `Clients`, `ApiScopes`, `IdentityResources` (ConfigurationDbContext)
  - `PersistedGrants`, `DeviceCodes` (PersistedGrantDbContext)

## Running the Application

### 1. Build the Project

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet build
```

### 2. Run the Application

```bash
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 3. Test the Application

Open browser and navigate to: `https://localhost:5001`

You should see the IdentityServer home page.

### 4. Test Discovery Endpoint

Navigate to: `https://localhost:5001/.well-known/openid-configuration`

You should see a JSON response with IdentityServer configuration.

## Testing Authentication

### 1. Local Account Login

Default admin account:
- **Username**: `admin`
- **Email**: `admin@yourdomain.com`
- **Password**: `Admin@123!`

Navigate to: `https://localhost:5001/Account/Login`

### 2. Microsoft Entra Login

1. Click **Microsoft Entra** button on login page
2. Redirected to Microsoft login
3. Sign in with your Microsoft account
4. Consent to permissions (if prompted)
5. Redirected back to IdentityServer
6. New user account created automatically

## RSK Admin UI Setup

### 1. Obtain License

Contact Rock Solid Knowledge:
- Website: https://www.identityserver.com/products/admin-ui
- Email: sales@identityserver.com

Choose between:
- **Cloud-hosted**: They host the Admin UI
- **Self-hosted**: You install on your infrastructure

### 2. Install Admin UI Package (Self-Hosted)

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet add package Rsk.IdentityServer.AdminUI --version 3.0.0
```

### 3. Update Project File

Uncomment in `IdentityServer.Host.csproj`:
```xml
<PackageReference Include="Rsk.IdentityServer.AdminUI" Version="3.0.0" />
```

### 4. Configure Admin UI

Update `Program.cs` - add before `var app = builder.Build();`:

```csharp
// Add Admin UI
builder.Services.AddIdentityServerAdmin(options =>
{
    options.LicenseKey = builder.Configuration["AdminUI:LicenseKey"];
    options.RequireHttps = true;
    options.BasePath = "/admin";
});
```

Update `appsettings.json`:
```json
"AdminUI": {
  "LicenseKey": "YOUR_LICENSE_KEY_HERE",
  "AllowedOrigins": [
    "https://localhost:5001"
  ],
  "AdminUsers": [
    "admin@yourdomain.com"
  ]
}
```

### 5. Access Admin UI

Navigate to: `https://localhost:5001/admin`

Login with admin credentials.

### 6. Managing Clients in Admin UI

1. Go to **Clients** section
2. Click **Add Client**
3. Fill in client details:
   - Client ID
   - Client Name
   - Grant Type (Code, ClientCredentials, etc.)
   - Redirect URIs
   - Allowed Scopes
4. Save

## Creating Your First Client Application

### Example: ASP.NET Core MVC Client

1. **In Admin UI or Config.cs**, add client:

```csharp
new Client
{
    ClientId = "mvc-client",
    ClientName = "MVC Application",
    ClientSecrets = { new Secret("mvc-secret-key".Sha256()) },

    AllowedGrantTypes = GrantTypes.Code,
    RequirePkce = true,

    RedirectUris = { "https://localhost:7001/signin-oidc" },
    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },

    AllowedScopes =
    {
        "openid",
        "profile",
        "email",
        "api1"
    },

    AllowOfflineAccess = true
}
```

2. **Create MVC Application:**

```bash
dotnet new mvc -n MvcClient
cd MvcClient
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
```

3. **Configure Authentication in Program.cs:**

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:5001";
    options.ClientId = "mvc-client";
    options.ClientSecret = "mvc-secret-key";
    options.ResponseType = "code";

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
});

app.UseAuthentication();
app.UseAuthorization();
```

4. **Protect Controller:**

```csharp
[Authorize]
public class HomeController : Controller
{
    // ...
}
```

5. **Run both applications:**

Terminal 1:
```bash
cd IdentityServer/src/IdentityServer.Host
dotnet run
```

Terminal 2:
```bash
cd MvcClient
dotnet run --urls="https://localhost:7001"
```

## Production Deployment

### 1. Security Hardening

- [ ] Change all default passwords
- [ ] Use strong, random client secrets
- [ ] Enable HTTPS with valid certificates
- [ ] Configure proper CORS
- [ ] Enable rate limiting
- [ ] Configure security headers
- [ ] Regular security updates

### 2. Environment Variables

Use environment variables or Azure Key Vault for secrets:

```bash
# Instead of appsettings.json
export AzureAd__ClientSecret="your-secret"
export ConnectionStrings__DefaultConnection="your-connection-string"
```

### 3. Database

- Use managed database (Azure SQL, AWS RDS)
- Enable encryption at rest
- Regular backups
- Connection pooling

### 4. Monitoring

- Enable Application Insights or similar
- Configure alerts
- Log aggregation (ELK, Seq, etc.)
- Performance monitoring

## Troubleshooting

### Error: "Login failed for user"

**Solution**: Check connection string, SQL Server is running, and user has permissions.

### Error: "Unable to resolve service for type 'ConfigurationDbContext'"

**Solution**: Ensure migrations are created and applied.

### Azure AD Login Redirects to Error Page

**Solution**:
1. Verify Tenant ID, Client ID, Client Secret in appsettings.json
2. Check redirect URIs in Azure Portal match exactly
3. Check logs in `logs/` folder

### CORS Error in Browser Console

**Solution**: Add client origin to `AllowedCorsOrigins` in client configuration.

### Database Already Exists Error

**Solution (DEV ONLY)**:
```bash
dotnet ef database drop -c ApplicationDbContext --force
dotnet ef database update -c ApplicationDbContext
```

## Next Steps

1. ✅ Complete this setup guide
2. ✅ Test local login
3. ✅ Test Azure AD login
4. ✅ Configure RSK Admin UI
5. ✅ Create first client application
6. ✅ Test end-to-end flow
7. ✅ Configure for production
8. ✅ Deploy

## Additional Resources

- [Complete Sample Applications](https://github.com/DuendeSoftware/Samples)
- [IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7)
- [Video Tutorials](https://www.youtube.com/c/DuendeSoftware)
- [Community Forum](https://github.com/DuendeSoftware/IdentityServer/discussions)
