# KLMN – Tanımlama Audit Log Altyapısı

## Kapsam

- EF Core `SaveChanges` interceptor değişiklikleri ana kayıtla **aynı transaction içinde** yazdırır.
- Kullanıcı, rol, organizasyon, permission ve UserRole / RolePermission /
  UserPermission / UserAgreementPdf ilişkileri desteklenir.
- Oluşturma, değiştirme, pasifleştirme, aktifleştirme ve soft-delete kaydedilir.
- Kayıt kimliği, işlem zamanı (UTC), işlemi yapan kullanıcı ve güvenli alanların
  eski/yeni değerleri JSONB olarak saklanır.
- İlişki kayıtları ayrıca ana kullanıcı/rol geçmişinde de listelenir.
- Geçmiş salt okunurdur; düzenleme veya silme endpoint'i yoktur.
- `PasswordHash`, `SecurityStamp`, refresh/reset tokenlar, PDF disk yolu gibi
  sırlar loglanmaz. Yalnızca interceptor'da açıkça izin verilen alanlar saklanır.
- Audit verisini görmek için `AuditLogs.View` yetkisi gerekir. İzin seeder'a
  eklendiği için mevcut admin tekrar giriş yaparak yeni menüyü görür.

## Migration – ÖNEMLİ

GitHub deposunda KLMN'in **önceden oluşturulmuş EF Core migration ve
ModelSnapshot dosyaları bulunmamaktadır**. Yerel projende bu dosyalar mevcut
olduğu için yeni migration'ı **o snapshot üzerinde üretmelisin**.

Yeni entity, konfigürasyon, DbContext ve interceptor dosyalarını
kopyaladıktan sonra `KLMN/Backend` dizininde:

```powershell
dotnet ef migrations add AddAuditLogs --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Visual Studio Package Manager Console kullanıyorsan:

```powershell
Add-Migration AddAuditLogs -Project KLMN.Persistence -StartupProject KLMN.Api -Context KLMNDbContext
Update-Database -Project KLMN.Persistence -StartupProject KLMN.Api -Context KLMNDbContext
```

**Migration uygulanmadan API'yi başlatma.** `Program.cs` zaten
`MigrateAsync` çağırıyor; tablo henüz yoksa uygulama veri kaydedemez.
Migration kodunu açıp yeni `AuditLogs` tablosu ve indeksleri dışında
beklenmeyen eski tablo değişiklikleri olup olmadığını kontrol et.

## Endpoint ve ekran

- Genel geçmiş: `GET /api/audit-logs?pageNumber=1&pageSize=20`
- Tek kayıt: `GET /api/audit-logs?entityType=User&entityId={guid}`
- İlişkili hareketler ilgili User/Role kaydının geçmişinde görünür.
- Yalnızca okuma yetkisi: `AuditLogs.View`
- Menü: **İşlem Geçmişi** / `/app/audit-logs`
- Kullanıcı ve Rol Tanımları satırlarında: saat/Geçmiş ikonu ve ortak geçmiş dialog'u
- Kaydetme işlemi: Rol Yetkileri panelinin **sağ üst köşesi**.

## Test adımları

1. Veritabanına migration uygula; `AuditLogs` tablosunu doğrula.
2. Admin ile yeniden oturum aç; menüye **İşlem Geçmişi** eklendiğini doğrula.
3. Yeni kullanıcı oluştur → satırdaki Geçmiş'te `Create` ve değişen alanları gör.
4. Kullanıcı adı/mesai kısıtı değiştir → sadece değişen alanlar `Update` olarak görünmeli.
5. Aktif/pasif ve silme → sırasıyla `Deactivate/Activate/Delete` ve önceki değerler.
6. Rol oluştur, güncelle ve yetkilerini değiştir → rol geçmişinde
   `RolePermission` ilişki hareketleri de görünmeli.
7. Genel geçmişte tarih, işlem, varlık türü, GUID filtreleri ve sayfalama.
8. Yetkisiz kullanıcıya `GET /api/audit-logs` isteği 403 dönmeli.
9. `AuditLogs` içinde PasswordHash, SecurityStamp veya token bulunmamalı.
10. Bir işlem rollback olursa audit kaydı da yazılmamalı.

## Operasyon notu

Eski kayıtların geçmişi geriye dönük oluşturulamaz; kayıtlar ancak bu sistem
devreye alındıktan sonra oluşur. Audit Log, uygulama hataları ve HTTP istek
loglarından ayrı bir veri türüdür.

Üretim ortamında `AuditLogs` tablosu için saklama süresi, güvenli yedekleme,
kişisel veri erişim politikası ve büyüme/partition planı ayrıca belirlenmelidir.
Uygulama seviyesinde kayıtları güncelleme/silme endpoint'i bulunmaması,
veritabanı yöneticisine karşı kriptografik değiştirilemezlik sağlamaz.

> Bu ortamda .NET SDK ve tam Angular bağımlılıkları olmadığı için gerçek derleme,
> migration ve tarayıcı testleri kullanıcı ortamında yapılmalıdır.
