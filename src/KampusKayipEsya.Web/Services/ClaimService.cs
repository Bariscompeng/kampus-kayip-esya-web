using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Services;

public class ClaimService(AppDbContext context, INotificationService notificationService) : IClaimService
{
    public Task<Item?> GetClaimableItemAsync(int itemId, CancellationToken cancellationToken = default) =>
        context.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Photos.OrderBy(p => p.Order))
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Status == ItemStatus.Published && i.Type == ItemType.Found, cancellationToken);

    public async Task<ClaimCreateOutcome> CreateAsync(int itemId, string claimantId, ClaimInput input, CancellationToken cancellationToken = default)
    {
        var item = await context.Items.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Status == ItemStatus.Published && i.Type == ItemType.Found, cancellationToken);
        if (item is null) return new(ClaimCreateResult.NotFound, null);
        if (item.OwnerId == claimantId) return new(ClaimCreateResult.OwnItem, null);
        if (await GetActiveClaimAsync(itemId, claimantId, cancellationToken) is not null)
            return new(ClaimCreateResult.AlreadyClaimed, null);

        var claim = new ItemClaim
        {
            ItemId = itemId,
            ClaimantId = claimantId,
            VerificationAnswer = input.VerificationAnswer.Trim(),
            Message = string.IsNullOrWhiteSpace(input.Message) ? null : input.Message.Trim()
        };
        context.Claims.Add(claim);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new(ClaimCreateResult.AlreadyClaimed, null);
        }

        await notificationService.CreateAsync(item.OwnerId, NotificationType.ClaimReceived,
            $"\"{item.Title}\" ilanınıza bir sahiplenme talebi geldi.", "/gelen-talepler", cancellationToken);
        return new(ClaimCreateResult.Success, claim);
    }

    public Task<ItemClaim?> GetActiveClaimAsync(int itemId, string claimantId, CancellationToken cancellationToken = default) =>
        context.Claims
            .AsNoTracking()
            .Where(c => c.ItemId == itemId && c.ClaimantId == claimantId && c.Status != ClaimStatus.Cancelled)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ItemClaim>> GetMineAsync(string claimantId, CancellationToken cancellationToken = default) =>
        await context.Claims
            .AsNoTracking()
            .Where(c => c.ClaimantId == claimantId)
            .Include(c => c.Item).ThenInclude(i => i.Location)
            .Include(c => c.Item).ThenInclude(i => i.Category)
            .Include(c => c.Item).ThenInclude(i => i.Photos)
            .OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ItemClaim>> GetIncomingAsync(string ownerId, CancellationToken cancellationToken = default) =>
        await context.Claims
            .AsNoTracking()
            .Where(c => c.Item.OwnerId == ownerId && c.Status != ClaimStatus.Cancelled)
            .Include(c => c.Claimant)
            .Include(c => c.Item).ThenInclude(i => i.Location)
            .Include(c => c.Item).ThenInclude(i => i.Category)
            .Include(c => c.Item).ThenInclude(i => i.Photos)
            .OrderBy(c => c.Status == ClaimStatus.Pending ? 0 : 1)
            .ThenByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public Task<int> CountIncomingPendingAsync(string ownerId, CancellationToken cancellationToken = default) =>
        context.Claims.CountAsync(c => c.Item.OwnerId == ownerId && c.Status == ClaimStatus.Pending, cancellationToken);

    public Task<int> CountActiveForItemAsync(int itemId, CancellationToken cancellationToken = default) =>
        context.Claims.CountAsync(c => c.ItemId == itemId && c.Status != ClaimStatus.Cancelled, cancellationToken);

    public async Task<ItemChangeResult> WithdrawAsync(int claimId, string claimantId, CancellationToken cancellationToken = default)
    {
        var claim = await context.Claims.FirstOrDefaultAsync(c => c.Id == claimId && c.ClaimantId == claimantId, cancellationToken);
        if (claim is null) return ItemChangeResult.NotFound;
        if (claim.Status != ClaimStatus.Pending) return ItemChangeResult.Conflict;

        claim.Status = ClaimStatus.Cancelled;
        claim.DecidedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return ItemChangeResult.Success;
    }
}
