namespace KampusKayipEsya.Web.Models.Enums;

public static class ClaimEnumExtensions
{
    public static string ToTurkish(this ClaimStatus status) => status switch
    {
        ClaimStatus.Pending => "Yanıt bekliyor",
        ClaimStatus.Approved => "Onaylandı",
        ClaimStatus.Rejected => "Reddedildi",
        ClaimStatus.Cancelled => "Geri çekildi",
        _ => status.ToString()
    };

    public static string BadgeClass(this ClaimStatus status) => status switch
    {
        ClaimStatus.Pending => "status status-pending",
        ClaimStatus.Approved => "status status-published",
        ClaimStatus.Rejected => "status status-rejected",
        _ => "status status-closed"
    };
}
