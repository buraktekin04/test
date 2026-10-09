# KLMN Backend – .NET 10 / EF Core 10

Bu klasör, orijinal MSKBS sınıflarının **KLMN** ad alanına uyarlanmış katmanlı sürümüdür.

## Ortama göre appsettings dosyaları

KLMN projesinin **iki ayrı ayar dosyası** vardır:

| Ortam | Dosya | Kullanım |
| --- | --- | --- |
| **Production (canlı)** | `KLMN.Api/appsettings.json` | Canlı bağlantılar ve uygulama ayarları |
| **Development (geliştirme/test)** | `KLMN.Api/appsettings.Development.json` | Geliştirme ve test bağlantıları ve ayarları |

`WebApplication.CreateBuilder(args)` bu dosyaları varsayılan olarak yükler. **Development** ortamında `appsettings.Development.json` aynı isimli `appsettings.json` değerlerini ezer. **Production** ortamında `appsettings.json` kullanılır. Artık `Program.cs` içerisinde ikinci kez `AddJsonFile("appsettings.json")` yapılmaz; önceki hatalı öncelik davranışı düzeltildi.

Her iki dosyada da aynı bölümler bulunur. Her ortamın parolası ve bağlantı ayarları ayrı tutulabilir:

| Bölüm | İçerik |
| --- | --- |
| `ConnectionStrings:PostgreSQL` | PostgreSQL sunucusu, port, veritabanı, kullanıcı adı ve parola |
| `Jwt` | SecretKey, issuer, audience, access/refresh token süreleri |
| `Authentication` | Giriş başarısızlık limiti ve hesap kilitleme süresi |
| `PasswordReset` | Sıfırlama token süresi ve Angular reset ekranı URL'si |
| `InitialAdmin` | Ortamın ilk yönetici kullanıcı adı/e-posta/parolası |
| `Smtp` | SMTP host, port, kullanıcı, parola, gönderen adresi/adı |
| `Cors:AllowedOrigins` | İlgili ortamdan API'ye erişebilen Angular origin'leri |
| `Logging` | Log seviyeleri |
| `AllowedHosts` | ASP.NET Core host filtresi |

`JwtSettings`, `AuthenticationSettings`, `PasswordResetSettings` ve `SmtpSettings` **ayrı ayar deposu değildir**. Etkin ortamın JSON dosyasındaki değerleri C# tarafında güçlü tiple sunan ve `ValidateOnStart` ile kontrol eden modellerdir.

### Geliştirme ve test ortamı

`KLMN.Api/Properties/launchSettings.json` dosyasındaki **KLMN.Api (Development)** profiliyle çalıştırıldığında `ASPNETCORE_ENVIRONMENT=Development` seçilir; uygulama `appsettings.Development.json` dosyasındaki değerleri kullanır. Launch profile yalnızca ortamı ve HTTPS portunu seçer; veritabanı, admin, SMTP veya JWT bilgisi bu dosyada saklanmaz.

```powershell
cd KLMN/Backend
dotnet restore KLMN.slnx
dotnet build KLMN.slnx
dotnet run --project KLMN.Api --launch-profile "KLMN.Api (Development)"
```

Geliştirme API adresi `https://localhost:7145` olarak ayarlıdır. Angular da proxy kullanmadan `https://localhost:7145/api` adresine doğrudan istek gönderir.

### Canlı ortam

Publish edilen uygulamanın çalışma ortamı **Production** seçili olmalıdır. Visual Studio veya `dotnet run` launch profilini canlıda kullanmayın; development profilini canlı sunucuda başlatırsanız Development ayarları okunur. Production'da `appsettings.json` içerisindeki gerçek host, JWT, SMTP, e-posta ve admin bilgilerini yapılandırın. Dosyadaki `PROD_...` ve `klmn.example.invalid` değerleri **yalnızca değiştirilecek örneklerdir**.

### Doldurulması gereken alanlar

- Her iki ortamda `Jwt:SecretKey`: En az 32 byte uzunlukta, **birbirinden farklı** rastgele secret girin.
- `ConnectionStrings:PostgreSQL`: Development'ta örnek `KLMN_Dev`, canlıda `KLMN`; iki ortamın veritabanı hesabını ve şifresini gerçek değerlerle doldurun.
- `InitialAdmin:UserName`, `Email`, `Password`: İlgili ortamda ilk admin oluşturulacaksa doldurun. Seeder mevcut kullanıcının parolasını her startup'ta değiştirmez.
- `Smtp:Host`, `Smtp:FromAddress`, gerektiğinde `UserName` ve `Password`: İlgili ortamın SMTP sunucusunun bilgilerini yazın.

