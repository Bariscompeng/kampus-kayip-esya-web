using System.Globalization;
using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.ViewModels;

public record HomeIndexViewModel(
    IReadOnlyList<Category> Categories,
    IReadOnlyList<Item> SonBulunanlar,
    IReadOnlyList<Item> SonKayiplar)
{
    public static string GoreliZaman(DateTime olayTarihi)
    {
        var gun = (DateTime.Today - olayTarihi.Date).Days;
        return gun switch
        {
            <= 0 => "Bugün",
            1 => "Dün",
            < 7 => $"{gun} gün önce",
            _ => olayTarihi.ToString("d MMMM", new CultureInfo("tr-TR"))
        };
    }
}
