using Microsoft.AspNetCore.Identity;

namespace KampusKayipEsya.Web.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? StudentNo { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // İlanlarda gösterilen kısaltılmış ad (ör. "Ayşe Y."); veritabanına yazılmaz.
    public string ShortName
    {
        get
        {
            var parcalar = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parcalar.Length switch
            {
                0 => "Kullanıcı",
                1 => parcalar[0],
                _ => $"{parcalar[0]} {char.ToUpper(parcalar[^1][0], new System.Globalization.CultureInfo("tr-TR"))}."
            };
        }
    }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
