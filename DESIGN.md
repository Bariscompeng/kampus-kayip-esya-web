# DESIGN.md — Kampüs Kayıp Eşya

Web arayüzünün tasarım kararları. Kod (`src/KampusKayipEsya.Web/wwwroot/css/site.css`) bu dosyadan sapıyorsa bu dosya esas alınır.

## 1. Bağlam

- **Ürün:** Kampüste kaybolan ve bulunan eşyaların ilan sitesi. Öğrenci tarafı (arama, ilan açma, ilanlarım) ve görevli/yönetim paneli (onay kuyruğu, kategori, konum).
- **Kullanıcı:** Telefonundan hızlıca bakan öğrenci; masada çalışan kayıp eşya bürosu görevlisi.
- **Tür ve mod:** Uygulama arayüzü (Operate). Ana sayfa arama odaklı, pazarlama sayfası değil.
- **Sıfatlar:** güvenilir, sakin, hızlı taranır, kurumsal ama sıcak.
- **Öz:** "büronun bagaj etiketi".

## 2. Görsel dünya ve imza

Kayıp eşya bürosunda eşyaya takılan **delikli askı etiketi**. Lacivert zemin, sinyal turuncusu eylem rengi.

**İmza hareketi:** ilan türü rozetleri (`.tag`) ucu kesik, delikli etiket biçiminde. Aynı biçim logo işaretinde (`.brand-mark`), 404/hata kodlarında (`.status-code`), ana sayfadaki arama panelinin kendisinde (sivri uçlu, delikli büyük etiket) ve fotoğrafsız ilanların yer tutucusunda (türüne göre yeşil/kırmızı, hafif eğik asılı etiket) tekrar eder. Başka yerde süs kullanılmaz.

## 3. Yazı

| Rol | Yazı tipi | Not |
|---|---|---|
| Başlık (h1–h3) | Bricolage Grotesque 600–700 | Yerel WOFF2, `wwwroot/lib/fonts` |
| Gövde, arayüz, buton, tablo | Onest 400–700 | Yerel WOFF2, latin + latin-ext (Türkçe) |

Ölçek 1.2 oranlı, taban 16px: 12 / 14 / 16 / 19 / 23 / 28 / 40. Etiket ve buton yazılarında başlık fontu kullanılmaz. Sayılar `tabular-nums`.

## 4. Renk (koyu tema, 2026-11-23)

Nötr grafit zemin (lacivert yok), açık metin, tek parlak yüzey olarak turuncu etiket. Yükseklik gölgeyle değil, yüzeyin açılmasıyla anlatılır. Bootstrap `data-bs-theme="dark"` açık; yönetim paneli (SB Admin, Bootstrap 5.2) için eksikler `site.css` sonunda elle verilir.

| Token | Değer | Rol | Kontrast |
|---|---|---|---|
| `--paper` | #121212 | Sayfa zemini, form içi | |
| `--surface` | #1c1c1e | Kart, panel | |
| `--surface-2` | #26262a | Yükseltilmiş alan, hover | |
| `--nav-bg` | #18181a | Üst menü | |
| `--line` / `--field-line` | #2e2e32 / #6e6e75 | Ayırıcı / form kenarı | form kenarı 3.4:1 |
| `--ink` | #ececec | Ana metin | kağıtta 15.9:1 |
| `--muted` | #a9a9ae | İkincil metin | yüzeyde 7.3:1 |
| `--accent` | #ff7a2f | Birincil eylem, seçili çizgi, hero etiketi | |
| `--on-accent` | #141414 | Turuncu üzerinde metin | 7.1:1 |
| `--link` / `--ring` | #8cc0ff | Bağlantı, odak halkası | yüzeyde 9:1 |
| `--strong-bg` / `--strong-fg` | #ececec / #121212 | Seçili sayfa no, sekme sayacı, buton dolgusu | 15.8:1 |
| `--lost` / `--lost-fg` | #c4372f / #ff8f85 | Kayıp dolgusu (beyaz yazı 5.3:1) / kayıp metni | metin 7.7:1 |
| `--found` / `--found-fg` | #1b7a4b / #5fd39a | Bulundu dolgusu (5.3:1) / bulundu metni | metin 9.2:1 |
| `--pending` | #ffc861 | Onay bekliyor | tint üzerinde 8.9:1 |

