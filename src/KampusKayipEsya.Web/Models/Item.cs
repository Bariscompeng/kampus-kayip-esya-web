using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Models;

public class Item
{
    public int Id { get; set; }
    public ItemType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.Pending;
    public string? VerificationQuestion { get; set; }
    public bool IsAtOffice { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public string OwnerId { get; set; } = string.Empty;
    public AppUser Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ApprovedById { get; set; }
    public AppUser? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
