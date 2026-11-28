using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Models;

public class ItemClaim
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string ClaimantId { get; set; } = string.Empty;
    public AppUser Claimant { get; set; } = null!;

    public string VerificationAnswer { get; set; } = string.Empty;
    public string? Message { get; set; }

    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DecidedAt { get; set; }
}
