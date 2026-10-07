# KLMN Backend Aktarım Paketi

Bu klasör, kapalı ağdaki gerçek KLMN projesine manuel aktarım için hazırlanmış güncel backend dosyalarını içerir.

## Mimari

```text
KLMN.Api
KLMN.Application
KLMN.Domain
KLMN.Infrastructure
KLMN.Persistence
```

## Teknoloji ve kararlar

- .NET 10
- EF Core 10
- PostgreSQL / Npgsql
- Clean Architecture
- CQRS + MediatR
- FluentValidation
- Generic Repository / UnitOfWork yok
- Handler -> IKLMNDbContext -> DbSet -> LINQ -> EF Core
- JWT access token
- HttpOnly refresh token cookie
- Refresh token rotation + reuse detection
- Password reset token hash storage
- Role + Permission authorization
- ADMIN > UserPermission override > RolePermission > deny
- PostgreSQL xmin optimistic concurrency
- Named Global Query Filters: SoftDeleteFilter + ActiveFilter
- Soft delete + audit SaveChanges interceptor
- Access token memory-only frontend yaklaşımı

## Auth endpointleri

```text
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
POST /api/auth/logout-all
GET  /api/auth/me
POST /api/auth/change-password
POST /api/auth/forgot-password
POST /api/auth/reset-password
```

> Bu repository gerçek proje repository'si değildir. Dosyalar kapalı ağdaki projeye manuel aktarım amacıyla tutulmaktadır.
