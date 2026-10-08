# KLMN Backend Paket / Referans Notları

Kapalı ağdaki gerçek projede paket sürümlerini mevcut .NET 10 / EF Core 10 sürümleriyle uyumlu tutun.

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
- Swagger UI paketi (mevcut projede `UseSwaggerUI` sağlayan paket)

## Source control'e yazılmaması gerekenler

- `ConnectionStrings:PostgreSQL`
- `Jwt:SecretKey`
- `InitialAdmin:Password`
- `Smtp:Password`

Bu değerleri user-secrets, environment variable veya kurumun güvenli configuration mekanizması üzerinden verin.