Kurallar: kırmızı ve yeşil dolgu olarak ve metin olarak ayrı tonlarla kullanılır (koyu zeminde dolgu tonu metin olarak okunmaz). Turuncu üzerinde beyaz metin yok. Renk tek başına anlam taşımaz. Mor/indigo ve lacivert kullanılmaz (kullanıcı tercihi, 2026-11-23); bağlantı rengi açık gök mavisi.

## 5. Biçim, boşluk, derinlik, hareket

- Köşe: 6px (buton, form, çip içi), 10px (kart, panel). Rozet ve sayaçlar tam yuvarlak.
- Derinlik: **çizgi** (1px `--line`). Gölge yalnızca bağlantılı kartın hover'ında (`--shadow-lift`) birlikte 2px yükselme ile.
- Boşluk tabanı 4px; grup içi sıkı, bölümler arası 2.5–3rem.
- Hareket: 120–380ms, `cubic-bezier(0.22, 1, 0.36, 1)`, yalnızca transform / opacity / renk. `prefers-reduced-motion` ile hepsi durur, renk ve çizgi değişimi kalır.
- **Hareket fiili: "askıdaki etiket gibi sallanır."** Logo etiketi, kart üzerindeki tür etiketi ve fotoğrafsız ilanların etiket yer tutucusu hover/odakta deliklerinin etrafında kısa bir salınım yapar (`tag-swing`).
- Rol başına tek tepki: birincil ve ikincil butonlar alttan dolguyla dolar (lacivert zeminde beyaz dolgu); kartlar 2px yükselir ve etiketi sallanır; ok içeren bağlantıların altı soldan çizilir, oku gittiği yöne 5px kayar; yardımcı bağlantılar (Gizlilik) yalnızca renk değiştirir.
- Sayfa geçişi: yok (belgeler arası View Transition Chrome'da zaman zaman yarıda kesilip konsola hata düşürdüğü için 2026-11-24'te kaldırıldı). Tema düğmesindeki daire geçişi ayrı ve korunuyor.
- **Durgun sayfa kararı:** bu bir uygulama; kullanıcı dokunmadığında hiçbir şey kıpırdamaz. Canlılık hover, odak ve geçişlerde.

## 6. Bileşenler

- **Buton sırası:** birincil = turuncu (`.btn-primary`), ikincil = lacivert çerçeve (`.btn-outline-primary`), üçüncül = sessiz (`.btn-outline-secondary`). Bir ekranda tek birincil eylem. Yıkıcı işlem çerçeveli kırmızı + onay penceresi.
- **Buton durumları:** hover, active (1px), focus-visible halkası, disabled, gönderimde meşgul (`site.js`: buton kilitlenir, metne "…" eklenir; istemci doğrulaması durdurursa açık kalır).
- **Rozetler:** `.tag` (tür), `.status` (durum), `.chip` (kategori), `.count-pill` (bekleyen sayısı).
- **Formlar:** görünür etiket, 44px yükseklik, hata alanın altında, gömük bölüm için `.field-panel`.
- **Boş durum:** `.empty-state` — ne olduğunu söyler, sonraki adımı (buton) verir.
- **Durum sayfaları:** 403, 404, 500 Türkçe; `/Home/Durum/{kod}` (API yolları hariç).

## 7. Erişilebilirlik

WCAG 2.2 AA hedefi: "İçeriğe geç" bağlantısı (her iki şablonda), menüde `aria-current="page"`, tek tip `:focus-visible` halkası (lacivert zeminde turuncu), süs ikonlarında `aria-hidden`, yalnızca ikon kalan butonlarda `aria-label`, dokunma hedefleri ≥ 44px (küçük butonlar 36px), tüm metin kontrastları ≥ 4.5:1.

## 8. Yönetim paneli

SB Admin iskeleti korunur, renkleri token'lardan gelir. Pano kutuları tek nötr yüzey; yalnızca onay bekleyen ilan varsa kutu turuncuya döner (dikkat gerektiren tek sayı). "Yakında" bölümleri soluk ve etiketli.

