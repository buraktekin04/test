# Kullanıcı Tanımları ekranı — 12 Ekim 2026 iyileştirmeleri

**Kapsam:** Kullanıcı Tanımları ekranı ve bu ekranın mevcut Users API endpoint'leri.
**Yapı:** Bilinçli olarak ortak Angular bileşeni, generic tablo veya global toast servisi
eklenmedi. Bütün görsel/etkileşim davranışları mevcut UserListComponent dosyalarındadır.
Mevcut `/app/users` route'u ve Tanımlamalar menüsü aynen korunur.

## Yapılan işler

1. **10 saniyelik bildirimler:** Başarı/hata bildirimleri ekranın üst sağında çıkar;
   10 saniye sonra otomatik kapanır ve sağındaki X ile kapatılabilir.
   `user-list.component.ts` içinde Angular `effect` zamanlayıcıları kullanıldı;
   herhangi bir global servis yazılmadı.
2. **Kolon filtreleri:** Ad Soyad, Kullanıcı Adı, E-posta, Organizasyon, Roller,
   Aktif/Pasif, Kilitli/Açık, Mesai Dışı erişim, Başlangıç, Bitiş ve Oluşturulma.
   Sorgular **sunucu tarafında** yapılır; yalnızca mevcut 20 satır filtrelenmez.
   Genel arama/pasifleri dahil et filtreleriyle birlikte kullanılabilir.
   Tarih filtreleri UTC gün sınırına göre çalışır.
   Metin alanlarında Enter ya da odak kaybında, seçim/tarihlerde değiştiğinde uygulanır.
   `Temizle` tüm filtreleri sıfırlar.
3. **Kullanıcı kilitleme:** Düzenleme formundaki `Kullanıcı hesabını kilitle`
   işareti `Users.IsLocked` alanına kaydedilir. `LockoutEnd` ve başarısız giriş
   sayısı sıfırlanır. Kullanıcı kendi hesabını/ADMIN hesabını kilitleyemez.
   Kilit durumunun değişmesi `SecurityStamp` değerini değiştirerek eski access
   token'ları geçersiz kılar. Eski HTTP istemcileri `isLocked` göndermiyorsa
   kilit değeri korunur; yanlışlıkla açılmaz.
4. **Paginator açılır menüsü:** Yatay scroll yalnızca tabloya alındı;
   `p-paginator` scroll dışına çıkarıldı. Böylece `10, 20, 50, 100` açılır
   listesi tablo/alt bölüm tarafından kırpılmaz.
5. **Geçerlilik tarihleri:** Kullanıcı formunda hem başlangıç hem bitiş
   tarih/saat alanının yanında X temizleme butonu bulunur.
   Veritabanındaki `ValidFrom` ve `ValidTo` alanları değişmedi.
6. **Gerçek Excel (.xlsx):** Tablo üst sağında `Excel'e Aktar` butonu var.
   Aynı arama ve kolon filtreleriyle, mevcut sayfayı değil bütün eşleşen
   kayıtları export eder. `GET /api/users/export` endpoint'i `Users.Query`
   iznini kullanır. Açık kaynak üçüncü taraf NuGet/npm paketi gerektirmeyen,
   Open XML ZIP formatı kullanıldı. En fazla 5000 kayıt aktarılır; fazla ise
   bilinçli olarak 400 hatası döndürülür, satırlar sessizce kesilmez.
   Yalnızca kullanıcı profil alanları aktarılır, gizli parola/token/PDF yoktur.

## Yeni dosya

Backend:
- `KLMN.Api/Exports/UserExcelExporter.cs`

## Güncellenen Backend dosyaları

- `KLMN.Api/Controllers/UsersController.cs`
- `KLMN.Application/Users/Commands/UpdateUser/UpdateUserCommand.cs`
- `KLMN.Application/Users/Commands/UpdateUser/UpdateUserCommandHandler.cs`
- `KLMN.Application/Users/Queries/GetUsers/GetUsersQuery.cs`
- `KLMN.Application/Users/Queries/GetUsers/GetUsersQueryHandler.cs`
- `KLMN.Application/Users/Queries/GetUsers/GetUsersQueryValidator.cs`

## Güncellenen Frontend dosyaları

- `src/app/features/users/models/update-user-request.model.ts`
- `src/app/features/users/models/user-query.model.ts`
- `src/app/features/users/services/user.service.ts`
- `src/app/features/users/user-list/user-list.component.ts`
- `src/app/features/users/user-list/user-list.component.html`
- `src/app/features/users/user-list/user-list.component.scss`

## Veritabanı ve migration

Bu commit için **migration yok**. `IsLocked`, `LockoutEnd`,
`AccessFailedCount`, `SecurityStamp`, `ValidFrom`, `ValidTo`
Users tablosunda zaten mevcuttur. Yeni tablo veya kolon yoktur.

## Test listesi

1. `dotnet build`, `npm run build` ile derleyin.
2. Oturum açıp bildirimleri test edin: otomatik 10 saniye ve manuel X.
3. Global arama ile farklı kolon filtreleri birlikte tüm kullanıcı sayfalarında
   aynı sonucu vermeli; `Temizle` ile reset olmalı.
4. ADMIN kullanıcısını veya kendi hesabınızı kilitlemeye kalkışınca 409 dönmeli.
5. Normal kullanıcı kilitlenince aktif JWT/refresh/login engellenmeli;
   kilit açılınca başarısız giriş sayısı ve kilit süresi sıfırlanmalı.
6. Kayıt sayısı seçicisini açınca menü görünür kalmalı, tablo scroll bozulmamalı.
7. Geçerlilik tarihlerini temizleyip kaydettiğinizde API'ye null gitmeli.
8. Excel dosyasını Excel'de açın; başlıklar ve filtreli tüm satırlar bulunmalı.
   5000 üzerindeki sonucu daraltmadan export etmeye çalışınca uyarı beklenmeli.
9. PDF belgeleri, görev yeri ve önceki ekran özellikleri korunmalı.

> Kod GitHub'a aktarılmıştır; çalışan bilgisayarda gerçek .NET/Angular derlemesi,
> veritabanı ve tarayıcı testleri ayrıca yapılmalıdır.
