using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.ViewModels.Validation;

namespace KampusKayipEsya.Web.Dtos;

public class ItemSearchRequest
{
    public string? Q { get; set; }
    public ItemType? Type { get; set; }
    public int? CategoryId { get; set; }
    public int? LocationId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası en az 1 olmalıdır.")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Sayfa boyutu {1}–{2} arasında olmalıdır.")]
    public int PageSize { get; set; } = 20;
}

// POST ve PUT aynı gövdeyi kullanır; kurallar web formundaki ItemCreateViewModel ile aynıdır.
public class ItemWriteRequest
{
    [Required(ErrorMessage = "İlan türünü seçin.")]
    public ItemType? Type { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Başlık {2}–{1} karakter olmalıdır.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçin.")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Konum seçin.")]
    public int? LocationId { get; set; }

    [Required(ErrorMessage = "Olay tarihini girin.")]
    [NotInFuture(ErrorMessage = "Olay tarihi bugünden sonra olamaz.")]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama {2}–{1} karakter olmalıdır.")]
    public string Description { get; set; } = string.Empty;

    [RequiredForFound(nameof(Type), ErrorMessage = "Bulunan eşya için doğrulama sorusu zorunludur; sahibi bu soruyu cevaplayarak eşyanın kendisine ait olduğunu gösterecek.")]
    [StringLength(300, ErrorMessage = "Doğrulama sorusu en fazla {1} karakter olabilir.")]
    public string? VerificationQuestion { get; set; }

    public ItemInput ToInput() => new(
        Type!.Value, Title, Description, EventDate!.Value, CategoryId!.Value, LocationId!.Value, VerificationQuestion);
}

public record ItemInput(
    ItemType Type,
    string Title,
    string Description,
    DateTime EventDate,
    int CategoryId,
    int LocationId,
    string? VerificationQuestion);

public record ItemListDto(
    int Id,
    ItemType Type,
    ItemStatus Status,
    string Title,
    int CategoryId,
    string CategoryName,
    string? CategoryIcon,
    int LocationId,
    string LocationName,
    DateTime EventDate,
    DateTime CreatedAt,
    string? CoverPhotoUrl);

// E-posta, telefon ve OwnerId bilinçli olarak yok; VerificationQuestion yalnızca sahibine ve görevliye dolu gelir.
public record ItemDetailDto(
    int Id,
    ItemType Type,
    ItemStatus Status,
    string Title,
    string Description,
    int CategoryId,
    string CategoryName,
    string? CategoryIcon,
    int LocationId,
    string LocationName,
    DateTime EventDate,
    DateTime CreatedAt,
    bool IsAtOffice,
    string OwnerName,
    bool IsOwner,
    string? VerificationQuestion,
    IReadOnlyList<string> PhotoUrls);

public static class ItemDtoMapper
{
    public static ItemListDto ToListDto(this Item item, Func<string, string> photoUrl) => new(
        item.Id,
        item.Type,
        item.Status,
        item.Title,
        item.CategoryId,
        item.Category.Name,
        item.Category.Icon,
        item.LocationId,
        LocationName(item.Location),
        AsUtc(item.EventDate),
        AsUtc(item.CreatedAt),
        item.Photos.OrderBy(p => p.Order).Select(p => photoUrl(p.Path)).FirstOrDefault());

    public static ItemDetailDto ToDetailDto(this Item item, ItemViewer viewer, Func<string, string> photoUrl) => new(
        item.Id,
        item.Type,
        item.Status,
        item.Title,
        item.Description,
        item.CategoryId,
        item.Category.Name,
        item.Category.Icon,
        item.LocationId,
        LocationName(item.Location),
        AsUtc(item.EventDate),
        AsUtc(item.CreatedAt),
        item.IsAtOffice,
        item.Owner.ShortName,
        viewer.Owns(item),
        viewer.Owns(item) || viewer.IsStaff ? item.VerificationQuestion : null,
        item.Photos.OrderBy(p => p.Order).Select(p => photoUrl(p.Path)).ToList());

    private static string LocationName(Location location) =>
        location.FloorOrRoom is null ? location.Building : $"{location.Building} – {location.FloorOrRoom}";

    // SQL Server tarihi Kind'sız döndürür; JSON'a "Z" ile çıksın diye UTC olarak işaretlenir.
    private static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
