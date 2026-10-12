# Kullanıcı Görev Yeri (Organizasyon) Seçimi — Kurulum

## İş kuralı

**Tanımlamalar > Kullanıcı Tanımları** ekranında yeni kullanıcı açarken
ve kullanıcı düzenlerken `Görev Yeri / Organizasyon` seçicisi vardır.

- Kaynak: `OrganizationUnits` tablosundaki aktif ve silinmemiş birimler.
- Alan: zaten Users tablosunda bulunan `OrganizationUnitId` nullable GUID foreign key.
- Yeni tablo, kolon, indeks veya migration **yoktur**.
- Seçim isteğe bağlıdır; mevcut kullanıcı kayıtlarını geriye dönük bozmaz.
- Listede arama yapılabilir; kod ve ad birlikte görünür.
- Var olan kullanıcının birimi sonradan pasifleşmişse önceki ataması
  etikette gösterilir. Mevcut atamayı korumak mümkündür ancak pasif birim
  yeni bir kullanıcıya veya başka kullanıcıya yeniden atanamaz.
- API, istemciden gelen GUID'in aktif ve silinmemiş birime ait
  olduğunu kontrol eder; geçersiz atamayı 409 Conflict ile reddeder.
- Kullanıcı listesinde halen `organizationUnitName` ve
  `organizationUnitCode` görüntülenir.

## CQRS / API

Yeni endpoint: `GET /api/users/organization-options`.

Yetki: `Users.Query`. Yanıt: `OrganizationUnitOptionResponse[]`
(`id`, `code`, `name`, `parentOrganizationUnitId`).

Yeni sınıflar:
- `KLMN.Application/Users/Queries/GetOrganizationUnitOptions/GetOrganizationUnitOptionsQuery.cs`
- `KLMN.Application/Users/Queries/GetOrganizationUnitOptions/GetOrganizationUnitOptionsQueryHandler.cs`
- `KLMN.Application/Users/Responses/OrganizationUnitOptionResponse.cs`
- `Frontend/src/app/features/users/models/organization-unit-option.model.ts`

Kullanıcı oluşturma/güncelleme HTTP body içinde `organizationUnitId` alanı
yer alır; null gönderilmesi mevcut isteğe bağlı FK ile uyumludur.

## Kopyalanacak mevcut dosyalar

**Backend** (`KLMN/Backend` köküne göre):
- `KLMN.Api/Controllers/UsersController.cs`
- `KLMN.Application/Users/Commands/CreateUser/CreateUserCommand.cs`
- `KLMN.Application/Users/Commands/CreateUser/CreateUserCommandHandler.cs`
- `KLMN.Application/Users/Commands/CreateUser/CreateUserCommandValidator.cs`
- `KLMN.Application/Users/Commands/UpdateUser/UpdateUserCommand.cs`
- `KLMN.Application/Users/Commands/UpdateUser/UpdateUserCommandHandler.cs`
- `KLMN.Application/Users/Commands/UpdateUser/UpdateUserCommandValidator.cs`

**Frontend** (`KLMN/Frontend/src/app` köküne göre):
- `features/users/models/create-user-request.model.ts`
- `features/users/models/update-user-request.model.ts`
- `features/users/services/user.service.ts`
- `features/users/user-list/user-list.component.ts`
- `features/users/user-list/user-list.component.html`
- `features/users/user-list/user-list.component.scss`

Mevcut `User` ve `OrganizationUnit` domain entity'lerini, migration
dosyalarını veya auth/refresh mekanizmasını değiştirmeye gerek yoktur.

## Roller, yetkiler ve kullanıcı tipi

Bu geliştirmede kullanıcı formuna rol, kullanıcı tipi veya ayrı permission
seçici eklenmedi. Mevcut `CreateUserCommandHandler` yeni kullanıcıya
otomatik `STANDARD_USER` rolü atamaya devam eder.

- **Rol Tanımları:** Rol oluşturma ve rolün izinlerini tanımlama.
- **Kullanıcı Rol Atamaları:** Kullanıcının sahip olduğu rolleri
  `Users.AssignRole` yetkisiyle yönetme (ayrı ekran/iş akışı).
- **Kullanıcı Tanımları:** Kimlik, iletişim, görev yeri, mesai izni,
  erişim tarihleri ve PDF belgeleri.

Rol Tanımları ve Kullanıcı Rol Atamaları ekranları bu commit'in kapsamı
dışındadır; henüz hazır oldukları varsayılmamalıdır.

## Kontrol adımları

1. `dotnet build` ve `npm run build` çalıştırın.
2. `GET /api/users/organization-options` 200 ve aktif birimleri dönmeli.
3. Birim yoksa seçici boş kalır; tabloda veri varmış gibi sahte birim eklenmez.
4. Kullanıcı oluştururken görev yeri seçin ve `Users.OrganizationUnitId`
   alanının seçilen GUID olarak kaydedildiğini kontrol edin.
5. Kullanıcı düzenlerken mevcut görev yeri seçili gelmeli; değişiklik
   sonrası liste ve detay sorgularında yeni birim görülmeli.
6. Geçersiz veya pasif GUID için API 409 dönmeli.
7. Rol/permission ve otomatik STANDARD_USER davranışları değişmemeli.

Bu çalışma yalnızca GitHub kaynağını günceller; yerel MSKBS proje
dosyalarına veya PostgreSQL'e doğrudan müdahale edilmez.
