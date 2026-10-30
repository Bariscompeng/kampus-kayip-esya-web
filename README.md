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
│   ├── Data/                  AppDbContext, SeedData, Migrations/
│   ├── Models/  ViewModels/  Dtos/
│   ├── Services/  Hubs/  BackgroundJobs/
│   └── wwwroot/lib/           istemci kütüphaneleri (LibMan, CDN yok)
├── docs/                      API.md · KARARLAR.md
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

**3. Admin ve test görevlisi şifrelerini girin** (uygulama açılışında roller ve bu iki hesap otomatik oluşur; şifre girilmezse hesap atlanır)

```powershell
dotnet user-secrets set "Seed:Admin:Password" "<ADMIN_SIFRESI>"
dotnet user-secrets set "Seed:Gorevli:Password" "<GOREVLI_SIFRESI>"
```

E-posta adresleri `appsettings.json`'da: `admin@ankara.edu.tr`, `gorevli@ankara.edu.tr`. Şifreler en az 8 karakter; büyük/küçük harf, rakam ve sembol içermeli.

**4. Veritabanını migration ile oluşturun ve çalıştırın**

```powershell
dotnet build
dotnet ef database update
dotnet run --launch-profile http
```

Tarayıcıda http://localhost:5227 açılır; ana sayfada 7 kategori kartı görünür.

**Roller ve kayıt**

- Kayıt yalnızca `appsettings.json` → `AllowedEmailDomains` listesindeki alan adlarıyla (alt alan adları dahil, ör. `ogrenci.ankara.edu.tr`) yapılır.
- Yeni kayıt otomatik olarak `Ogrenci` rolünü alır. `Gorevli` ve `Admin` rollerini yalnızca admin verir.
- `/Admin` yalnızca `Admin`; `/Gorevli` ise `Gorevli` ve `Admin` rolüne açıktır.

**Yönetim paneli**

- `admin@ankara.edu.tr` ile giriş yapınca menüde **Yönetim** çıkar (`/Admin`). Panel SB Admin şablonuyla, `Views/Shared/_AdminLayout.cshtml`.
- **Kategoriler** (`/Admin/Categories`) ve **Konumlar** (`/Admin/Locations`): ekleme, düzenleme, silme. İlana bağlı bir kategori ya da konum silinemez.
- Koordinatlar isteğe bağlıdır; virgül ya da nokta ile girilebilir (ör. `39,9334`).

**Migration'lar**

| Migration | İçerik |
|---|---|
| `Initial` | Identity tabloları (AspNetUsers…), Categories, Locations, Items ve ilişkileri |
| `SeedLookups` | 7 kategori (Bootstrap ikon adlarıyla) ve 8 kampüs konumu |

## Lisanslar

- [Bootstrap](https://getbootstrap.com/) 5.3 — MIT
- [Bootstrap Icons](https://icons.getbootstrap.com/) 1.11.3 — MIT
- [Start Bootstrap SB Admin](https://startbootstrap.com/template/sb-admin) 7.0.7 — MIT (`wwwroot/lib/sb-admin/LICENSE`)
- [jQuery](https://jquery.com/), [jQuery Validation](https://jqueryvalidation.org/) — MIT
