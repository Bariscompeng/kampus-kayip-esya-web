using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Data;

public static class DemoDataSeeder
{
    private record DemoKullanici(string Email, string FullName, string StudentNo);

    private record DemoIlan(ItemType Type, string Title, string Description, string Category, string Building, string? FloorOrRoom,
        int GunOnce, ItemStatus Status, int Sahip, string? Soru = null);

    private static readonly DemoKullanici[] Kullanicilar =
    [
        new("ayse.yilmaz@ogrenci.ankara.edu.tr", "Ayşe Yılmaz", "23290001"),
        new("mehmet.kaya@ogrenci.ankara.edu.tr", "Mehmet Kaya", "23290002"),
        new("zeynep.demir@ogrenci.ankara.edu.tr", "Zeynep Demir", "23290003"),
        new("can.ozturk@ogrenci.ankara.edu.tr", "Can Öztürk", "23290004")
    ];

    private static readonly DemoIlan[] Ilanlar =
    [
        new(ItemType.Found, "Siyah deri cüzdan", "Amfinin arka sıralarında yerde buldum. İçinde öğrenci kartı ve banka kartları var, başka bir şeye dokunmadım.",
            "Cüzdan", "Merkezi Derslik", "Amfi", 1, ItemStatus.Published, 0, "Cüzdandaki banka kartı hangi bankaya ait?"),
        new(ItemType.Found, "Beyaz kablosuz kulaklık kutusu", "Kütüphane ikinci katta, pencere kenarındaki masada şarj kutusu kalmış. Kutunun içinde iki kulaklık da var.",
            "Elektronik", "Kütüphane", null, 2, ItemStatus.Published, 1, "Kutunun arkasında ne tür bir çıkartma var?"),
        new(ItemType.Found, "Kırmızı deri anahtarlık", "Yemekhane çıkışındaki bankta buldum. Anahtarlıkta birkaç anahtar ve küçük bir el feneri var.",
            "Anahtar", "Yemekhane", null, 3, ItemStatus.Published, 2, "Anahtarlıkta kaç anahtar var?"),
        new(ItemType.Found, "Öğrenci kimlik kartı", "Kantinde kasa önünde yerde duruyordu. Kartın sahibi ilanı görünce büroya ya da bana ulaşabilir.",
            "Kimlik/Kart", "Kantin", null, 1, ItemStatus.Published, 3, "Kartın üzerindeki öğrenci numarasının son dört hanesi nedir?"),
        new(ItemType.Found, "Lacivert şişme mont", "Spor salonunun soyunma odasında askıda unutulmuş. Erkek modeli, L beden.",
            "Giyim", "Spor Salonu", null, 4, ItemStatus.Published, 0, "Montun iç cebinde ne vardı?"),
        new(ItemType.Found, "Lineer Cebir ders kitabı", "Mühendislik birinci kattaki dersliklerden birinde sırada kalmış. Kenarlarında kurşun kalem notlar var.",
            "Kitap", "Mühendislik Fakültesi", "1. kat", 0, ItemStatus.Pending, 1, "Kitabın ilk sayfasında hangi isim yazıyor?"),
        new(ItemType.Found, "Siyah katlanır şemsiye", "Güvenlik kulübesinin önünde unutulmuştu, kayıp eşya bürosuna bıraktım.",
            "Diğer", "Güvenlik / Kayıp Eşya Bürosu", null, 6, ItemStatus.Published, 2, "Şemsiyenin sapı ne renk?"),
        new(ItemType.Found, "Gri laptop şarj aleti", "Mühendislik zemin kattaki çalışma alanında prizde takılı kalmış.",
            "Elektronik", "Mühendislik Fakültesi", "Zemin kat", 2, ItemStatus.Published, 3, "Şarj aletinin markası ve gücü (watt) nedir?"),
        new(ItemType.Found, "Yeşil çelik termos", "Kütüphane girişindeki dolapların üstünde buldum.",
            "Diğer", "Kütüphane", null, 9, ItemStatus.Resolved, 0, "Termosun üzerinde hangi yazı var?"),
        new(ItemType.Lost, "Siyah kablosuz kulaklık", "Kantinde öğle arasında masada unuttum. Kulaklıklar siyah, kutusunda küçük bir çizik var.",
            "Elektronik", "Kantin", null, 1, ItemStatus.Published, 1),
        new(ItemType.Lost, "Kahverengi çerçeveli gözlük", "Merkezi derslikteki amfide ders sonrası kılıfıyla birlikte kaybettim. Kılıf lacivert.",
            "Diğer", "Merkezi Derslik", "Amfi", 3, ItemStatus.Published, 2),
        new(ItemType.Lost, "Bilimsel hesap makinesi", "Mühendislik birinci kattaki sınavdan sonra sırada unutmuşum. Arkasında adımın baş harfleri yazılı.",
            "Elektronik", "Mühendislik Fakültesi", "1. kat", 2, ItemStatus.Published, 3),
        new(ItemType.Lost, "Gri sırt çantası", "Spor salonunun girişinde bıraktığım çanta kayboldu. İçinde spor kıyafetleri ve bir su şişesi var.",
            "Diğer", "Spor Salonu", null, 5, ItemStatus.Published, 0),
        new(ItemType.Lost, "32 GB USB bellek", "Kütüphanedeki bilgisayarlardan birinde takılı unuttum. Mavi kapaklı, içinde bitirme projem var.",
            "Elektronik", "Kütüphane", null, 0, ItemStatus.Pending, 1),
        new(ItemType.Lost, "Banka kartı", "Yemekhanede ödeme yaptıktan sonra kartımı kaybettim. Kartı iptal ettirdim ama bulan olursa haber versin.",
            "Kimlik/Kart", "Yemekhane", null, 2, ItemStatus.Published, 2),
        new(ItemType.Lost, "Bordo yün atkı", "Yemekhaneden çıkarken düşürmüş olabilirim. El örgüsü, uçlarında püskül var.",
            "Giyim", "Yemekhane", null, 1, ItemStatus.Pending, 3),
        new(ItemType.Lost, "Algoritmalar ders defteri", "Merkezi derslikte kalmış olmalı. Mavi spiralli defter, kapağında adım yazıyor.",
            "Kitap", "Merkezi Derslik", "Amfi", 4, ItemStatus.Published, 0),
        new(ItemType.Lost, "Bere", "Bere.", "Giyim", "Kantin", null, 7, ItemStatus.Rejected, 1)
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var photoService = scope.ServiceProvider.GetRequiredService<IPhotoService>();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DemoDataSeeder));

        var sifre = config["DemoData:Password"];
        if (string.IsNullOrWhiteSpace(sifre))
        {
            log.LogWarning("Demo verisi yüklenmedi: user-secrets'ta DemoData:Password girilmeli.");
            return;
        }

        if (await userManager.FindByEmailAsync(Kullanicilar[0].Email) is not null)
        {
            log.LogInformation("Demo verisi zaten yüklü; tekrar eklenmedi.");
            return;
        }

        var sahipler = new List<AppUser>();
        foreach (var k in Kullanicilar)
        {
            var kullanici = new AppUser { UserName = k.Email, Email = k.Email, EmailConfirmed = true, FullName = k.FullName, StudentNo = k.StudentNo };
            var sonuc = await userManager.CreateAsync(kullanici, sifre);
            if (!sonuc.Succeeded)
            {
                log.LogError("Demo kullanıcısı oluşturulamadı ({Email}): {Hatalar}", k.Email,
                    string.Join(" ", sonuc.Errors.Select(e => e.Description)));
                return;
            }
            await userManager.AddToRoleAsync(kullanici, Roles.Ogrenci);
            sahipler.Add(kullanici);
        }

        var onaylayan = (await userManager.GetUsersInRoleAsync(Roles.Gorevli)).FirstOrDefault()
            ?? (await userManager.GetUsersInRoleAsync(Roles.Admin)).FirstOrDefault();
        var kategoriler = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
        var konumlar = await context.Locations.ToListAsync();

        var eklenenler = new List<Item>();
        for (var i = 0; i < Ilanlar.Length; i++)
        {
            var d = Ilanlar[i];
            var konum = konumlar.FirstOrDefault(l => l.Building == d.Building && l.FloorOrRoom == d.FloorOrRoom);
            if (!kategoriler.TryGetValue(d.Category, out var kategoriId) || konum is null)
            {
                log.LogWarning("Demo ilanı atlandı, kategori ya da konum yok: {Baslik}", d.Title);
                continue;
            }

            var onayli = d.Status is ItemStatus.Published or ItemStatus.Resolved;
            var olusturma = DateTime.UtcNow.AddDays(-d.GunOnce).AddHours(-i);
            var ilan = new Item
            {
                Type = d.Type,
                Title = d.Title,
                Description = d.Description,
                EventDate = DateTime.Today.AddDays(-d.GunOnce),
                Status = d.Status,
                VerificationQuestion = d.Type == ItemType.Found ? d.Soru : null,
                CategoryId = kategoriId,
                LocationId = konum.Id,
                OwnerId = sahipler[d.Sahip].Id,
                CreatedAt = olusturma,
                ApprovedById = onayli ? onaylayan?.Id : null,
                ApprovedAt = onayli && onaylayan is not null ? olusturma.AddHours(2) : null
            };
            context.Items.Add(ilan);
            eklenenler.Add(ilan);
        }
        await context.SaveChangesAsync();

        var fotoSayisi = await FotograflariEkle(config["DemoData:PhotoFolder"], eklenenler, photoService, log);
        log.LogInformation("Demo verisi yüklendi: {Kullanici} kullanıcı, {Ilan} ilan, {Foto} fotoğraf.", sahipler.Count, eklenenler.Count, fotoSayisi);
    }

    private static async Task<int> FotograflariEkle(string? klasor, List<Item> ilanlar, IPhotoService photoService, ILogger log)
    {
        if (string.IsNullOrWhiteSpace(klasor) || !Directory.Exists(klasor)) return 0;

        var dosyalar = Directory.GetFiles(klasor)
            .Where(f => Path.GetExtension(f).ToLowerInvariant() is ".jpg" or ".jpeg" or ".png")
            .OrderBy(f => f)
            .ToList();
        if (dosyalar.Count == 0) return 0;

        var eklenen = 0;
        for (var i = 0; i < ilanlar.Count; i++)
        {
            var yol = dosyalar[i % dosyalar.Count];
            var icerik = await File.ReadAllBytesAsync(yol);
            using var akis = new MemoryStream(icerik);
            var dosya = new FormFile(akis, 0, icerik.Length, "files", Path.GetFileName(yol))
            {
                Headers = new HeaderDictionary(),
                ContentType = Path.GetExtension(yol).ToLowerInvariant() == ".png" ? "image/png" : "image/jpeg"
            };

            var hatalar = photoService.Validate([dosya]);
            if (hatalar.Count > 0)
            {
                log.LogWarning("Demo fotoğrafı atlandı: {Hata}", hatalar[0]);
                continue;
            }

            await photoService.SaveAsync(ilanlar[i].Id, [dosya]);
            eklenen++;
        }
        return eklenen;
    }
}
