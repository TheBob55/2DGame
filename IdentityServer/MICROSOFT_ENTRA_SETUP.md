# Microsoft Entra (Azure AD) Setup Guide

Complete guide for configuring Microsoft Entra ID (formerly Azure AD) as an external authentication provider for Duende IdentityServer.

## Overview

This integration allows users to:
- Sign in using their Microsoft work/school accounts
- Sign in using personal Microsoft accounts (optional)
- Automatically provision user accounts in IdentityServer
- Link existing accounts with Microsoft credentials

## Prerequisites

- Azure subscription (free tier works)
- Azure AD tenant (comes with subscription)
- Admin access to Azure Portal
- Duende IdentityServer instance running

## Part 1: Azure Portal Configuration

### Step 1: Access Azure Portal

1. Navigate to: https://portal.azure.com
2. Sign in with your Azure account
3. Search for "Azure Active Directory" or "Microsoft Entra ID"

### Step 2: Register Application

1. In Azure AD, select **App registrations**
2. Click **+ New registration**

3. Fill in registration form:

   **Name:**
   ```
   IdentityServer External Authentication
   ```

   **Supported account types:** Choose one:
   - ✓ **Accounts in this organizational directory only (Single tenant)**
     - For: Internal company users only
     - Use: Enterprise applications

   - ✓ **Accounts in any organizational directory (Multi-tenant)**
     - For: B2B scenarios, multiple organizations
     - Use: SaaS applications

   - ✓ **Accounts in any organizational directory + personal Microsoft accounts**
     - For: Consumer + enterprise
     - Use: Public applications with Microsoft login

   **Redirect URI:**
   - Platform: `Web`
   - URI: `https://localhost:5001/signin-azuread`

4. Click **Register**

5. **Save these values** from the **Overview** page:
   - Application (client) ID: `12345678-1234-1234-1234-123456789012`
   - Directory (tenant) ID: `87654321-4321-4321-4321-210987654321`

### Step 3: Create Client Secret

1. In your app registration, navigate to **Certificates & secrets**
2. Click **+ New client secret**

3. Configure secret:
   - **Description**: `IdentityServer Production Secret`
   - **Expires**: Choose based on your security policy:
     - 6 months (more secure, requires regular rotation)
     - 12 months
     - 24 months
     - Custom

4. Click **Add**

5. **⚠️ CRITICAL**: Copy the **Value** immediately (you won't see it again!)
   ```
   abc123~DefGhI456JklMnoPqRsTuVwXyZ
   ```

6. Save to password manager or secure location

### Step 4: Configure Authentication Settings

1. Navigate to **Authentication** in your app registration

2. **Platform configurations:**
   - Verify Web platform exists
   - Redirect URIs should include:
     ```
     https://localhost:5001/signin-azuread
     https://yourdomain.com/signin-azuread (for production)
     ```

3. **Front-channel logout URL:**
   ```
   https://localhost:5001/signout-azuread
   ```

4. **Logout URL (optional):**
   ```
   https://localhost:5001/signout-callback-azuread
   ```

5. **Implicit grant and hybrid flows:**
   - ☐ Access tokens (not needed)
   - ☐ ID tokens (not needed)

   We're using Authorization Code flow, so leave these unchecked.

6. **Allow public client flows:**
   - ☐ No (we're a confidential web application)

7. Click **Save**

### Step 5: Configure API Permissions

1. Navigate to **API permissions**

2. Default permissions (already present):
   - Microsoft Graph → User.Read (Delegated)

3. Add additional permissions (recommended):

   Click **+ Add a permission** → **Microsoft Graph** → **Delegated permissions**

   Select:
   - ☑ `openid` - Required for OpenID Connect
   - ☑ `profile` - User profile information
   - ☑ `email` - Email address
   - ☑ `User.Read` - Read user profile
   - ☑ `offline_access` - Refresh tokens (optional)

4. Click **Add permissions**

5. **Grant admin consent** (if you have admin rights):
   - Click **Grant admin consent for [Your Organization]**
   - Click **Yes** to confirm
   - Status should show green checkmarks

   If you don't have admin rights, request consent from your Azure AD administrator.

### Step 6: Branding (Optional)

1. Navigate to **Branding & properties**

2. Configure:
   - **Name**: User-friendly name shown during consent
   - **Logo**: Your application logo (optional)
   - **Home page URL**: `https://yourdomain.com`
   - **Terms of service URL**: Link to your TOS
   - **Privacy statement URL**: Link to privacy policy
   - **Publisher domain**: Your verified domain

3. Click **Save**

### Step 7: Token Configuration (Optional but Recommended)

1. Navigate to **Token configuration**

2. **Add optional claims** to ID token:

   Click **+ Add optional claim** → **ID** → Select:
   - ☑ `email`
   - ☑ `family_name`
   - ☑ `given_name`
   - ☑ `upn` (User Principal Name)

3. Click **Add**

4. If prompted about Microsoft Graph permissions, click **Yes**

### Step 8: Enterprise Application Settings (Optional)

1. Go to **Azure AD** → **Enterprise applications**
2. Find your application
3. Navigate to **Properties**

4. Configure:
   - **Enabled for users to sign-in?**: Yes
   - **Assignment required?**:
     - Yes = Only assigned users can login
     - No = All tenant users can login
   - **Visible to users?**: Your choice

5. If **Assignment required** is Yes:
   - Navigate to **Users and groups**
   - Click **+ Add user/group**
   - Assign users or groups

## Part 2: IdentityServer Configuration

### Step 1: Update appsettings.json

Navigate to `IdentityServer/src/IdentityServer.Host/appsettings.json`:

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{YOUR_TENANT_ID}/v2.0",
    "ClientId": "{YOUR_CLIENT_ID}",
    "ClientSecret": "{YOUR_CLIENT_SECRET}",
    "TenantId": "{YOUR_TENANT_ID}",
    "CallbackPath": "/signin-azuread",
    "SignedOutCallbackPath": "/signout-callback-azuread",
    "RemoteSignOutPath": "/signout-azuread"
  }
}
```

**Example with actual values:**

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/12345678-1234-1234-1234-123456789012/v2.0",
    "ClientId": "87654321-4321-4321-4321-210987654321",
    "ClientSecret": "abc123~DefGhI456JklMnoPqRsTuVwXyZ",
    "TenantId": "12345678-1234-1234-1234-123456789012",
    "CallbackPath": "/signin-azuread",
    "SignedOutCallbackPath": "/signout-callback-azuread",
    "RemoteSignOutPath": "/signout-azuread"
  }
}
```

