# KLMN Kod İçi Yorumlama Standardı

Bu doküman Backend ve Frontend üzerinde uygulanan **kod açıklama yaklaşımını** tamamlar. Asıl açıklamalar ilgili C# ve TypeScript sınıflarının içinde bulunur.

## Backend: C# XML documentation

- **Sınıf / interface / record:** Sorumluluğu ve hangi katmanda çalıştığını `/// <summary>` ile açıkla.
- **Property:** Sadece alan adını tekrar etme; verinin amacı, nullable olması, güvenlik koşulu, tarih birimi veya veritabanı ilişkisini belirt.
- **Dependency injection alanları:** `_dbContext`, `_timeProvider`, `_authorizationService` gibi üyelerin hangi görevi yerine getirdiğini açıkla.
- **Metot / handler:** Girdinin nasıl değerlendirildiğini, hangi güvenlik/iş kuralının uygulandığını ve ne üretildiğini açıkla.
- **Yerel değişkenler ve işlem blokları:** Token rotation, lockout, concurrency, soft delete, permission override, PostgreSQL index ve seed işlemlerinde neden o hesaplamanın yapıldığını belirt.
- **Güvenlik:** Parola, refresh token, SMTP sırrı ve JWT imzalama anahtarları için loglarda açık değer bulunmaması gerektiğini unutma.
- Yeni public API metotlarına gerektikçe `/// <param>`, `/// <returns>`, `/// <exception>` etiketleri de eklenmelidir.

## Frontend: TypeScript JSDoc

- **Component ve servis:** Uygulamanın hangi işlevinden sorumlu olduğunu `/** ... */` ile yaz.
- **Signal ve observable:** Ne zaman güncellendiğini; oturum, yüklenme veya hata durumuna etkisini belirt.
- **Service dependency ve model alanları:** Backend karşılığını, URI kullanımını ve kullanıcının gördüğü karşılığını açıkla.
- **Component metotları:** Form doğrulama, HTTP isteği, hata işleme ve yönlendirme davranışını belirt.
- **Güvenlik:** Access token yalnızca bellekte, refresh token HttpOnly cookie içinde tutulur. JWT/parola konsola yazılmaz.
- API adresi `environment.development.ts` üzerinden **doğrudan HTTPS** kullanır; Angular CLI proxy kullanılmaz.

## Kontrol yöntemi

Bu dokümantasyon düzenlemesinde kaynak kodun mevcut işlevsel satırları korunarak açıklama satırları eklendi. Gerçek .NET SDK ve node_modules kullanılarak yapılacak `dotnet build` ve `npm run build` kontrolleri, uygulamanın çalışma ortamında ayrıca yürütülmelidir.

## Merkezi yapılandırma

JWT, SMTP, PasswordReset, Authentication, InitialAdmin, PostgreSQL bağlantısı, CORS ve log ayarlarının kaynak dosyası `KLMN.Api/appsettings.json` şeklindedir. `*Settings` C# sınıfları bu dosyadan binding yapılan tipli modellerdir; başka bir konfigürasyon dosyası gerektirmezler. Kod içi yeni açıklamalarda bu ayrımı koruyun.
