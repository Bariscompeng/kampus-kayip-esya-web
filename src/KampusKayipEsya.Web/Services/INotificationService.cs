using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Services;

// Bildirimler 6. haftada (Barış) Notifications tablosu ve SignalR ile gerçek uygulamaya kavuşacak.
// Servisler şimdiden bu arayüzü çağırır; uygulama değişince çağıran kod değişmez.
public interface INotificationService
{
    Task CreateAsync(string userId, NotificationType type, string content, string? linkUrl, CancellationToken cancellationToken = default);
}
