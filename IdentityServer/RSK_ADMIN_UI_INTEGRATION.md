# RSK Admin UI Integration Guide

## Overview

Rock Solid Knowledge (RSK) Admin UI is a commercial product that provides a web-based management interface for Duende IdentityServer. It allows you to manage clients, resources, scopes, and users without writing code or directly editing the database.

## Why Use RSK Admin UI?

- **User-Friendly Interface**: Manage IdentityServer configuration through a web UI
- **No Code Changes**: Add/modify clients without redeploying
- **Role-Based Access**: Control who can manage IdentityServer
- **Audit Trail**: Track configuration changes
- **Multi-Tenant Support**: Manage multiple IdentityServer instances
- **API Support**: REST API for automation

## Licensing Options

### 1. Cloud-Hosted Solution

**Best for:**
- Quick setup
- Small to medium deployments
- Teams without infrastructure management resources

**Pricing**: Contact RSK Sales
**Website**: https://www.identityserver.com/products/admin-ui

### 2. Self-Hosted Solution

**Best for:**
- Enterprise deployments
- On-premise requirements
- Custom integration needs
- High-security environments

**Pricing**: Annual subscription
**Requirements**:
- Valid Duende IdentityServer license
- .NET hosting infrastructure

## Installation Steps (Self-Hosted)

### Step 1: Purchase License

1. Visit: https://www.identityserver.com/products/admin-ui
2. Contact sales team
3. Choose deployment model (cloud vs self-hosted)
4. Receive license key and installation package

### Step 2: Add NuGet Package

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet add package Rsk.IdentityServer.AdminUI --version 3.0.0
```

Verify in `IdentityServer.Host.csproj`:
```xml
<PackageReference Include="Rsk.IdentityServer.AdminUI" Version="3.0.0" />
```

### Step 3: Configure Services

Edit `Program.cs` and add Admin UI services:

```csharp
// After AddIdentityServer() configuration
var identityServerBuilder = builder.Services.AddIdentityServer(/* ... */)
    // ... existing configuration

// Add Admin UI
builder.Services.AddIdentityServerAdmin(options =>
{
    // License key from RSK
    options.LicenseKey = builder.Configuration["AdminUI:LicenseKey"];

    // Base path for Admin UI
    options.BasePath = "/admin";

    // Require HTTPS
    options.RequireHttps = !builder.Environment.IsDevelopment();

    // Database connection (uses same as IdentityServer)
    options.ConfigureDbContext = b => b.UseSqlServer(connectionString);

    // Optional: Custom branding
    options.ApplicationName = "My IdentityServer Admin";
});

// Add authorization policy for Admin UI
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminUIAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin"); // Or custom claim
    });
});
```

### Step 4: Configure Application

Add middleware in `Program.cs` (after `UseIdentityServer()`):

```csharp
app.UseIdentityServer();

// Add Admin UI middleware
app.UseIdentityServerAdmin();

app.UseAuthorization();
```

### Step 5: Update Configuration

Edit `appsettings.json`:

```json
{
  "AdminUI": {
    "LicenseKey": "YOUR-LICENSE-KEY-FROM-RSK",
    "BasePath": "/admin",
    "AllowedOrigins": [
      "https://localhost:5001"
    ],
    "AdminUsers": [
      "admin@yourdomain.com"
    ],
    "AdminRole": "Admin",
    "EnableAuditLog": true,
    "SessionTimeout": 30
  }
}
```

### Step 6: Create Admin Users

Ensure admin users have appropriate roles:

```csharp
// In DatabaseInitializer.cs
var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

// Create Admin role
if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}

// Create admin user
var adminUser = new ApplicationUser
{
    UserName = "admin",
    Email = "admin@yourdomain.com",
    EmailConfirmed = true
};

await userManager.CreateAsync(adminUser, "Admin@123!");
await userManager.AddToRoleAsync(adminUser, "Admin");
```

## Accessing Admin UI

### Step 1: Run IdentityServer

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet run
```

### Step 2: Navigate to Admin UI

Open browser: `https://localhost:5001/admin`

### Step 3: Login

Use admin credentials:
- Username: `admin`
- Password: `Admin@123!`

## Managing Clients via Admin UI

### Creating a New Client

1. Navigate to **Clients** → **Add Client**

2. **Basic Information:**
   - **Client ID**: `my-web-app` (unique identifier)
   - **Client Name**: `My Web Application`
   - **Description**: Optional description
   - **Enabled**: ✓

