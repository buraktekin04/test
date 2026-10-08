# KLMN Backend paket / referans notları

Kurumun kapalı ağında .NET 10 ve EF Core 10 ile uyumlu paket sürümlerini kullanın.

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

## Yapılandırma politikası

Uygulama, tüm bağlantı ve kimlik doğrulama bilgilerini **KLMN.Api/appsettings.json** dosyasından okur. JWT, SMTP, PasswordReset ve Authentication Settings/Options sınıfları bağımsız ayar deposu değildir; appsettings bölümlerine tip güvenli erişim sağlar.

`user-secrets`, ek JSON ayar dosyası veya kullanıcı ortam değişkeni üzerinden yapılandırma adımı gerekmemektedir.

**Önemli:** GitHub reposu public olduğu için gerçek `ConnectionStrings:PostgreSQL`, `Jwt:SecretKey`, `InitialAdmin:Password` ve `Smtp:Password` değerleri repoya commit edilmemelidir. Kapalı ağda bu alanlar `appsettings.json` içinde doldurulur.
