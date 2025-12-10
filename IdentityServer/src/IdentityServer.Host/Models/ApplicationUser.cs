using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Host.Models;

/// <summary>
/// Application user entity extending IdentityUser
/// Add custom properties as needed
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
