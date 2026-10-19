# Bağlı Kurul Karar Tanımları — Teknik Kurulum ve Kabul Notları

## Eski modelden KLMN'ye taşınan alanlar

Eski kaynak: `MSKBSModel.HcConnectedDecisionDef`. Fotoğrafta iki modele
özgü alan vardır: `Code` ve `Name`. Başka ilişki, kategori veya seçim
alanı **tanımlanmadı**.

| Eski alan | Yeni KLMN karşılığı |
|---|---|
| `ObjectId : Guid` | `BaseEntity.Id` |
| `Code : string` | `HcConnectedDecisionDef.Code` (en çok 100 karakter, zorunlu) |
| `Name : string` | `HcConnectedDecisionDef.Name` (en çok 250 karakter, zorunlu) |
| `IsActive : bool` | `BaseEntity.IsActive` |
| `CreationDate / CreatedBy` | `BaseEntity.CreatedDate / CreatedBy` |
| `LastUpdate / UpdatedBy` | `BaseEntity.UpdatedDate / UpdatedBy` |
| `ConcurrencyToken` | `BaseEntity.Version` (PostgreSQL `xmin`) |
| `StartDate / EndDate` | `EffectiveDatedEntity.ValidFrom / ValidTo` (nullable UTC) |

Yeni entity `KLMN.Domain.Entities.Admissions.HcConnectedDecisionDef`,
`EffectiveDatedEntity`'den kalıtım alır. Ayrı bir `Active`,
`CreationDate`, `ConcurrencyToken` veya `RelatedCity` alanı yoktur.

## Backend

- Tablo: `HcConnectedDecisionDefs` (EF Core configuration ve
  `KLMNDbContext` / `IKLMNDbContext`).
- Silinmemiş kayıtlar arasında `Code` unique index; kod biçimi ve
  ad/UTC dönem doğrulamaları FluentValidation ile sağlanır.
- Her işlem ayrı CQRS dizininde Command + Handler + Validator:
  `CreateHcConnectedDecisionDef`, `UpdateHcConnectedDecisionDef`,
  `DeleteHcConnectedDecisionDef`, `ChangeStatusHcConnectedDecisionDef`.
- Liste ve detay sorguları: `GetHcConnectedDecisionDefs`,
  `GetHcConnectedDecisionDefById`.
- Concurrency: `Version` kontrolü, başarılı kayıttan sonra yeni xmin.
  Silmede mevcut soft-delete interceptor'ı çalışır.
- İzin kodları: `ConnectedDecisions.View/Query/Create/Update/Delete`;
  IdentitySeedData, frontend PermissionCodes, route ve backend API
  yetkilendirmesi birbiriyle eşlenmiştir.
- Audit whitelist: `Code`, `Name`, `ValidFrom`, `ValidTo`,
  `IsActive`. Genel audit filtre türü `HcConnectedDecisionDef`.
- Conflict/NotFound API hataları mevcut ProblemDetails altyapısında
  409/404 olarak döner.

API:
`GET /api/connected-decisions` (search, includeInactive, isActive,
pageNumber, pageSize), `GET /api/connected-decisions/{id}`,
`POST`, `PUT /{id}`, `PATCH /{id}/status`,
`DELETE /{id}?version=<xmin>`,
`GET /api/connected-decisions/export` (Query izni, en fazla 5000 Excel satırı).

## Angular 21 / PrimeNG

`/app/connected-decisions`, **Tanımlamalar → Bağlı Kurul Karar Tanımları**.

Ekran **Askerî Sınıf Tanımları** ile aynı onaylı şablondadır:
arama, Pasifleri dahil et, kolon durumu filtresi
(Tümü / Aktif / Pasif), filtreleri uygula/temizle, frontend
`p-table` sütun sıralaması, backend sayfalama, Excel,
yeni kayıt/düzenleme popup'ı, aktif/pasif, silme, audit geçmişi
ve ayrı işlem izinleri.

- Giriş alanları sadece **Kod, Karar Adı, Geçerli Başlangıç ve Bitiş**.
- Aktif/pasif form alanı olarak eklenmez; liste eyleminden yönetilir.
- Ek sorgu filtreleme katmanı, `FilterOptionsQuery`, tablo ilişkisi
  veya DTO alanı eklenmez.

## Migration — henüz oluşturulmadı / uygulanmadı

GitHub çalışma kopyasında güncel EF Core migration snapshot zincirinin
varlığı doğrulanamadığı için mevcut migration geçmişini bozan veya
başlangıç şeması oluşturan *uydurma migration* eklenmedi.
Veritabanına bağlantısı olan gerçek ortamda:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddConnectedDecisionDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# Script ve schema değişikliğini doğruladıktan sonra önce TEST veritabanında:
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Beklenen yeni tablo: `HcConnectedDecisionDefs`, Code/Name,
nullable UTC ValidFrom/ValidTo, BaseEntity ortak kolonları,
soft-delete unique Code index. `Version` için PostgreSQL `xmin`
kullanılır, ikinci sürüm kolonu oluşturulmaz.

## Derleme ve kabul kontrolleri

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/ConnectedDecisions.Regression/ConnectedDecisions.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Manuel test: code duplicate 409, geçersiz tarih, concurrent edit,
aktif/pasif, soft-delete, permission 403, Excel, audit, mobil
ve masaüstü form. Kaynak düzeyinde statik kontroller yapılmış olsa
da gerçek .NET/Angular build, PostgreSQL migration ve tarayıcı
entegrasyon testleri burada çalıştırılmadı.
