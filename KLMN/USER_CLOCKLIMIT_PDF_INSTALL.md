# ClockLimit ve kullanıcı PDF dokümanları: kurulum

## Mesai saati kuralı

ClockLimit = true: Kullanıcı yalnızca tanımlı mesai gün ve saatlerinde sisteme erişebilir.
ClockLimit = false: Mesai dışı kullanım yetkilendirilmiştir.

Bu kural LoginCommandHandler, RefreshTokenCommandHandler ve JWT OnTokenValidated
aşamalarında uygulanır. Erişim engelinde HTTP 403 ve Türkçe uyarı döner.
Angular login sayfası uyarıyı gösterir. Mesai saati uygulamanın çalıştığı
sunucunun değil, WorkingHours.TimeZoneId değerinin yerel saatine göre hesaplanır.

İki appsettings dosyasında WorkingHours:
- TimeZoneId: Europe/Istanbul
- Start: 08:00 (dahil)
- End: 18:00 (hariç)
- DaysOfWeek: 1,2,3,4,5 (Pazartesi-Cuma, ISO 1..7)

Bu örnek saatleri kurumun gerçek mesai saatlerine göre değiştirin.
Özel günler, resmî tatiller ve geceye sarkan vardiyalar bu sürümde yoktur.

## PDF dosya saklama

Development appsettings.Development.json:
DocumentStorage.RootPath = %USERPROFILE%\Desktop\KLMN\UserAgreements
DocumentStorage.MaxFileSizeMb = 10

Production appsettings.json:
DocumentStorage.RootPath = D:\KLMN\Storage\UserAgreements
DocumentStorage.MaxFileSizeMb = 10

Canlı sunucuda D diski yoksa gerçek diski yapılandırın. Linux'ta
örneğin /var/lib/klmn/user-agreements kullanın. İlk yüklemede gerekli
alt klasörler otomatik açılır. API işlem hesabına klasör okuma/yazma/silme
izni verin, wwwroot içinde tutmayın. Sunucu arızalarına karşı PDF
klasörünün ve PostgreSQL veritabanının eşzamanlı yedeğini alın.

Dosyalar rastgele GUID adıyla UserId klasörüne kaydedilir. Veri tabanında
UserAgreementPdfs tablosu sadece UserId, OriginalFileName, RelativePath,
SizeBytes, ContentType, audit ve xmin değerlerini tutar. BinaryData
kolonu yoktur. API yalnızca dosya stream'ini yetki kontrolünden sonra verir.
PDF uzantısı, ilk PDF imza baytları ve 10 MB dosya sınırı kontrol edilir.
Gelecekte antivirüs taraması ve şifreli depolama eklenmesi önerilir.

## Yetkiler ve endpointler

- Users.ViewDocuments: GET /api/users/{id}/agreements
- Users.ViewDocuments: GET /api/users/{id}/agreements/{documentId}/download
- Users.ManageDocuments: POST /api/users/{id}/agreements (multipart file)
- Users.ManageDocuments: DELETE /api/users/{id}/agreements/{documentId}

ADMIN rolü yeni permission seed kayıtlarını otomatik alır; diğer roller
için ayrıca atama gerekir. Frontend butonlarının gizlenmesi tek başına
yetkilendirme değildir; backend HasPermission kontrolü zorunludur.

## EF Core migration (zorunlu)

Yeni Users.ClockLimit bool kolonu ve UserAgreementPdfs tablosu
ekleniyor. Github reposunda migration geçmişi olmadığından yapay bir
snapshot üretilmedi. Kendi yerel solution'ınızdaki mevcut migration
geçmişini kullanarak şu komutları çalıştırın:

PowerShell:
1) cd KLMN/Backend
2) $env:ASPNETCORE_ENVIRONMENT = "Development"
3) dotnet ef migrations add AddClockLimitAndUserAgreementPdfs --project KLMN.Persistence --startup-project KLMN.Api --output-dir Migrations
4) dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api
5) Remove-Item Env:ASPNETCORE_ENVIRONMENT

Yerelde proje MSKBS adını kullanıyorsanız komutlardaki proje adlarını,
namespace'leri ve migration klasörünü yerel projenizle eşleştirin.
Mevcut migration geçmişiniz yoksa önce orijinal mevcut şemaya uygun
baseline yaklaşımını belirleyin; mevcut veri tabanına sıfırdan
InitialCreate uygulamayın. Canlıda migration öncesi yedek alın.

Yeni kolonda ClockLimit=true varsayılanı mevcut kullanıcılara da gelir.
Mesai dışı erişmesi gereken kullanıcıların alanını false yapmalısınız.
InitialAdmin.ClockLimit=false sadece ilk defa oluşturulan admin için
geçerlidir. Zaten var olan admin'in ClockLimit değerini seeder her
açılışta değiştirmez; mevcut ADMIN için değer ayrıca güncellenmelidir.

## Test listesi

- ClockLimit=true için mesai içinde login başarılı, dışında 403.
- ClockLimit=false için mesai dışı login başarılı.
- Mesai bittiğinde daha önce alınan JWT ile yeni API isteği 403.
- Refresh token mesai dışında ClockLimit=true için 403.
- Kullanıcı düzenleme ekranında mesai kısıt checkbox kontrolü.
- Kaydedilmiş kullanıcıya PDF yükleme, listeleme, indirme, silme.
- Kullanıcıya ait PDF fiziksel klasörde var, DB'de byte içeriği yok.
- Belge permission yokken endpoint 403, yetki varken çalışıyor.
- dotnet build, npm run build ve yerel ortamda migration testi.

Bu işlem yalnızca Github'a kaynak kod ve örnek ayarlar ekler.
Gerçek masaüstü/sunucu klasörü ve veritabanı GitHub üzerinden değişmez.