## 9. Slop denetimi (2026-11-22)

Geçti: mor/gradyan yok, gradyan metin yok, kart içinde kart yok, tek yanlı renk çubuğu yok, çizgi + yayvan gölge aynı anda yok (hover hariç), Inter/Roboto yok, ikon-kutusu-başlık kart şablonu yok, tek birincil eylem, hareket azaltma var.
Bilinçli istisna: ilan listesinde filtre sütunu sonuç ızgarasından kısa (liste sayfasının doğası; impeccable `first-viewport-column-overflow` uyarısı).

## 10. SEO ve paylaşım

Her sayfada kendi açıklaması, canonical, Open Graph ve Twitter etiketleri (`/img/og.png`, 1200×630), SVG favicon. `/robots.txt` ve yayındaki ilanlarla `/sitemap.xml` (HomeController). Giriş, kayıt, form, profil ve panel sayfaları `noindex`.

## 11. Tema düğmesi, butonlar, tipografi (2026-11-24)

- **Tema:** varsayılan koyu (grafit). Üst menüdeki yuvarlak düğme açık temaya geçirir; seçim `localStorage("tema")` içinde saklanır ve `<head>` içindeki küçük betikle ilk boyamadan önce uygulanır (yanıp sönme yok). Geçişte yeni tema düğmeden daire olarak yayılır (View Transitions); hareket azaltmada anında değişir. Yönetim paneli her zaman koyu.
- **Açık tema:** sıcak nötr (zemin #f4f3f1, yüzey #fff, metin #1c1a18 15.7:1, ikincil #5b5752 6.5:1, bağlantı #0b5cc4, turuncu metin #b84a0c). Lacivert yok. Her iki temada axe WCAG AA: 0 ihlal.
- **Butonlar:** katmanlı birincil buton (üstte ince ışık, altta yumuşak turuncu gölge), hover'da 1px yükselme, basınca %97'ye küçülme, tıklanan noktadan yayılan dalga (`.ripple`, site.js; klavyede ortadan). Hareket azaltmada dalga ve ölçek kapalı.
- **Tipografi:** başlıklar Bricolage Grotesque 800, sıkı harf aralığı (-0.03 / -0.045em hero'da); sayfa başlıkları 32–46px akışkan; kart ve satır başlıkları da başlık fontunda (700). Fontlar değişken (Onest 100–900, Bricolage 200–800).

## Değişiklik günlüğü

- 2026-11-28 — Talepler: profil menüsü (`.profile-nav`, seçili bağlantı `--strong-bg` dolgulu, telefonda tek satır ve ikonsuz), talep cevabı kutusu (`.claim-answer`), bölüm başlığında sayaç (`.section-title .tab-count`). Talep durumları mevcut `.status-*` rozetleriyle gösterilir.

- 2026-11-24 — Açık/koyu tema düğmesi (daire geçişli), açık tema paleti, katmanlı butonlar ve tıklama dalgası, daha güçlü başlık tipografisi.

- 2026-11-23 — Koyu tema lacivertten nötr grafite çevrildi (zemin #121212, yüzeyler #1c1c1e / #26262a); favicon ve paylaşım görseli güncellendi.
- 2026-11-23 — Koyu tema: gece lacivertî zemin, turuncu hero etiketi, rol bazlı token'lar (metin / yüzey / seçili / durum dolgusu ve metni), giriş-kayıt sayfalarına "Nasıl çalışır?" adımları, ana sayfa satırlarında mini etiketler.

- 2026-11-22 — Jury puanlamasına göre ikinci tur: SEO/meta, etiket biçimli hero ve yer tutucular, "sallanan etiket" hareket dili, dolgu hover'ları, sayfa geçişi, yapışkan menü, telefon için sıkı kartlar, gizlilik sayfası.

- 2026-11-22 — Bootstrap varsayılan görünümünden "bagaj etiketi" kimliğine geçiş; token sistemi, yerel fontlar, durum sayfaları, erişilebilirlik düzeltmeleri.
