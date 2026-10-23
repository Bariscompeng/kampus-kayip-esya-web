using KampusKayipEsya.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Data;

public static class SeedData
{
    public static void Apply(ModelBuilder builder)
    {
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Elektronik", Icon = "bi-phone" },
            new Category { Id = 2, Name = "Kimlik/Kart", Icon = "bi-person-vcard" },
            new Category { Id = 3, Name = "Cüzdan", Icon = "bi-wallet2" },
            new Category { Id = 4, Name = "Anahtar", Icon = "bi-key" },
            new Category { Id = 5, Name = "Giyim", Icon = "bi-bag" },
            new Category { Id = 6, Name = "Kitap", Icon = "bi-book" },
            new Category { Id = 7, Name = "Diğer", Icon = "bi-three-dots" }
        );

        builder.Entity<Location>().HasData(
            new Location { Id = 1, Building = "Mühendislik Fakültesi", FloorOrRoom = "Zemin kat" },
            new Location { Id = 2, Building = "Mühendislik Fakültesi", FloorOrRoom = "1. kat" },
            new Location { Id = 3, Building = "Merkezi Derslik", FloorOrRoom = "Amfi" },
            new Location { Id = 4, Building = "Kütüphane" },
            new Location { Id = 5, Building = "Yemekhane" },
            new Location { Id = 6, Building = "Kantin" },
            new Location { Id = 7, Building = "Spor Salonu" },
            new Location { Id = 8, Building = "Güvenlik / Kayıp Eşya Bürosu" }
        );
    }
}
