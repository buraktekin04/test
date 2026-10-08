# KLMN Backend – .NET 10 / EF Core 10

Bu klasör, orijinal MSKBS sınıflarının **KLMN** ad alanına uyarlanmış katmanlı sürümüdür.

## Tek yapılandırma dosyası: appsettings.json

**Uygulamanın bütün teknik ve başlangıç ayarları** `KLMN/Backend/KLMN.Api/appsettings.json` içerisinde yönetilir. `user-secrets`, ayrı `appsettings.Development.json` dosyası veya uygulamaya özgü environment variable tanımlamak **zorunlu değildir**. Bu projede tüm KLMN ayarlarının önceliği `appsettings.json` olacak biçimde düzenlenmiştir.

Yönetilecek bölümler:

| Bölüm | İçerik |
| --- | --- |
| `ConnectionStrings:PostgreSQL` | PostgreSQL host, port, veritabanı, kullanıcı adı, parola |
| `Jwt` | İmza anahtarı, issuer, audience, access/refresh token süreleri |
| `Authentication` | Hatalı giriş limiti, geçici hesap kilidi süresi |
| `PasswordReset` | Token süresi, Angular sıfırlama ekranı URL'si |
| `InitialAdmin` | İlk yönetici kullanıcı adı, e-posta, parola, ad ve soyad |
| `Smtp` | Sunucu, port, kimlik doğrulama hesabı/parola, gönderen adresi/adı |
| `Cors:AllowedOrigins` | Proxy kullanılmadan API'ye erişecek Angular HTTPS origin'leri |
| `Logging` | Log seviye ayarları |
| `AllowedHosts` | ASP.NET Core host filtreleme ayarı |

`JwtSettings`, `AuthenticationSettings`, `PasswordResetSettings` ve `SmtpSettings` sınıfları ayrı dosyada konfigürasyon **saklamaz**. Bunlar yalnızca `appsettings.json` içindeki değerleri tip güvenli biçimde uygulamaya bağlayan ve doğrulayan C# modelleridir. Bu sayede mevcut DI/Options kullanan sınıflar değişmeden çalışır.

### appsettings.json nasıl doldurulacak?

Dosya içerisindeki boş alanları **kurum ağındaki gerçek bilgilerle** değiştirin:

- `ConnectionStrings:PostgreSQL`: Şablonda host/port/veritabanı bilgisi vardır; `Username` ve `Password` dahil tamamını kuruma göre düzenleyin.
- `Jwt:SecretKey`: En az 32 byte uzunluğunda **rastgele oluşturulmuş** bir imza anahtarı yazın. Uygulama boş anahtarla başlatılmaz.
- `InitialAdmin:UserName`, `Email`, `Password`: İlk yönetici oluşturulacaksa bu üç alanı doldurun. Boş bırakıldığında ilk admin kullanıcısı oluşturulmaz; roller ve permission'lar oluşturulabilir. Seeder mevcut yöneticinin parolasını her açılışta sıfırlamaz.
- `Smtp:Host` ve `Smtp:FromAddress`: E-posta sunucusu ve gönderen adresini girin; SMTP authentication varsa `Smtp:UserName` ve `Password` alanlarını da doldurun.
- `PasswordReset:ResetUrlBase`: Angular parola yenileme sayfası. Geliştirmede `https://localhost:4200/reset-password`, kurumda yayın adresiniz.
- `Cors:AllowedOrigins`: Angular'ın tam origin'i (`https://localhost:4200` gibi). CORS wildcard yerine belirtilen adresleri ve credential'lı istekleri destekler.

**Güvenlik ve GitHub:** Bu GitHub deposu şu anda **public**. Bu yüzden gerçek admin/SMTP/PostgreSQL parolaları ve JWT imza anahtarı commit'e eklenmemiştir. Uygulamayı kapalı ağda çalıştırırken gerçek bilgileri yalnızca oradaki `appsettings.json` dosyasına yazabilirsiniz. Bu dosyanın gerçek parolalı halini public GitHub'a göndermeyin; kapalı ağda olması, GitHub deposunun erişimini değiştirmez.

## Projeler

- **KLMN.Domain:** BaseEntity, kullanıcı, rol, permission, organizasyon ve token entity'leri.
- **KLMN.Application:** CQRS/MediatR, FluentValidation, kullanıcı yetkilendirme ve auth akışları.
- **KLMN.Persistence:** PostgreSQL EF Core, `xmin` concurrency, named query filter'lar, soft delete ve seed.
- **KLMN.Infrastructure:** JWT, security stamp, MailKit SMTP ve teknik servisler.
- **KLMN.Api:** HTTP endpoint'leri, ProblemDetails, CORS ve JWT middleware.

## İlk kurulum

.NET 10 SDK ve PostgreSQL kurulmuş olmalıdır.

```powershell
cd KLMN/Backend

# Önce KLMN.Api/appsettings.json dosyasındaki bilgileri doldurun.
dotnet restore KLMN.slnx
dotnet build KLMN.slnx
```

### Migration (önemli)

Depoda henüz **ilk EF Core migration dosyası bulunmuyor**. API açılışta `MigrateAsync` ve `IdentitySeeder` çalıştırır. Boş veritabanında ilk migration oluşturulmadan API'yi başlatmayın.

```powershell
# KLMN/Backend klasöründe
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations add InitialCreate --project KLMN.Persistence --startup-project KLMN.Api --output-dir Migrations
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api
dotnet run --project KLMN.Api
```

## Authentication

- Access token Angular'da yalnızca bellekte tutulur; refresh token HttpOnly cookie'dedir.
- Refresh token veritabanında SHA-256 hash olarak saklanır ve rotation/reuse detection uygulanır.
- Parola değiştirme/sıfırlama ve tüm cihazlardan çıkış security stamp'i yeniler.
- Yetki kontrolleri anlık veritabanı bilgisine göre çalışır.
- EF Core concurrency çatışmaları HTTP 409 olarak döner; `xmin` fiziksel `Version` kolonu gerektirmez.

## Angular ile bağlantı

Angular proxy **kullanılmaz**. Frontend `https://localhost:4200` üzerinden API'ye `https://localhost:7145/api` adresinden doğrudan istek gönderir. Farklı origin/port kullanılıyorsa backend `appsettings.json > Cors:AllowedOrigins` ve Angular `environment.development.ts > apiBaseUrl` alanlarını eşleştirin. Refresh cookie `Secure=true` ve `SameSite=Lax` ayarlıdır; yerel HTTPS sertifikalarını güvenilir kılın.

> Derleme, migration ve kurum içi SMTP/DB bağlantıları mevcut çalışma ortamında ayrıca doğrulanmalıdır.
