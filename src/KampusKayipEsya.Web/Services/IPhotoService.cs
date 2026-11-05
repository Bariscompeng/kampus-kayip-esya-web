using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.Services;

public interface IPhotoService
{
    IReadOnlyList<string> Validate(IReadOnlyCollection<IFormFile> files, int existingCount = 0);

    Task<List<ItemPhoto>> SaveAsync(int itemId, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken = default);

    void DeleteAll(int itemId);
}
