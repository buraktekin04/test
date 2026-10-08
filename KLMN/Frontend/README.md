# KLMN Frontend – Angular 21 + PrimeNG 21

KLMN backend authentication sözleşmeleri ile uyumlu Angular 21 uygulamasıdır.

## Geliştirme
```powershell
cd KLMN/Frontend
npm install
npm start
```

Angular CLI **proxy kullanılmıyor**. `npm start`, `https://localhost:4200` adresinde uygulamayı başlatır. Angular'dan API çağrıları doğrudan `src/environments/environment.development.ts` dosyasındaki `https://localhost:7145/api` adresine gider. Backend portu değişirse yalnızca `apiBaseUrl` değerini güncelleyin.

`environment.ts` içindeki `/api`, yalnızca production'da API frontend ile **aynı origin** altında sunuluyorsa geçerlidir; ayrı bir API domain'i kullanılıyorsa tam HTTPS URL yazılmalıdır.

### CORS ve refresh cookie
Angular ile API ayrı portlarda çalıştığından backend geliştirme/test için `KLMN.Api/appsettings.Development.json > Cors:AllowedOrigins`, canlı için `appsettings.json > Cors:AllowedOrigins` bölümünden Angular adreslerini okur ve `AllowCredentials()` kullanır. HttpOnly refresh cookie'nin gönderilebilmesi için Angular ve API'dyi HTTPS ile çalıştırın (farklı şemalar `SameSite=Lax` cookie akışını bozabilir). Local geliştirme sertifikalarının tarayıcı tarafından güvenilir olmasını sağlayın. Backend API'de örnek port `7145` kullanılır; gerçek HTTPS portunu doğrulayın.

İleride proxy istenirse ayrıca yapılandırılabilir; bu sürümde Angular proxy dosyası veya proxy script'i yoktur.

## Yapı
- `core/auth`: Login, refresh, /me, logout, reset/change password, memory-based access token.
- `core/permissions`: Backend ile eşleşen permission sabitleri ve UI guard'ları.
- `features/authentication`: Login / forgot / reset / change password.
- `features/users`: Filtreli, sayfalanmış kullanıcı listesi.
- `layout`: Sidebar, topbar ve uygulama iskeleti.

## Auth güvenliği
Refresh cookie HttpOnly'dir; tarayıcı JavaScript kodu tarafından okunmaz. `AuthService` ve interceptor tek uçuşlu refresh kullanır. Access token localStorage'a yazılmaz. Login veya refresh response'undaki JWT **console'a yazılmaz**. Backend `POST /api/auth/logout` access token süresi dolduğunda da cookie ile logout yapabilir.

## Derleme
```powershell
npm run build
```

Projenin gerçek kurumsal ağ ve API üzerinde çalışması ayrıca doğrulanmalıdır.
