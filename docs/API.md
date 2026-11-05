# API Sözleşmesi

iOS projesi (BLM4587) bu uçları kullanır. Bir uç değişecekse önce diğer üyeye haber verilir.

## Genel kurallar

- Tüm uçlar `/api` altında.
- JSON alan adları `camelCase`.
- Kimlik doğrulama: `Authorization: Bearer <jwt>`.
- Liste yanıtı: `{ items, page, pageSize, totalCount }`.
- Hata yanıtı: `ProblemDetails`.
- Tarihler ISO 8601, UTC.
- Fotoğraf adresleri mutlak URL.
- Enum'lar metin olarak döner: `ItemType` = `Lost`, `Found` · `ItemStatus` = `Pending`, `Published`, `Rejected`, `Resolved`, `Archived`.
- Kategori `icon` alanı Bootstrap Icons adıdır (`bi-phone`, `bi-key`…).

## Uçlar

| Uç | Yöntem | Yazan | Web haftası | iOS ihtiyacı | Durum |
|---|---|---|---|---|---|
| `/api/auth/register` | POST | Şeyma | 3 (ek) | 3. hafta | ✅ |
| `/api/auth/login` | POST | Şeyma | 3 | 3. hafta | ✅ |
| `/api/auth/me` | GET | Şeyma | 3 (ek) | 3. hafta | ✅ |
| `/api/categories` | GET | Şeyma | 3 | 3. hafta | ✅ |
| `/api/locations` | GET | Şeyma | 3 (ek) | 3. hafta | ✅ |
| `/api/items` | GET (filtreli, sayfalı) | Şeyma | 3 | 3. hafta | ✅ |
| `/api/items/{id}` | GET | Şeyma | 3 | 4. hafta | ✅ |
| `/api/items` | POST | Şeyma | 3 | 5. hafta | ✅ |
| `/api/items/{id}` | PUT / DELETE | Şeyma | 3 | 6. hafta | ✅ |
| `/api/items/mine` | GET | Barış | 4 (ek) | 6. hafta | ⏳ |
| `/api/items/{id}/photos` | POST (multipart, `files`) | Barış | 4 (ek) | 5. hafta | ⏳ |
| `/api/items/{id}/claims` | POST | Şeyma | 5 | 6. hafta | ⏳ |
| `/api/claims/mine` · `/incoming` | GET | Şeyma | 5 | 6. hafta | ⏳ |
| `/api/claims/{id}/approve` · `/reject` | POST | Şeyma | 5 | 6. hafta | ⏳ |
| `/api/items/{id}/matches` | GET | Şeyma | 6 iskelet (ek) / 7 tam | 6. hafta | ⏳ |
| `/api/claims/{id}/messages` | GET (`?before=&take=`) | Barış | 6 (ek) | 7. hafta | ⏳ |
| `/api/claims/{id}/messages/read` | POST | Barış | 6 (ek) | 7. hafta | ⏳ |
| `/api/notifications` · `/unread-count` | GET | Barış | 6 (ek) | 7. hafta | ⏳ |
| `/api/notifications/{id}/read` · `/read-all` | POST | Barış | 6 (ek) | 7. hafta | ⏳ |
| `/hubs/chat` | SignalR | Barış | 6 | 7. hafta | ⏳ |

Durum: ⏳ yazılmadı · 🚧 yapılıyor · ✅ `main`'de, Swagger'da denendi

## Kimlik doğrulama ve ilan uçları

Token 24 saat geçerlidir (`Jwt:ExpiresMinutes`); yenileme ucu yok, süresi dolunca tekrar giriş yapılır. Kilitlenen kullanıcının eski token'ı da 401 alır.

| Uç | Giriş | Gövde | Başarılı yanıt |
|---|---|---|---|
| `POST /api/auth/register` | — | `{ fullName, studentNo?, email, password }` | 201 `{ token, expiresAt, user }` |
| `POST /api/auth/login` | — | `{ email, password }` | 200 `{ token, expiresAt, user }` |
| `GET /api/auth/me` | gerekli | — | 200 `{ id, email, fullName, studentNo, roles }` |
| `GET /api/categories` | — | — | 200 `[{ id, name, icon }]` |
| `GET /api/locations` | — | — | 200 `[{ id, building, floorOrRoom, latitude, longitude }]` |
| `GET /api/items` | — | sorgu: `q, type, categoryId, locationId, from, to, page=1, pageSize=20` (en fazla 50) | 200 liste yanıtı; yalnızca `Published` |
| `GET /api/items/{id}` | isteğe bağlı | — | 200 ilan detayı |
| `POST /api/items` | gerekli | `{ type, title, categoryId, locationId, eventDate, description, verificationQuestion? }` | 201 ilan detayı, `Pending` |
| `PUT /api/items/{id}` | sahibi / görevli | `POST` ile aynı | 200 ilan detayı |
| `DELETE /api/items/{id}` | sahibi / görevli | — | 204 |

- Liste öğesi: `{ id, type, status, title, categoryId, categoryName, categoryIcon, locationId, locationName, eventDate, createdAt, coverPhotoUrl }`.
- İlan detayı: liste alanları (`coverPhotoUrl` hariç) + `{ description, isAtOffice, ownerName, isOwner, verificationQuestion, photoUrls }`. `verificationQuestion` yalnızca sahibine ve görevliye dolu gelir.
- `eventDate`, `from`, `to` `YYYY-MM-DD` olarak gönderilir; saat dikkate alınmaz.
- `verificationQuestion`, `Found` ilanda zorunludur.
- Sahibi ilanı düzenleyince ilan yeniden `Pending` olur; görevlinin düzenlemesi durumu değiştirmez.
- Hata kodları: 400 doğrulama (`errors`) · 401 oturum yok ya da giriş hatalı · 403 yetki yok · 404 ilan yok ya da görme yetkisi yok · 409 `Resolved`/`Archived` ilan değiştirilemez.

## SignalR hub (`/hubs/chat`)

| Yön | Ad | Veri |
|---|---|---|
| İstemci → Sunucu | `JoinClaim` | `claimId` |
| İstemci → Sunucu | `SendMessage` | `claimId, text` |
| Sunucu → İstemci | `ReceiveMessage` | `{ id, claimId, senderId, senderName, text, sentAt }` |
| Sunucu → İstemci | `Notify` | `{ id, type, content, linkUrl, createdAt }` |

WebSocket başlık gönderemediği için iOS istemcisi token'ı `?access_token=<jwt>` sorgu dizesiyle verir.
