# KLMN — Rol Tanımları (Angular 21 + .NET 10)

Kullanıcı Tanımları ekranındaki minimalist tasarım yaklaşımı örnek alınarak oluşturuldu.

## Ekran adresi ve yetkiler

- Menü: Tanımlamalar → Rol Tanımları
- Route: `/app/roles`
- Listeleme ve Excel: `Roles.Query`
- Oluşturma: `Roles.Create`
- Düzenleme, aktifleştirme, pasifleştirme: `Roles.Update`
- Silme: `Roles.Delete`
- Permission paneli ve kaydetme: `Roles.ManagePermissions`

Angular yetki kontrolü görünürlük içindir; API endpoint'leri aynı yetki kodlarıyla korunur.


## CQRS dizin yapısı

Her işlem kendi klasöründe `Command / CommandHandler / CommandValidator`
veya `Query / QueryHandler / QueryValidator` dosyaları ile tutulur.
Bu düzen, `Application/Users` modülündeki yapıyla aynıdır.

```text
KLMN.Application/Roles/
├── Commands/
│   ├── Common/
│   │   └── RoleRules.cs
│   ├── CreateRole/
│   │   ├── CreateRoleCommand.cs
│   │   ├── CreateRoleCommandHandler.cs
│   │   └── CreateRoleCommandValidator.cs
│   ├── UpdateRole/
│   │   ├── UpdateRoleCommand.cs
│   │   ├── UpdateRoleCommandHandler.cs
│   │   └── UpdateRoleCommandValidator.cs
│   ├── ChangeRoleStatus/
│   │   ├── ChangeRoleStatusCommand.cs
│   │   ├── ChangeRoleStatusCommandHandler.cs
│   │   └── ChangeRoleStatusCommandValidator.cs
│   ├── DeleteRole/
│   │   ├── DeleteRoleCommand.cs
│   │   ├── DeleteRoleCommandHandler.cs
│   │   └── DeleteRoleCommandValidator.cs
│   └── UpdateRolePermissions/
│       ├── UpdateRolePermissionsCommand.cs
│       ├── UpdateRolePermissionsCommandHandler.cs
│       └── UpdateRolePermissionsCommandValidator.cs
├── Queries/
│   ├── GetRoles/
│   │   ├── GetRolesQuery.cs
│   │   ├── GetRolesQueryHandler.cs
│   │   └── GetRolesQueryValidator.cs
│   ├── GetRoleById/
│   │   ├── GetRoleByIdQuery.cs
│   │   └── GetRoleByIdQueryHandler.cs
│   └── GetRolePermissionOptions/
│       ├── GetRolePermissionOptionsQuery.cs
│       └── GetRolePermissionOptionsQueryHandler.cs
├── Responses/
│   ├── RoleResponse.cs
│   ├── RoleMutationResponse.cs
│   └── RolePermissionOptionResponse.cs
└── Exceptions/
    ├── RoleNotFoundException.cs
    └── RoleConflictException.cs
```

Eski toplu `RoleCommands.cs`, `RoleCommandHandlers.cs` ve
`RoleValidators.cs` dosyaları silinmiştir. Bu dosyaları yerel projeden de
kaldırmak gerekir; aksi takdirde aynı sınıfların iki kez derlenmesi hatası alınır.
API route, DTO ve kayıt iş kuralları değişmemiştir.

## API sözleşmesi

| İşlem | HTTP | Endpoint |
| --- | --- | --- |
| Rol listesi | GET | `/api/roles` |
| Detay | GET | `/api/roles/{id}` |
| Excel | GET | `/api/roles/export` |
| Yeni rol | POST | `/api/roles` |
| Rol güncelleme | PUT | `/api/roles/{id}` |
| Aktif/pasif | PATCH | `/api/roles/{id}/status` |
| Soft-delete | DELETE | `/api/roles/{id}?version={xmin}` |
| İzin kataloğu | GET | `/api/roles/permission-options` |
| Rol izinleri | PUT | `/api/roles/{id}/permissions` |

Rol kodu otomatik büyük harfe çevrilir, ad ve kod benzersizdir.
Permission güncelleme `permissionIds` (GUID dizisi) kabul eder.
Boş dizi tüm mevcut özel rol izinlerini iptal eder.
`Version` PostgreSQL xmin'dir: eşzamanlı güncellemelerde 409 döner.

## Veri ve güvenlik kuralları

1. `ADMIN` ve `STANDARD_USER` sistem rolleridir; silinemez, pasifleştirilemez, düzenlenemez veya izinleri değiştirilemez.
2. Başka bir kullanıcıya hâlen atanmış rol silinemez. Bu durumda 409 döner. Pasifleştirme ayrı bir işlemdir.
3. Yetki ilişkileri aktiflik + soft-delete uygulanarak korunur. Daha önce kaldırılmış ilişki tekrar seçilirse aynı kayıt yeniden etkinleştirilir.
4. Örnek özel roller boş veritabanında yalnızca ilk kurulumda seed edilir. Sonrasında kullanıcı tarafından düzenlenen/silinen özel roller startup sırasında değiştirilmez veya yeniden oluşturulmaz.
5. `RolePermissions` değiştirilirken rol `UpdatedDate` de güncellenir; böylece `xmin` permission sayfasındaki concurrency kontrolünde de değişir.
6. Rol seçimi kullanıcıya atanmış rolü doğrudan değiştirmez. Kullanıcıya rol atama ekranı ayrı geliştirilecektir.

## Manuel kabul testleri (önerilen sıra)

1. Admin ile giriş → **Rol Tanımları** menü ve `/app/roles` açılışı.
2. Liste: sayfalama, arama, pasifleri dahil et, aktif/sistem filtresi, Excel.
3. Özel rol ekle: **Deneme Rolü** / `DENEME_ROLU`; aynı ad/kodla tekrar ekleme 409 olmalı.
4. Rolü düzenle; başka tarayıcıdan aynı versiyonla kaydetme 409 olmalı.
5. Permission anahtar butonu → modüle göre seç → kaydet → sayfayı yenile → izinler korunmalı.
6. Bir yetkiyi kaldır ve tekrar ekle; benzersiz index hatası alınmamalı.
7. Rolü pasifleştir ve yeniden aktifleştir; liste filtreleri doğru çalışmalı.
8. Atanmış rolü silmeye çalış: backend 409 dönmeli.
9. ADMIN ve STANDARD_USER için değiştir/sil/pasifleştir yetkileri görünmemeli; API'den de değiştirilememeli.
10. API yeniden başlatıldığında özel rol adı/durumu/izinleri korunmalı.

## Kurulum

Backend ve frontend kaynak dosyalarını ilgili klasörlerine kopyalayın.
Yeni entity veya tablo eklenmediği için migration gerekmez.
.NET API ve Angular uygulamasını yeniden başlatın.
Geliştirme ortamında Angular HTTPS, API HTTPS kullanmaya devam edin.

> Kaynak dosyaları statik olarak denetlendi. Bu çalışma ortamında .NET SDK yoktu;
> `dotnet build`, `npm run build:dev` ve uçtan uca tarayıcı testi ayrıca yapılmalıdır.
