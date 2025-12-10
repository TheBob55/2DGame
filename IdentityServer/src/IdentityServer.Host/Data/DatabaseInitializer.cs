using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityServer.Host.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Host.Data;

/// <summary>
/// Initializes the database with seed data
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // Migrate and seed ApplicationDbContext
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Migrate PersistedGrantDbContext
        var persistedGrantContext = serviceProvider.GetRequiredService<PersistedGrantDbContext>();
        await persistedGrantContext.Database.MigrateAsync();

        // Migrate and seed ConfigurationDbContext
        var configurationContext = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        await configurationContext.Database.MigrateAsync();

        // Seed configuration data if not already present
        if (!configurationContext.Clients.Any())
        {
            foreach (var client in Config.Clients)
            {
                configurationContext.Clients.Add(client.ToEntity());
            }
            await configurationContext.SaveChangesAsync();
        }

        if (!configurationContext.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                configurationContext.IdentityResources.Add(resource.ToEntity());
            }
            await configurationContext.SaveChangesAsync();
        }

        if (!configurationContext.ApiScopes.Any())
        {
            foreach (var scope in Config.ApiScopes)
            {
                configurationContext.ApiScopes.Add(scope.ToEntity());
            }
            await configurationContext.SaveChangesAsync();
        }

        if (!configurationContext.ApiResources.Any())
        {
            foreach (var resource in Config.ApiResources)
            {
                configurationContext.ApiResources.Add(resource.ToEntity());
            }
            await configurationContext.SaveChangesAsync();
        }

        // Create default admin user
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create admin role
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create default admin user
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@yourdomain.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                IsActive = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