### Step 2: Secure Secrets (Production)

**Option 1: User Secrets (Development)**

```bash
cd IdentityServer/src/IdentityServer.Host

dotnet user-secrets init
dotnet user-secrets set "AzureAd:ClientSecret" "your-secret-value"
```

**Option 2: Environment Variables (Production)**

```bash
export AzureAd__ClientSecret="your-secret-value"
export AzureAd__ClientId="your-client-id"
export AzureAd__TenantId="your-tenant-id"
```

**Option 3: Azure Key Vault (Production - Recommended)**

```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Step 3: Verify Program.cs Configuration

The `Program.cs` file should already have Azure AD configured:

```csharp
builder.Services.AddAuthentication()
    .AddOpenIdConnect("AzureAD", "Microsoft Entra", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        options.SignOutScheme = IdentityServerConstants.SignoutScheme;

        options.Authority = builder.Configuration["AzureAd:Authority"];
        options.ClientId = builder.Configuration["AzureAd:ClientId"];
        options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
        options.ResponseType = "code";

        options.CallbackPath = "/signin-azuread";
        options.SignedOutCallbackPath = "/signout-callback-azuread";
        options.RemoteSignOutPath = "/signout-azuread";

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
```

## Part 3: Testing the Integration

### Step 1: Start IdentityServer

```bash
cd IdentityServer/src/IdentityServer.Host
dotnet run
```

### Step 2: Navigate to Login Page

Open browser: `https://localhost:5001/Account/Login`

You should see **Microsoft Entra** as an external login option.

### Step 3: Test Login Flow

1. Click **Microsoft Entra** button
2. Redirected to Microsoft login page
3. Enter Microsoft credentials
4. Grant consent (if prompted)
5. Redirected back to IdentityServer
6. User account automatically created

### Step 4: Verify User Creation

Check logs or database:

```sql
SELECT * FROM Users WHERE Email LIKE '%microsoft%';
```

Or view in IdentityServer UI.

## Part 4: Advanced Configuration

### Multi-Tenant Support

For multiple Azure AD tenants:

```csharp
// In Program.cs, add multiple providers
builder.Services.AddAuthentication()
    .AddOpenIdConnect("AzureAD-Tenant1", "Organization A", options => { /* ... */ })
    .AddOpenIdConnect("AzureAD-Tenant2", "Organization B", options => { /* ... */ });
```

### Custom Claims Mapping

Map additional claims from Azure AD:

```csharp
.AddOpenIdConnect("AzureAD", "Microsoft Entra", options =>
{
    // ... existing configuration

    // Map additional claims
    options.ClaimActions.MapJsonKey("employee_id", "employee_id");
    options.ClaimActions.MapJsonKey("department", "department");
    options.ClaimActions.MapJsonKey("job_title", "jobTitle");
    options.ClaimActions.MapJsonKey("office", "officeLocation");

    // Remove claims you don't want
    options.ClaimActions.DeleteClaim("nonce");
    options.ClaimActions.DeleteClaim("aud");
});
```

### Automatic User Provisioning

Customize user creation in a custom event handler:

```csharp
.AddOpenIdConnect("AzureAD", "Microsoft Entra", options =>
{
    // ... existing configuration

    options.Events = new OpenIdConnectEvents
    {
        OnTicketReceived = async context =>
        {
            var userManager = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = context.Principal?.FindFirst(ClaimTypes.GivenName)?.Value,
                    LastName = context.Principal?.FindFirst(ClaimTypes.Surname)?.Value,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user);

                // Assign default role
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    };
});
```

### Group-Based Authorization

Map Azure AD groups to roles:

```csharp
options.Events = new OpenIdConnectEvents
{
    OnTokenValidated = async context =>
    {
        var userManager = context.HttpContext.RequestServices
            .GetRequiredService<UserManager<ApplicationUser>>();

        var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var user = await userManager.FindByEmailAsync(email);

        if (user != null)
        {
            // Map Azure AD groups to roles
            var groups = context.Principal?.FindAll("groups");

            if (groups?.Any(g => g.Value == "admin-group-id") == true)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
};
```

## Part 5: Production Considerations

### Security Checklist

- [ ] Use HTTPS everywhere (valid SSL certificate)
- [ ] Store secrets in Azure Key Vault or similar
- [ ] Enable token encryption in Azure AD
- [ ] Set appropriate token lifetimes
- [ ] Enable conditional access policies (Azure AD Premium)
- [ ] Configure MFA requirements
- [ ] Regular security audits
- [ ] Monitor sign-in logs

### Redirect URIs for Production

In Azure Portal, add production redirect URIs:

```
https://yourdomain.com/signin-azuread
https://yourdomain.com/signout-callback-azuread
https://api.yourdomain.com/signin-azuread (if separate API)
```

### Token Lifetime Configuration

In Azure Portal → Token configuration:

- **Access token**: 60-90 minutes
- **Refresh token**: 14 days to 90 days
- **ID token**: 60-90 minutes

### Monitoring and Logging

Enable Application Insights:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

Monitor:
- Failed login attempts
- Token validation errors
- Unusual login patterns
- Performance metrics

## Troubleshooting

### Error: "AADSTS50011: The reply URL specified in the request does not match"

**Solution**:
1. Check redirect URI in Azure Portal matches exactly (case-sensitive)
2. Ensure protocol (http/https) matches
3. Verify port number if using localhost

### Error: "AADSTS700016: Application not found in directory"

**Solution**:
1. Verify ClientId in appsettings.json
2. Ensure you're in correct Azure AD tenant
3. Check TenantId configuration

### Error: "AADSTS7000215: Invalid client secret provided"

**Solution**:
1. Client secret may have expired
2. Create new secret in Azure Portal
3. Update appsettings.json with new secret

### Error: "AADSTS650052: The app needs access to a service"

**Solution**:
1. Grant admin consent in Azure Portal
2. Or contact Azure AD administrator

### Error: "Correlation failed" or "Invalid state"

**Solution**:
1. Ensure cookies are enabled
2. Check Data Protection is configured
3. Verify HTTPS is enabled

### Users Not Auto-Provisioned

**Solution**:
1. Check OnTicketReceived event handler
2. Verify database connection
3. Check logs for errors
4. Ensure UserManager is properly configured

## Testing Checklist

- [ ] User can login with Microsoft account
- [ ] User account created automatically
- [ ] Claims populated correctly
- [ ] User can logout
- [ ] Tokens refresh properly
- [ ] Error handling works
- [ ] Multiple logins don't create duplicate users
- [ ] Production redirect URIs configured
- [ ] SSL certificate valid
- [ ] Secrets secured properly

## Additional Resources

- [Microsoft identity platform documentation](https://learn.microsoft.com/en-us/entra/identity-platform/)
- [Azure AD app registration](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app)
- [OpenID Connect with Azure AD](https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc)
- [Duende IdentityServer External Authentication](https://docs.duendesoftware.com/identityserver/v7/ui/login/external/)

## Support

For issues:
- Azure AD: https://azure.microsoft.com/support/
- IdentityServer: https://github.com/DuendeSoftware/IdentityServer/discussions
- Stack Overflow: Tag with `azure-ad` and `identityserver`
