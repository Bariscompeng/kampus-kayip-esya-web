# Proje Kararları

Tarih: 2026-10-24 · Durum: Şeyma ve Barış onayına sunuldu

Bu dosyadaki kararlar iki üyenin birlikte onayıyla değişir.

## İş kuralları

| Karar | Öneri | Onay |
|---|---|---|
| Talebi kim onaylar? | `IsAtOffice` ise **görevli**, değilse **ilan sahibi** | ☐ Şeyma ☐ Barış |
| "Kayıp" ilanında talep? | `Claim.Kind = IFoundIt` → buton "Ben buldum", doğrulama cevabı isteğe bağlı | ☐ Şeyma ☐ Barış |
| Aynı ilana birden çok talep? | Serbest; teslim kaydı yapılınca diğerleri otomatik reddedilir | ☐ Şeyma ☐ Barış |
| Mükerrer talep | `(ItemId, ClaimantId)` benzersiz indeks | ☐ Şeyma ☐ Barış |
| Pasif kullanıcı | Silinmez, `LockoutEnd` ile kilitlenir; API'den 401 alır | ☐ Şeyma ☐ Barış |

## İlan ve talep durumları

```
Pending ──görevli onaylar──▶ Published ──sahibi/görevli teslim──▶ Resolved
   │                            │
   └──görevli reddeder──▶ Rejected   └──30 gün işlem yok──▶ Archived
```

- Liste ve aramada yalnızca `Published` ilanlar görünür.
- Görevlinin büroya kaydettiği ilan doğrudan `Published` açılır.
- Talep: `Pending → Approved` veya `Pending → Rejected`. Onaylı talepte teslim yapılınca `DeliveredAt` dolar, ilan `Resolved` olur.

## Teknik kararlar (1. hafta)

| Karar | Gerekçe | Onay |
|---|---|---|
| .NET 10, `global.json` ile `10.0.100` + `latestFeature` | Aynı ana sürümde derlenir; yama farkı sorun çıkarmaz | ☐ Şeyma ☐ Barış |
| Bağlantı dizesi `ConnectionStrings:DefaultConnection`, yalnızca user-secrets'ta | Gizli bilgi depoya girmez | ☐ Şeyma ☐ Barış |
| `ItemType` / `ItemStatus` veritabanında metin olarak tutulur | Tablolar okunabilir; enum sırası değişse de veri bozulmaz | ☐ Şeyma ☐ Barış |
| `Item` üzerindeki tüm yabancı anahtarlar `Restrict` | SQL Server çoklu cascade hatası önlenir; ilana bağlı kategori/konum silinemez | ☐ Şeyma ☐ Barış |
| Kategori ve konum başlangıç verisi `HasData` ile, ayrı `SeedLookups` migration'ında; kategori Id'leri 1–7 sabit | Temiz klonda `dotnet ef database update` yeterli; iOS aynı Id'lere güvenebilir | ☐ Şeyma ☐ Barış |
| `Category.Icon` Bootstrap Icons adı tutar | Web ve iOS aynı ikon adını API'den okur | ☐ Şeyma ☐ Barış |
| İstemci kütüphaneleri LibMan ile `wwwroot/lib`'e indirilir, CDN kullanılmaz | Gösterimde internet olmasa da sayfa bozulmaz | ☐ Şeyma ☐ Barış |
| Yönetici paneli şablonu: Start Bootstrap SB Admin 7.0.7 (MIT) | Bootstrap 5 tabanlı, ücretsiz | ☐ Şeyma ☐ Barış |
