# 1. Gösterim — 18 Kasım 2026

Şeyma sunar, Barış ekranı kullanır. Süre ~5 dakika.

## Gösterimden önce

- [ ] `main` temiz klonla README'deki adımlarla derleniyor ve açılıyor
- [ ] user-secrets girildi: bağlantı dizesi, `Jwt:Key`, `Seed:Admin:Password`, `Seed:Gorevli:Password`, `DemoData:Password`
- [ ] Veritabanı güncel: `dotnet ef database update` (son migration `ItemRejectionReason`)
- [ ] Demo verisi yüklendi: `dotnet run --launch-profile http -- --DemoData:Seed=true` (README → Demo verisi)
- [ ] Kayıt adımında kullanılacak e-posta daha önce kayıtlı değil (ör. `gosterim1@ogrenci.ankara.edu.tr`)
- [ ] İki tarayıcı profili açık: **Öğrenci** (demo hesabı) ve **Görevli** (`gorevli@ankara.edu.tr`)
- [ ] İnternet kapalıyken sayfalar düzgün açılıyor (CSS/JS `wwwroot/lib`'de, CDN yok)
- [ ] Swagger açılıyor: http://localhost:5227/swagger
- [ ] Telefonda (ya da tarayıcının mobil görünümünde, 375 px) giriş, kayıt, ilanlar ve onay kuyruğu sayfalarına bakıldı
- [ ] Laptop şarjı, HDMI/USB-C adaptörü
- [ ] Yedek video hazır

## Akış

1. **Kayıt ve giriş**
   - `deneme@gmail.com` ile kayıt → "Yalnızca okul e-postasıyla kayıt olabilirsiniz" hatası.
   - Okul e-postasıyla kayıt → otomatik giriş.
2. **Fotoğraflı "Buldum" ilanı**
   - İlan Aç → tür "Buldum", doğrulama sorusu, 1–2 fotoğraf.
   - Detay sayfasında "Onay bekliyor" uyarısı. İlan listede henüz görünmez.
3. **Görevli: onay kuyruğu**
   - Görevli profilinde menüdeki sarı rozet → Onay Kuyruğu.
   - Yeni ilanı **Onayla**.
   - Demo verisindeki bekleyen ilanlardan birini sebep yazarak **Reddet**.
   - Öğrenci profilinde Profilim → Kapanmış: ret sebebi görünür.
4. **Arama, filtre, sayfalama**
   - İlanlar → metin araması ("cüzdan").
   - Tür, kategori ve bina filtreleri; adres çubuğunda filtreler görünür.
   - Sayfa 2'ye geçince filtreler korunur.
5. **Swagger**
   - `POST /api/auth/login` → token → **Authorize**.
   - `GET /api/items` (filtreli), `POST /api/items` → 201 `Pending`, `PUT`, `DELETE`.
6. **Mobil görünüm**
   - Tarayıcıyı daraltın ya da telefondan açın.
   - İlanlar sayfasında filtre paneli düğmeyle açılır; Profilim sekmeleri ve onay kuyruğu tek sütun.

## Hafta 4 videosu (Şeyma çeker, ≥ 5 dk)

Görevli girişi → onay kuyruğu → onay/ret → öğrencinin ilanlarında durum değişimi → düzenle/sil/teslim edildi → mobil görünüm. Dosya adı: `Hafta04_KampusKayipEsya.mp4`, Pazar 23:59'a kadar Drive'a.
