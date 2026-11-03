using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Services;

public class PhotoService(AppDbContext context, IWebHostEnvironment environment) : IPhotoService
{
    public const int MaxPhotoCount = 5;
    public const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, byte[]> Imzalar = new()
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
    };

    private static readonly HashSet<string> IcerikTurleri = ["image/jpeg", "image/png"];

    public IReadOnlyList<string> Validate(IReadOnlyCollection<IFormFile> files, int existingCount = 0)
    {
        var hatalar = new List<string>();

        if (existingCount + files.Count > MaxPhotoCount)
            hatalar.Add($"Bir ilana en fazla {MaxPhotoCount} fotoğraf eklenebilir.");

        foreach (var dosya in files)
        {
            var uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();

            if (!Imzalar.ContainsKey(uzanti))
                hatalar.Add($"{dosya.FileName}: yalnızca JPG ve PNG yüklenebilir.");
            else if (dosya.Length == 0)
                hatalar.Add($"{dosya.FileName}: dosya boş.");
            else if (dosya.Length > MaxFileSize)
                hatalar.Add($"{dosya.FileName}: en fazla 5 MB olabilir.");
            else if (!IcerikTurleri.Contains(dosya.ContentType.ToLowerInvariant()) || !ImzaDogruMu(dosya, uzanti))
                hatalar.Add($"{dosya.FileName}: geçerli bir resim dosyası değil.");
        }

        return hatalar;
    }

    public async Task<List<ItemPhoto>> SaveAsync(int itemId, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var siradaki = await context.ItemPhotos
            .Where(p => p.ItemId == itemId)
            .Select(p => (int?)p.Order)
            .MaxAsync(cancellationToken) is int sonSira ? sonSira + 1 : 0;

        var goreliKlasor = Path.Combine("uploads", "items", itemId.ToString());
        var tamKlasor = Path.Combine(environment.WebRootPath, goreliKlasor);
        Directory.CreateDirectory(tamKlasor);

        var fotograflar = new List<ItemPhoto>();
        var yazilanDosyalar = new List<string>();

        try
        {
            foreach (var dosya in files)
            {
                var dosyaAdi = $"{Guid.NewGuid():N}{Path.GetExtension(dosya.FileName).ToLowerInvariant()}";
                var tamYol = Path.Combine(tamKlasor, dosyaAdi);

                await using (var hedef = File.Create(tamYol))
                    await dosya.CopyToAsync(hedef, cancellationToken);
                yazilanDosyalar.Add(tamYol);

                fotograflar.Add(new ItemPhoto
                {
                    ItemId = itemId,
                    Path = Path.Combine(goreliKlasor, dosyaAdi).Replace('\\', '/'),
                    Order = siradaki++
                });
            }

            context.ItemPhotos.AddRange(fotograflar);
            await context.SaveChangesAsync(cancellationToken);
            return fotograflar;
        }
        catch
        {
            foreach (var yol in yazilanDosyalar)
                File.Delete(yol);
            throw;
        }
    }

    private static bool ImzaDogruMu(IFormFile dosya, string uzanti)
    {
        var beklenen = Imzalar[uzanti];
        var okunan = new byte[beklenen.Length];

        using var akis = dosya.OpenReadStream();
        return akis.Read(okunan, 0, okunan.Length) == okunan.Length && okunan.SequenceEqual(beklenen);
    }
}
