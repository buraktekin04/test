# KLMN Backend - Güncel Aktarım Paketi

Bu klasör kapalı ağdaki gerçek KLMN projesine manuel aktarım için tutulur.

## Mimari

```text
KLMN.Api
KLMN.Application
KLMN.Domain
KLMN.Infrastructure
KLMN.Persistence
```

## Güncel authentication akışı

- Access token JWT olarak response body'de döner ve Angular memory state'te tutulur.
- Refresh token yalnızca HttpOnly cookie'de tutulur.
- Refresh token DB'de açık değer olarak değil SHA-256 hash olarak saklanır.
- Refresh rotation ve reuse detection vardır.
- JWT içerisine `security_stamp` claim'i yazılır.
- `OnTokenValidated`, JWT security stamp ile DB'deki güncel `User.SecurityStamp` değerini karşılaştırır.
- Change password, reset password ve logout-all SecurityStamp'i yeniler.
- Change password başarılı olursa bütün refresh token'lar revoke edilir.
- Yanlış mevcut parola `400`; gerçek authentication problemi `401` döner.

### Önemli 401 düzeltmesi

Eski kodda `OnTokenValidated` security stamp claim'ini zorunlu tutarken
`JwtTokenService` access token'a bu claim'i eklemiyordu. Bu durumda
`[Authorize]` action'ları controller'a ulaşmadan 401 oluyordu.

Güncel sürümde token üretilirken:

```csharp
new(CustomClaimTypes.SecurityStamp, user.SecurityStamp)
```

claim'i eklenmektedir.

Bu güncellemeden sonra eski access token'lar kullanılmamalıdır. Tarayıcı session/cookie temizlenip yeniden login olunmalıdır.

## Public auth endpointleri

```text
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/forgot-password
POST /api/auth/reset-password
```

Authenticated endpointler:

```text
POST /api/auth/logout
POST /api/auth/logout-all
GET  /api/auth/me
POST /api/auth/change-password
```

## Secret değerleri

Connection string, JWT secret, initial admin parolası ve SMTP parolası source control'e yazılmamalıdır.
