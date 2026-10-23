# Kampüs Kayıp Eşya Platformu

Kampüste kaybolan ve bulunan eşyaları tek bir portalda toplayan ve eşyanın sahibine güvenli şekilde ulaşmasını sağlayan web sitesi.

**Ders:** BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları · 2026–2027 Güz
**Grup:** KampusKayipEsya · Şeyma Kula (23291513) · Barış Coşkun (23291721)

## Proje özeti

- Öğrenci ve personel okul e-postasıyla kayıt olur, fotoğraflı **"Kaybettim" / "Buldum"** ilanı açar, ilanları kategori, bina ve tarihe göre arar.
- Bulan kişi ilana bir **doğrulama sorusu** ekler; sahibi olduğunu düşünen kişi soruyu cevaplayıp **"Bu benim"** talebi gönderir.
- Talep onaylanınca taraflar **site içinden** anlık mesajlaşır; telefon ve e-posta paylaşılmaz.
- **Görevli** ilanları onaylar, büroya bırakılan eşyaları ve teslimleri kaydeder; **Admin** kullanıcı, rol, kategori ve konumları yönetir, istatistikleri görür.
- Web API'si (Swagger) iOS Development dersindeki Flutter uygulaması tarafından da kullanılır.

## Teknolojiler

ASP.NET Core MVC (.NET 10) · MS SQL Server · Entity Framework Core (migration) · ASP.NET Core Identity · JWT · SignalR · Swagger · Bootstrap 5 · Chart.js · xUnit

## Depo yapısı

```
├── src/KampusKayipEsya.Web/   MVC + API projesi
│   ├── Areas/Admin/  Areas/Gorevli/
│   ├── Controllers/  Controllers/Api/
│   ├── Data/                  AppDbContext, Migrations/
│   ├── Models/  ViewModels/  Dtos/
│   ├── Services/  Hubs/  BackgroundJobs/
│   └── wwwroot/lib/           istemci kütüphaneleri
├── global.json                .NET SDK sürümü
└── KampusKayipEsya.slnx
```

## Kurulum

**Gereksinimler**

- .NET 10 SDK
- SQL Server: Windows'ta LocalDB (Visual Studio ile gelir) · Mac'te Docker
- EF Core aracı: `dotnet tool install --global dotnet-ef`

**1. Depoyu klonlayın**

```powershell
git clone https://github.com/Bariscompeng/kampus-kayip-esya-web.git
cd kampus-kayip-esya-web/src/KampusKayipEsya.Web
```

**2. Bağlantı dizesini user-secrets'a girin** (depoya girmez; `appsettings.json`'daki değer boştur)

Windows / LocalDB:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=KampusKayipEsya;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Mac / Docker (`<SIFRE>` en az 8 karakter; büyük/küçük harf, rakam ve sembol):

```bash
docker run -d --name kampus-sql -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='<SIFRE>' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=KampusKayipEsya;User Id=sa;Password=<SIFRE>;TrustServerCertificate=True"
```

**3. Veritabanını migration ile oluşturun ve çalıştırın**

```powershell
dotnet build
dotnet ef database update
dotnet run --launch-profile http
```

Tarayıcıda http://localhost:5227 açılır.

**Migration'lar**

| Migration | İçerik |
|---|---|
| `Initial` | Identity tabloları (AspNetUsers…), Categories, Locations, Items ve ilişkileri |

## Lisanslar

- [Bootstrap](https://getbootstrap.com/) 5.3 — MIT
- [jQuery](https://jquery.com/), [jQuery Validation](https://jqueryvalidation.org/) — MIT
