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

    public static string BadgeClass(this ItemStatus status) => status switch
    {
        ItemStatus.Pending => "text-bg-warning",
        ItemStatus.Published => "text-bg-success",
        ItemStatus.Rejected => "text-bg-danger",
        _ => "text-bg-secondary"
    };
}
