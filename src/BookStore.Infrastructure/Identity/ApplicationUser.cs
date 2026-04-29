using Microsoft.AspNetCore.Identity;

namespace BookStore.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public Guid? CustomerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
}
