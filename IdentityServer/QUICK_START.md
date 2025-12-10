# Quick Start Guide - 5 Minutes to Running IdentityServer

Get Duende IdentityServer v7 running in 5 minutes!

## Step 1: Prerequisites (1 minute)

Install .NET 8 SDK:
- Windows: Download from https://dotnet.microsoft.com/download/dotnet/8.0
- macOS: `brew install dotnet@8`
- Linux: Follow instructions at https://learn.microsoft.com/en-us/dotnet/core/install/linux

Verify:
```bash
dotnet --version
# Should show 8.0.x
```

## Step 2: Configure Database (1 minute)

### Option A: SQL Server LocalDB (Windows - Easiest)

Already installed with Visual Studio! Just use default connection string.

### Option B: Docker SQL Server (macOS/Linux - Recommended)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver -d \
   mcr.microsoft.com/mssql/server:2022-latest
```

Then update `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DuendeIdentityServer;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
}
```

## Step 3: Configure Azure AD (2 minutes)

### Register App in Azure Portal

1. Go to https://portal.azure.com
2. Azure Active Directory → App registrations → New registration
3. Name: `IdentityServer`
4. Redirect URI: `Web` → `https://localhost:5001/signin-azuread`
5. Click Register
6. Copy **Application (client) ID** and **Directory (tenant) ID**
7. Certificates & secrets → New client secret → Copy value
8. Authentication → Add redirect URI: `https://localhost:5001/signout-callback-azuread`

### Update appsettings.json

Edit `IdentityServer/src/IdentityServer.Host/appsettings.json`:

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{YOUR_TENANT_ID}/v2.0",
    "ClientId": "{YOUR_CLIENT_ID}",
    "ClientSecret": "{YOUR_CLIENT_SECRET}",
    "TenantId": "{YOUR_TENANT_ID}"
  }
}
```

Replace `{YOUR_TENANT_ID}`, `{YOUR_CLIENT_ID}`, and `{YOUR_CLIENT_SECRET}` with values from Azure Portal.

## Step 4: Run IdentityServer (1 minute)

```bash
cd IdentityServer/src/IdentityServer.Host

# Restore packages (first time only)
dotnet restore

# Run
dotnet run
```

You should see:
```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```

## Step 5: Test (30 seconds)

Open browser: `https://localhost:5001`

You should see the IdentityServer welcome page!

Test discovery endpoint: `https://localhost:5001/.well-known/openid-configuration`

## Default Admin Credentials

- Username: `admin`
- Password: `Admin@123!`

**⚠️ Change immediately in production!**

## What's Configured?

✅ Duende IdentityServer v7
✅ .NET 8
✅ SQL Server database
✅ ASP.NET Core Identity
✅ Microsoft Entra (Azure AD) login
✅ Ready for RSK Admin UI
✅ Sample clients configured

## Next Steps

### 1. Create Your First Client Application

See `README.md` section: "Client Configuration Examples"

### 2. Set Up RSK Admin UI

See `RSK_ADMIN_UI_INTEGRATION.md` for full guide

### 3. Customize Configuration

Edit `Data/Config.cs` to add/modify:
- Clients
- API Resources
- Identity Resources
- API Scopes

### 4. Test Microsoft Entra Login

1. Navigate to: `https://localhost:5001/Account/Login`
2. Click "Microsoft Entra"
3. Sign in with Microsoft account
4. Verify user created

## Common Issues

### Database Connection Failed

**Windows (LocalDB):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DuendeIdentityServer;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Docker:**
Ensure Docker container is running:
```bash
docker ps
```

### Azure AD Login Error

Verify in Azure Portal:
1. Redirect URIs match exactly
2. Client secret not expired
3. API permissions granted

### Port Already in Use

Change port in `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:5002;http://localhost:5001"
```

## Full Documentation

- [README.md](README.md) - Complete overview
- [SETUP_GUIDE.md](SETUP_GUIDE.md) - Detailed step-by-step setup
- [MICROSOFT_ENTRA_SETUP.md](MICROSOFT_ENTRA_SETUP.md) - Azure AD configuration
- [RSK_ADMIN_UI_INTEGRATION.md](RSK_ADMIN_UI_INTEGRATION.md) - Admin UI setup
- [MIGRATION_COMMANDS.md](MIGRATION_COMMANDS.md) - Database migrations

## Need Help?

- Duende Docs: https://docs.duendesoftware.com/identityserver/v7
- GitHub Issues: https://github.com/DuendeSoftware/IdentityServer/issues
- Community: https://github.com/DuendeSoftware/IdentityServer/discussions

## What's Next?

Now that IdentityServer is running, you can:

1. **Create client applications** (Web, SPA, Mobile, API)
2. **Configure RSK Admin UI** for easy management
3. **Add more external providers** (Google, Facebook, GitHub)
4. **Customize UI** with your branding
5. **Deploy to production** (Azure, AWS, on-premise)

Happy authenticating! 🎉
