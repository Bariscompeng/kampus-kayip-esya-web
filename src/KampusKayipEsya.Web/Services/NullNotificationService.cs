using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Services;

// Geçici uygulama: bildirimi yalnızca günlüğe yazar.
public class NullNotificationService(ILogger<NullNotificationService> logger) : INotificationService
{
    public Task CreateAsync(string userId, NotificationType type, string content, string? linkUrl, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Bildirim ({Type}) {UserId}: {Content} {LinkUrl}", type, userId, content, linkUrl);
        return Task.CompletedTask;
    }
}