### SMTP test örnekleri

**Development** (`appsettings.Development.json`) örneğinde `localhost:1025` ve `no-reply@klmn.test` kullanılır. Bu yapı **smtp4dev veya MailHog gibi yerel SMTP yakalayıcı** çalıştırıldığında gerçek alıcılara e-posta göndermeden parola sıfırlama e-postalarını test etmek içindir. Yerel test sunucusu kimlik doğrulama gerektirmediğinden `UserName` ve `Password` bilinçli olarak boştur. SMTP yakalayıcı kurulu/çalışır değilse e-posta gönderimi başarısız olur.

**Production** (`appsettings.json`) bölümüne `smtp.example.invalid:587`, `EXAMPLE_SMTP_USER`, `EXAMPLE_PASSWORD_NOT_REAL` ve `no-reply@klmn.example.invalid` gibi **tamamen kurgusal** değerler konuldu. Bu adres kasıtlı olarak çalışmayan örnektir; canlı sistemde host, gönderen adresi ve kullanıcı/parola değerleri gerçek kurum SMTP bilgileriyle değiştirilmelidir. GitHub reposu public olduğundan gerçek parolalar commit edilmez.
- `PasswordReset:ResetUrlBase`: Angular'ın o ortamdaki sıfırlama ekranı adresini yazın.
- `Cors:AllowedOrigins`: İlgili ortamın Angular adreslerini açıkça listeleyin.

**GitHub güvenliği:** `buraktekin04/test` şu anda public olduğu için gerçek parola ve JWT anahtarları repoya yazılmadı. Kapalı ağda gerçek bilgileri ilgili JSON'a girebilirsin; gerçek parolaları public GitHub'a geri pushlama. Her iki ortamda boş `Jwt:SecretKey` değeri doldurulmadan API'nin `ValidateOnStart` kontrolü geçmez. SMTP alanları artık örneklerle doludur ancak Production SMTP adresi kurgusal olduğundan gerçek e-posta gönderemez.

## Proje katmanları

- **KLMN.Domain:** Ortak entity, kullanıcı, rol, permission, organizasyon ve token modelleri.
- **KLMN.Application:** MediatR/CQRS, FluentValidation, auth ve authorization işlemleri.
- **KLMN.Persistence:** PostgreSQL EF Core, `xmin`, named query filter, soft delete ve seeding.
- **KLMN.Infrastructure:** JWT, MailKit SMTP, current user ve hashleme servisleri.
- **KLMN.Api:** HTTP endpoint'leri, CORS, ProblemDetails ve auth middleware.

## Migration (önemli)

Bu depoda henüz ilk EF Core migration dosyası bulunmuyor. İlk başlangıçtan önce uygun ortamın bağlantı değerleriyle migration oluşturun:

```powershell
cd KLMN/Backend
dotnet tool install --global dotnet-ef --version 10.0.0

# EF CLI için Development dosyasını açıkça seçer; bağlantı/parola
# bilgileri yine appsettings.Development.json üzerinden okunur.
$env:ASPNETCORE_ENVIRONMENT = "Development"

dotnet ef migrations add InitialCreate --project KLMN.Persistence --startup-project KLMN.Api --output-dir Migrations
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api

# Sonraki işlemlerin yanlışlıkla Development olarak çalışmaması için:
Remove-Item Env:ASPNETCORE_ENVIRONMENT
```

Migration ve seed işlemleri **seçili ortamın PostgreSQL veritabanına** uygulanacaktır. Geliştirme/test ve canlı veritabanlarını karıştırmayın.

## Auth ve frontend notları

JWT access token Angular belleğindedir, refresh token HttpOnly cookie'dedir; refresh rotation ve security stamp kontrolü korunur. `xmin` optimistic concurrency çatışmaları HTTP 409 ile ele alınır. Angular proxy kullanmaz. Geliştirmede `https://localhost:4200` Angular origin'i, `appsettings.Development.json > Cors:AllowedOrigins` ile eşleşmelidir.

> Bu değişikliklerde yapılandırma ve dosya tutarlılığı kontrol edilir; gerçek DB/SMTP bağlantısı ve derleme kurum ortamında ayrıca doğrulanmalıdır.