3. **Client Type**: Select from templates:
   - Web Application (MVC/Razor Pages)
   - Single Page Application (SPA)
   - Native Application (Mobile/Desktop)
   - Machine-to-Machine (Service)
   - Empty (Custom)

4. **Authentication Settings:**
   - **Grant Types**:
     - ☑ Authorization Code
     - ☐ Client Credentials
     - ☐ Resource Owner Password
     - ☐ Implicit (deprecated)
     - ☐ Hybrid (deprecated)

   - **Require PKCE**: ✓ (Recommended)
   - **Allow Offline Access**: ✓ (for refresh tokens)

5. **Client Secrets:**
   - Click **Add Secret**
   - Enter secret value or generate
   - Set expiration (optional)
   - Hash type: SHA256

6. **Redirect URIs:**
   - Add: `https://myapp.com/signin-oidc`
   - Add: `https://myapp.com/silent-renew.html` (for SPAs)

7. **Post Logout Redirect URIs:**
   - Add: `https://myapp.com/signout-callback-oidc`

8. **Allowed Scopes:**
   - ☑ openid
   - ☑ profile
   - ☑ email
   - ☑ api1 (your custom API scope)

9. **Token Settings:**
   - **Identity Token Lifetime**: 300 seconds (5 min)
   - **Access Token Lifetime**: 3600 seconds (1 hour)
   - **Refresh Token Lifetime**: 2592000 seconds (30 days)
   - **Sliding Refresh Token**: Optional

10. **Advanced Settings:**
    - **Require Consent**: ☐ (for trusted apps)
    - **Allow Remember Consent**: ✓
    - **CORS Origins**: `https://myapp.com`
    - **Front Channel Logout URI**: Optional
    - **Back Channel Logout URI**: Optional

11. Click **Save**

### Editing Existing Clients

1. Navigate to **Clients**
2. Search or filter clients
3. Click client name
4. Modify settings
5. Click **Save**

### Cloning a Client

1. Navigate to **Clients**
2. Select client to clone
3. Click **Clone** button
4. Modify Client ID and other settings
5. Click **Save**

### Deleting a Client

1. Navigate to **Clients**
2. Select client
3. Click **Delete**
4. Confirm deletion

## Managing API Resources

### Create API Resource

1. Navigate to **API Resources** → **Add API Resource**

2. **Basic Information:**
   - **Name**: `my-api`
   - **Display Name**: `My API`
   - **Description**: Optional

3. **Scopes:**
   - Add existing scopes or create new ones
   - Example: `api.read`, `api.write`, `api.admin`

4. **User Claims:**
   - Select claims to include in access tokens
   - Common: `email`, `name`, `role`

5. Click **Save**

## Managing Identity Resources

### Standard Identity Resources

Pre-configured:
- `openid` - Required for OpenID Connect
- `profile` - User profile information
- `email` - Email address

### Custom Identity Resource

1. Navigate to **Identity Resources** → **Add Identity Resource**

2. **Basic Information:**
   - **Name**: `custom_claims`
   - **Display Name**: `Custom Claims`

3. **User Claims:**
   - Add custom claims
   - Example: `department`, `employee_id`, `manager`

4. Click **Save**

## Managing API Scopes

### Create API Scope

1. Navigate to **API Scopes** → **Add API Scope**

2. **Basic Information:**
   - **Name**: `api.read`
   - **Display Name**: `Read Access to API`
   - **Description**: Allows reading data

3. **User Claims** (optional):
   - Add claims to include when this scope is granted

4. Click **Save**

## User Management (if enabled)

### View Users

1. Navigate to **Users**
2. Search/filter users
3. View user details

### Edit User

1. Select user
2. Modify:
   - Email
   - Phone
   - Roles
   - Claims
   - Enable/Disable account
3. Click **Save**

### Reset Password

1. Select user
2. Click **Reset Password**
3. Enter new password or generate
4. Send to user

## Audit Log

### View Configuration Changes

1. Navigate to **Audit Log**
2. Filter by:
   - Date range
   - Entity type (Client, Resource, etc.)
   - Action (Create, Update, Delete)
   - User

3. Export audit log (CSV/Excel)

## API Access

### Using Admin UI API

Admin UI provides a REST API for automation:

```csharp
// Example: Create client via API
var httpClient = new HttpClient();
httpClient.SetBearerToken(accessToken);

var newClient = new
{
    ClientId = "automated-client",
    ClientName = "Automated Client",
    AllowedGrantTypes = new[] { "client_credentials" },
    ClientSecrets = new[] { new { Value = "secret" } },
    AllowedScopes = new[] { "api1" }
};

var response = await httpClient.PostAsJsonAsync(
    "https://localhost:5001/admin/api/clients",
    newClient
);
```

