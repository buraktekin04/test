# KLMN Frontend – Angular 21 + PrimeNG 21

KLMN backend authentication sözleşmeleri ile uyumlu Angular 21 uygulamasıdır.

## Geliştirme
```powershell
cd KLMN/Frontend
npm install
npm start
```

`npm start`, `/api` çağrılarını `proxy.conf.json` dosyasındaki API adresine yönlendirir. API farklı portta çalışıyorsa proxy `target` değerini değiştirin.

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
