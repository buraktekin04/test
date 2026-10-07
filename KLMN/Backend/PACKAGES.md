# KLMN Backend NuGet / Framework Notları

Kapalı ağdaki gerçek projede mevcut paket sürümlerini koruyun. Bu aktarım klasörü source-code referansıdır.

Başlıca gereken paket/özellikler:

```text
KLMN.Application
- MediatR
- FluentValidation
- FluentValidation.DependencyInjectionExtensions
- Microsoft.EntityFrameworkCore

KLMN.Persistence
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Relational
- Npgsql.EntityFrameworkCore.PostgreSQL

KLMN.Infrastructure
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity
- System.IdentityModel.Tokens.Jwt
- MailKit

KLMN.Api
- Microsoft.AspNetCore.OpenApi
- Swagger UI / Swashbuckle.AspNetCore (UseSwaggerUI için)
```

Secret değerleri source control'e yazmayın:

- ConnectionStrings:DefaultConnection
- Jwt:SecretKey
- InitialAdmin:Password
- Smtp:Password

Development ortamında user-secrets veya kurumun güvenli configuration mekanizması kullanılmalıdır.
