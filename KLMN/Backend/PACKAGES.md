# KLMN Backend paket / referans notları

Kapalı ağda .NET 10 ve EF Core 10 sürümleriyle uyumlu paketleri kullanın.

## KLMN.Application
- MediatR
- FluentValidation
- FluentValidation.DependencyInjectionExtensions
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Options.ConfigurationExtensions

## KLMN.Persistence
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Relational
- Npgsql.EntityFrameworkCore.PostgreSQL

## KLMN.Infrastructure
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity
- Microsoft.IdentityModel.Tokens / System.IdentityModel.Tokens.Jwt
- Microsoft.Extensions.Options.ConfigurationExtensions
- MailKit

## KLMN.Api
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore.SwaggerUI

## İki ayrı appsettings ortamı

- **Production:** `KLMN.Api/appsettings.json`.
- **Development/test:** `KLMN.Api/appsettings.Development.json`.
- Her ikisinde `ConnectionStrings`, `Jwt`, `Authentication`, `PasswordReset`, `InitialAdmin`, `Smtp`, `Cors`, `Logging` ve `AllowedHosts` bölümleri vardır.
- `WebApplication.CreateBuilder(args)` aktif ortam dosyasını otomatik seçer; `Program.cs` içine tekrar `AddJsonFile("appsettings.json")` eklemeyin.
- `launchSettings.json` sadece geliştirme profilinin `Development` ortamında başlatılmasını sağlar. Gizli bilgiler orada tutulmaz.
- `JwtSettings`, `SmtpSettings` vb. C# sınıfları JSON ayarlarının tip güvenli modelleridir.
- `user-secrets` üzerinden bağlantı/parola tanımlama adımı gerekmiyor.

**Önemli:** GitHub deposu public olduğundan gerçek `ConnectionStrings:PostgreSQL`, `Jwt:SecretKey`, `InitialAdmin:Password` ve `Smtp:Password` bilgilerini commit etmeyin. İki ortamın gizli değerlerini yalnızca ilgili kapalı ağ yapılandırmasında doldurun.
