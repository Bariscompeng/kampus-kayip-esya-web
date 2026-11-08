using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Services;

public class ItemService(AppDbContext context, IPhotoService photoService) : IItemService
{
    public async Task<PagedResponse<Item>> SearchAsync(ItemSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = context.Items.AsNoTracking().Where(i => i.Status == ItemStatus.Published);

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var q = request.Q.Trim();
            query = query.Where(i => i.Title.Contains(q) || i.Description.Contains(q));
        }
        if (request.Type is ItemType type) query = query.Where(i => i.Type == type);
        if (request.CategoryId is int categoryId) query = query.Where(i => i.CategoryId == categoryId);
        if (request.LocationId is int locationId) query = query.Where(i => i.LocationId == locationId);
        if (request.From is DateTime from) query = query.Where(i => i.EventDate >= from.Date);
        if (request.To is DateTime to) query = query.Where(i => i.EventDate <= to.Date);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Photos)
            .OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedResponse<Item>(items, request.Page, request.PageSize, totalCount);
    }

    public async Task<Item?> GetVisibleAsync(int id, ItemViewer viewer, CancellationToken cancellationToken = default)
    {
        var item = await context.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Owner)
            .Include(i => i.Photos.OrderBy(p => p.Order))
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        return item is not null && CanSee(item, viewer) ? item : null;
    }

    public async Task<Dictionary<string, string>> ValidateReferencesAsync(int categoryId, int locationId, CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string>();

        if (!await context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken))
            errors[nameof(ItemInput.CategoryId)] = "Geçerli bir kategori seçin.";
        if (!await context.Locations.AnyAsync(l => l.Id == locationId, cancellationToken))
            errors[nameof(ItemInput.LocationId)] = "Geçerli bir konum seçin.";

        return errors;
    }

    public async Task<Item> CreateAsync(ItemInput input, string ownerId, IReadOnlyCollection<IFormFile>? photos = null, CancellationToken cancellationToken = default)
    {
        var item = new Item { OwnerId = ownerId, Status = ItemStatus.Pending };
        Apply(item, input);

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        context.Items.Add(item);
        await context.SaveChangesAsync(cancellationToken);
        if (photos is { Count: > 0 })
            await photoService.SaveAsync(item.Id, photos, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return item;
    }

    public async Task<ItemChangeResult> UpdateAsync(int id, ItemInput input, ItemViewer viewer, CancellationToken cancellationToken = default)
    {
        var item = await context.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null || !CanSee(item, viewer)) return ItemChangeResult.NotFound;
        if (!viewer.Owns(item) && !viewer.IsStaff) return ItemChangeResult.Forbidden;
        if (item.Status is ItemStatus.Resolved or ItemStatus.Archived) return ItemChangeResult.Conflict;

        Apply(item, input);

        // Sahibinin değişikliği yeniden onaya düşer; yoksa onaylı ilana onaysız içerik yazılabilirdi.
        if (!viewer.IsStaff)
        {
            item.Status = ItemStatus.Pending;
            item.ApprovedById = null;
            item.ApprovedAt = null;
        }

        await context.SaveChangesAsync(cancellationToken);
        return ItemChangeResult.Success;
    }

    public async Task<ItemChangeResult> DeleteAsync(int id, ItemViewer viewer, CancellationToken cancellationToken = default)
    {
        var item = await context.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null || !CanSee(item, viewer)) return ItemChangeResult.NotFound;
        if (!viewer.Owns(item) && !viewer.IsStaff) return ItemChangeResult.Forbidden;

        // Teslim kaydı geçmişi korunur; yalnızca görevli silebilir.
        if (!viewer.IsStaff && item.Status is ItemStatus.Resolved or ItemStatus.Archived)
            return ItemChangeResult.Conflict;

        context.Items.Remove(item);
        await context.SaveChangesAsync(cancellationToken);
        photoService.DeleteAll(id);

        return ItemChangeResult.Success;
    }

    private static bool CanSee(Item item, ItemViewer viewer) =>
        item.Status == ItemStatus.Published || viewer.Owns(item) || viewer.IsStaff;

    private static void Apply(Item item, ItemInput input)
    {
        item.Type = input.Type;
        item.Title = input.Title.Trim();
        item.Description = input.Description.Trim();
        item.EventDate = input.EventDate.Date;
        item.VerificationQuestion = input.Type == ItemType.Found ? input.VerificationQuestion!.Trim() : null;
        item.CategoryId = input.CategoryId;
        item.LocationId = input.LocationId;
    }
}
