namespace KampusKayipEsya.Web.Models.Enums;

public static class ItemEnumExtensions
{
    public static string ToTurkish(this ItemType type) => type switch
    {
        ItemType.Lost => "Kayıp",
        ItemType.Found => "Bulundu",
        _ => type.ToString()
    };

    public static string ToTurkish(this ItemStatus status) => status switch
    {
        ItemStatus.Pending => "Onay bekliyor",
        ItemStatus.Published => "Yayında",
        ItemStatus.Rejected => "Reddedildi",
        ItemStatus.Resolved => "Teslim edildi",
        ItemStatus.Archived => "Arşivlendi",
        _ => status.ToString()
    };

    /// <summary>Durum rozetinin CSS sınıfı (site.css: .status-*).</summary>
    public static string BadgeClass(this ItemStatus status) => status switch
    {
        ItemStatus.Pending => "status status-pending",
        ItemStatus.Published => "status status-published",
        ItemStatus.Rejected => "status status-rejected",
        _ => "status status-closed"
    };

    /// <summary>İlan türü etiketinin CSS sınıfı (site.css: .tag-*).</summary>
    public static string TagClass(this ItemType type) => type == ItemType.Found ? "tag tag-found" : "tag tag-lost";
}
