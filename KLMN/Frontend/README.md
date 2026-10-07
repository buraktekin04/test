# KLMN Frontend Aktarım Paketi

Kapalı ağdaki KLMN Angular 21 projesine manuel aktarım için hazırlanmıştır.

## Yapı

```text
src/
├── app/
│   ├── core/
│   │   ├── auth/{models,services,guards,interceptors}
│   │   ├── config
│   │   ├── models
│   │   └── permissions/{constants,services,guards}
│   ├── features/
│   │   ├── authentication/{login,forgot-password,reset-password,change-password}
│   │   ├── home
│   │   └── users/{models,services,user-list}
│   ├── layout/{models,services,sidebar,topbar,app-layout}
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
├── environments/
│   ├── environment.ts
│   └── environment.development.ts
├── main.ts
└── styles.scss
```

Development için proxy:
```text
/api -> https://localhost:7145
```

Çalıştırma:
```powershell
npx ng serve --proxy-config proxy.conf.json
```
