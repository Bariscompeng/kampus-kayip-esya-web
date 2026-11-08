using System.Security.Claims;

namespace KampusKayipEsya.Web.Models;

// İlana bakan kişi: anonimse UserId boştur.
public record ItemViewer(string? UserId, bool IsStaff)
{
    public static ItemViewer From(ClaimsPrincipal user) => new(
        user.FindFirstValue(ClaimTypes.NameIdentifier),
        user.IsInRole(Roles.Gorevli) || user.IsInRole(Roles.Admin));

    public bool Owns(Item item) => UserId is not null && item.OwnerId == UserId;
}
