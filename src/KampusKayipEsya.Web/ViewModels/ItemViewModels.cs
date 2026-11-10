using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.ViewModels.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KampusKayipEsya.Web.ViewModels;

public class ItemCreateViewModel
{
    [Required(ErrorMessage = "İlan türünü seçin.")]
    [Display(Name = "İlan türü")]
    public ItemType? Type { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Başlık {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçin.")]
    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Konum seçin.")]
    [Display(Name = "Bina / derslik")]
    public int? LocationId { get; set; }

    [Required(ErrorMessage = "Olay tarihini girin.")]
    [NotInFuture(ErrorMessage = "Olay tarihi bugünden sonra olamaz.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ne zaman kayboldu / bulundu?")]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = string.Empty;

    [RequiredForFound(nameof(Type), ErrorMessage = "Bulunan eşya için doğrulama sorusu zorunludur; sahibi bu soruyu cevaplayarak eşyanın kendisine ait olduğunu gösterecek.")]
    [StringLength(300, ErrorMessage = "Doğrulama sorusu en fazla {1} karakter olabilir.")]
    [Display(Name = "Doğrulama sorusu")]
    public string? VerificationQuestion { get; set; }

    [Display(Name = "Fotoğraflar")]
    public List<IFormFile> Photos { get; set; } = [];

    public IEnumerable<SelectListItem> Categories { get; set; } = [];
    public IEnumerable<SelectListItem> Locations { get; set; } = [];
}

public class ItemEditViewModel : ItemCreateViewModel
{
    public int Id { get; set; }
    public ItemStatus Status { get; set; }
    public List<int> RemovePhotoIds { get; set; } = [];
    [ValidateNever]
    public IReadOnlyList<ItemPhoto> ExistingPhotos { get; set; } = [];

    public bool OnayaDuser => Status is ItemStatus.Published or ItemStatus.Rejected;
}

public record ItemDetailsViewModel(Item Item, bool IsOwner, bool CanSeeQuestion, string OwnerShortName);

public class MyItemsViewModel(IReadOnlyList<Item> items, string? sekme)
{
    public const string Bekleyen = "bekleyen";
    public const string Yayinda = "yayinda";
    public const string Kapanmis = "kapanmis";

    public IReadOnlyList<Item> Bekleyenler { get; } = items.Where(i => i.Status == ItemStatus.Pending).ToList();
    public IReadOnlyList<Item> Yayindakiler { get; } = items.Where(i => i.Status == ItemStatus.Published).ToList();
    public IReadOnlyList<Item> Kapananlar { get; } = items
        .Where(i => i.Status is ItemStatus.Resolved or ItemStatus.Archived or ItemStatus.Rejected).ToList();

    public string AktifSekme { get; } = sekme is Bekleyen or Yayinda or Kapanmis
        ? sekme
        : items.Any(i => i.Status == ItemStatus.Pending) ? Bekleyen : Yayinda;

    public static string SekmeOf(ItemStatus status) => status switch
    {
        ItemStatus.Pending => Bekleyen,
        ItemStatus.Published => Yayinda,
        _ => Kapanmis
    };
}
