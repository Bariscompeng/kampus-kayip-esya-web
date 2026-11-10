using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Services;

public enum ItemChangeResult
{
    Success,
    NotFound,
    Forbidden,
    Conflict
}

// İlan iş kuralları tek yerde: MVC formu ve /api/items aynı servisi kullanır.
public interface IItemService
{
    Task<PagedResponse<Item>> SearchAsync(ItemSearchRequest request, CancellationToken cancellationToken = default);

    // Görme yetkisi yoksa ilan hiç yokmuş gibi null döner.
    Task<Item?> GetVisibleAsync(int id, ItemViewer viewer, CancellationToken cancellationToken = default);

    // Alan adı → hata mesajı; boşsa kategori ve konum geçerlidir.
    Task<Dictionary<string, string>> ValidateReferencesAsync(int categoryId, int locationId, CancellationToken cancellationToken = default);

    Task<Item> CreateAsync(ItemInput input, string ownerId, IReadOnlyCollection<IFormFile>? photos = null, CancellationToken cancellationToken = default);

    Task<ItemChangeResult> UpdateAsync(int id, ItemInput input, ItemViewer viewer, CancellationToken cancellationToken = default);

    Task<ItemChangeResult> DeleteAsync(int id, ItemViewer viewer, CancellationToken cancellationToken = default);

    Task<PagedResponse<Item>> GetOwnedAsync(string ownerId, ItemStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<ItemChangeResult> MarkResolvedAsync(int id, ItemViewer viewer, CancellationToken cancellationToken = default);

    Task<PhotoChangeResult> AddPhotosAsync(int id, IReadOnlyCollection<IFormFile> photos, ItemViewer viewer, CancellationToken cancellationToken = default);
}

public record PhotoChangeResult(ItemChangeResult Result, IReadOnlyList<string> Errors);
