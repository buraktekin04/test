# KLMN Backend – .NET 10 / EF Core 10

Bu klasör, orijinal MSKBS sınıflarının **KLMN ad alanına uyarlanmış** katmanlı sürümüdür.

## Projeler
- **KLMN.Domain:** BaseEntity, kullanıcı, rol, permission, organizasyon ve token entity'leri.
- **KLMN.Application:** CQRS/MediatR, FluentValidation, kullanıcı yetkilendirme ve auth akışları.
- **KLMN.Persistence:** PostgreSQL EF Core, `xmin` optimistic concurrency, named filtreler, soft delete ve seed.
- **KLMN.Infrastructure:** JWT, security stamp, MailKit SMTP ve teknik servisler.
- **KLMN.Api:** HTTP endpointleri, ProblemDetails ve JWT middleware.

## İlk kurulum
.NET 10 SDK ve PostgreSQL sunucusu gerekir.

```powershell
cd KLMN/Backend
dotnet restore KLMN.slnx
dotnet build KLMN.slnx
cd KLMN.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=klmn;Username=YOUR_USER;Password=YOUR_PASSWORD"
dotnet user-secrets set "Jwt:SecretKey" "YOUR_SECURE_RANDOM_SECRET_AT_LEAST_32_BYTES"
```

`InitialAdmin:UserName`, `InitialAdmin:Email`, `InitialAdmin:Password`, `Smtp:Host`, `Smtp:FromAddress` ve gerekiyorsa `Smtp:Password` değerlerini güvenli yapılandırmaya girin. **Şifreleri Git'e göndermeyin.**

### Migration (önemli)
Depoda henüz **EF Core migration bulunmuyor**. API başlangıçta `MigrateAsync` ve `IdentitySeeder` çalıştırır; ilk migration üretilmeden boş DB üzerinde çalıştırmayın.

```powershell
# KLMN/Backend klasöründe
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations add InitialCreate --project KLMN.Persistence --startup-project KLMN.Api --output-dir Migrations
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api
dotnet run --project KLMN.Api
```

Design-time komutlarında veritabanı ve JWT ayarları erişilebilir olmalıdır.

## Authentication
- JWT access token response gövdesindedir; Angular'da yalnızca bellekte tutulur.
- SHA-256 hashed refresh token DB'de; açık değer HttpOnly cookie'dedir.
- Refresh rotation/reuse detection; password change/reset/logout-all security stamp yeniler.
- `POST /api/auth/logout`, access token süresi dolsa da cookie ile session revoke edebilir.
- Permission kontrolleri anlık DB verisine göre çalışır; ADMIN bypass, kullanıcı override ve rol permission önceliği korunur.
- `GlobalExceptionHandler` concurrency çatışmasını HTTP 409 olarak döndürür.
- `xmin` PostgreSQL sistem kolonudur; fiziksel `Version` kolonu eklemeyin.

## Ortam ayrımı
`appsettings.json` içinde gerçek secret bırakmayın. Angular development proxy `/api` yolunu yerel API'ye yönlendirir. HTTPS ve cookie ayarlarını production ortamında doğrulayın.

Bu commit, GitHub dosyalarının kaynak sözleşmelerini düzenler; gerçek kurum/kapalı ağ veritabanında migration ve uygulama testi ayrıca yapılmalıdır.
