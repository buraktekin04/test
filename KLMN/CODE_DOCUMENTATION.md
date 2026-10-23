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

JWT, SMTP, PasswordReset, Authentication, InitialAdmin, PostgreSQL bağlantısı, CORS ve log ayarları iki ortamda ayrıdır: **Production** için `KLMN.Api/appsettings.json`, **Development/test** için `KLMN.Api/appsettings.Development.json` kullanılır. `WebApplication.CreateBuilder(args)` standart yükleme ve override sırasını sağlar. `*Settings` C# sınıfları seçili JSON değerlerinin tipli modelleridir; ayrı veri kaynağı değillerdir. Kod içi yeni açıklamalarda ortam ayrımını koruyun.

## Zorunlu DTO ve Exception dosya düzeni (23.10.2026)

- **Her public Response, MutationResponse ve Option tipi ayrı, kendi ismiyle eşleşen tek `.cs` dosyasında bulunur.** Aynı `Responses.cs` veya `*Response.cs` dosyasına birden fazla public DTO tanımı yazılmaz.
- **Her özel Exception ayrı `.cs` dosyasında bulunur.** NotFound ve Conflict sınıfları ortak `*Exceptions.cs` dosyasında tutulmaz.
- DTO içinde `Expression<Func<TEntity,TResponse>> Projection` üyesi bulunmaz. EF Core projeksiyonu ilgili CQRS QueryHandler'ın `Select` çağrısında yazılır. Böylece DTO sadece API verisini temsil eder.
- Mevcut nullable alanlar, FK gösterim adları, soft-delete, xmin, filter/paging ve permission davranışı korunur. Tür/namespace ve API JSON alan adları değişmez.
- Yeni modül geliştirmede bu kontroller kod incelemesi ve gerçek derleme ile doğrulanmalıdır.
