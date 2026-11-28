using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.Services;

public enum ClaimCreateResult
{
    Success,
    NotFound,
    OwnItem,
    AlreadyClaimed
}

public record ClaimInput(string VerificationAnswer, string? Message);

public record ClaimCreateOutcome(ClaimCreateResult Result, ItemClaim? ItemClaim);

public interface IClaimService
{
    Task<Item?> GetClaimableItemAsync(int itemId, CancellationToken cancellationToken = default);

    Task<ClaimCreateOutcome> CreateAsync(int itemId, string claimantId, ClaimInput input, CancellationToken cancellationToken = default);

    Task<ItemClaim?> GetActiveClaimAsync(int itemId, string claimantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemClaim>> GetMineAsync(string claimantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemClaim>> GetIncomingAsync(string ownerId, CancellationToken cancellationToken = default);

    Task<int> CountIncomingPendingAsync(string ownerId, CancellationToken cancellationToken = default);

    Task<int> CountActiveForItemAsync(int itemId, CancellationToken cancellationToken = default);

    Task<ItemChangeResult> WithdrawAsync(int claimId, string claimantId, CancellationToken cancellationToken = default);
}
