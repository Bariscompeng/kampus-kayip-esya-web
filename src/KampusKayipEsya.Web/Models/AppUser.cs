using Microsoft.AspNetCore.Identity;

namespace KampusKayipEsya.Web.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? StudentNo { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
