using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer.Host.Data;

/// <summary>
/// Configuration for IdentityServer resources and clients
/// This will be managed via RSK Admin UI in production
/// </summary>
public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "roles",
                DisplayName = "Roles",
                UserClaims = new[] { "role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("api1", "My API"),
            new ApiScope("admin", "Admin API"),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("api1", "My API")
            {
                Scopes = { "api1" },
                UserClaims = new[] { "role", "email", "name" }
            },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // Interactive client using code flow + PKCE
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:5002/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:5002/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1",
                    "roles"
                },

                RequirePkce = true,
                RequireConsent = false
            },

            // Machine to machine client
            new Client
            {
                ClientId = "m2m",
                ClientName = "Machine to Machine Client",
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "api1", "admin" }
            },

            // SPA client using PKCE
            new Client
            {
                ClientId = "spa",
                ClientName = "SPA Client",
                ClientUri = "https://localhost:5003",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris =
                {
                    "https://localhost:5003/callback",
                    "https://localhost:5003/silent-renew.html"
                },

                PostLogoutRedirectUris = { "https://localhost:5003/" },
                AllowedCorsOrigins = { "https://localhost:5003" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1"
                },

                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AccessTokenLifetime = 3600
            },

            // RSK Admin UI Client (if using Admin UI)
            new Client
            {
                ClientId = "rsk-admin-ui",
                ClientName = "RSK Admin UI",
                ClientSecrets = { new Secret("RSK-ADMIN-UI-SECRET".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,

                RedirectUris = { "https://localhost:5001/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.LocalApi.ScopeName
                },

                AllowOfflineAccess = true
            }
        };
}
