using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KampusKayipEsya.Web.ViewModels;

public record ItemListViewModel(
    ItemSearchRequest Filtre,
    PagedResponse<Item> Sonuc,
    IEnumerable<SelectListItem> Kategoriler,
    IEnumerable<SelectListItem> Konumlar)
{
    public int SayfaSayisi => Math.Max(1, (int)Math.Ceiling(Sonuc.TotalCount / (double)Sonuc.PageSize));

    public bool FiltreVarMi =>
        !string.IsNullOrWhiteSpace(Filtre.Q) || Filtre.Type is not null || Filtre.CategoryId is not null
        || Filtre.LocationId is not null || Filtre.From is not null || Filtre.To is not null;

    // Sayfa bağlantıları mevcut filtreleri korur (?categoryId=2&page=3).
    public Dictionary<string, string> SayfaAdresi(int sayfa)
    {
        var degerler = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(Filtre.Q)) degerler["q"] = Filtre.Q.Trim();
        if (Filtre.Type is not null) degerler["type"] = Filtre.Type.Value.ToString();
        if (Filtre.CategoryId is not null) degerler["categoryId"] = Filtre.CategoryId.Value.ToString();
        if (Filtre.LocationId is not null) degerler["locationId"] = Filtre.LocationId.Value.ToString();
        if (Filtre.From is not null) degerler["from"] = Filtre.From.Value.ToString("yyyy-MM-dd");
        if (Filtre.To is not null) degerler["to"] = Filtre.To.Value.ToString("yyyy-MM-dd");
        if (sayfa > 1) degerler["page"] = sayfa.ToString();
        return degerler;
    }
}