### Authentication for API

API uses the IdentityServer itself:

```csharp
// Request token
var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
    new ClientCredentialsTokenRequest
    {
        Address = "https://localhost:5001/connect/token",
        ClientId = "admin-api-client",
        ClientSecret = "admin-api-secret",
        Scope = "identityserver-admin-api"
    }
);
```

## Integration with Microsoft Entra

### Configure Azure AD Users for Admin Access

1. **In Admin UI**, navigate to **Settings** → **External Providers**

2. **Azure AD Configuration:**
   - Provider: Microsoft Entra
   - Automatic user provisioning: ✓
   - Role mapping:
     - Azure AD Group: `IdentityServer-Admins`
     - Local Role: `Admin`

3. **Save**

Now Azure AD users can login to Admin UI using Microsoft Entra.

## Custom Branding (Optional)

### Customize UI

1. Navigate to **Settings** → **Branding**

2. **Configure:**
   - Logo URL
   - Primary color
   - Secondary color
   - Application name
   - Footer text

3. Upload custom CSS (advanced)

4. **Save**

## Backup and Restore

### Export Configuration

1. Navigate to **Maintenance** → **Export**
2. Select entities:
   - ☑ Clients
   - ☑ API Resources
   - ☑ Identity Resources
   - ☑ API Scopes
3. Click **Export**
4. Download JSON file

### Import Configuration

1. Navigate to **Maintenance** → **Import**
2. Upload JSON file
3. Select import options:
   - Skip existing
   - Update existing
   - Replace all
4. Click **Import**

## Monitoring and Diagnostics

### View Server Status

1. Navigate to **Dashboard**
2. View metrics:
   - Active users
   - Token requests (last 24h)
   - Failed logins
   - Active clients

### Health Checks

1. Navigate to **Diagnostics** → **Health**
2. View:
   - Database connectivity
   - Cache status
   - Signing key status

## Security Best Practices

### 1. Role-Based Access

Only grant Admin UI access to trusted users:

```csharp
options.AddPolicy("AdminUIAccess", policy =>
{
    policy.RequireRole("Admin", "IdentityServerManager");
});
```

### 2. IP Restrictions (Optional)

Restrict Admin UI to specific IPs:

```csharp
builder.Services.AddIdentityServerAdmin(options =>
{
    options.AllowedIPAddresses = new[]
    {
        "192.168.1.0/24",
        "10.0.0.0/8"
    };
});
```

### 3. Audit All Changes

Enable comprehensive audit logging:

```json
"AdminUI": {
  "EnableAuditLog": true,
  "AuditLogRetentionDays": 90
}
```

### 4. Regular Reviews

- Review clients quarterly
- Remove unused clients
- Rotate client secrets annually
- Review audit logs monthly

## Troubleshooting

### Cannot Access Admin UI

**Check:**
1. Admin UI middleware is added: `app.UseIdentityServerAdmin()`
2. User has "Admin" role
3. License key is valid
4. HTTPS is enabled (or development environment)

### License Error

**Solutions:**
1. Verify license key in appsettings.json
2. Check license expiration
3. Contact RSK support

### Configuration Not Saving

**Check:**
1. Database connection is valid
2. User has appropriate permissions
3. Check application logs
4. Verify database schema is up to date

## Alternative: Managing Without Admin UI

If you don't have Admin UI, you can manage configuration via:

### 1. Code (Config.cs)

Update `Data/Config.cs` and restart application

### 2. Database Direct

Not recommended for production, but possible:

```sql
-- Query clients
SELECT * FROM Clients;

-- Update client
UPDATE Clients SET Enabled = 1 WHERE ClientId = 'my-client';
```

### 3. Custom Admin Tool

Build your own:
```csharp
[Authorize(Roles = "Admin")]
public class ClientManagementController : Controller
{
    private readonly ConfigurationDbContext _context;

    public ClientManagementController(ConfigurationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _context.Clients.ToListAsync();
        return View(clients);
    }

    // CRUD operations...
}
```

## Support and Resources

- **RSK Documentation**: https://www.identityserver.com/documentation/admin-ui
- **Support Portal**: https://www.identityserver.com/support
- **Community Forum**: https://github.com/DuendeSoftware/IdentityServer/discussions
- **Email Support**: support@identityserver.com

## Conclusion

RSK Admin UI simplifies IdentityServer management significantly. While it requires a license, the productivity gains and reduced maintenance overhead make it worthwhile for most production deployments.

For teams managing multiple clients and resources, the Admin UI is highly recommended.
