# Rapor Türü, Rütbe Grubu ve Rütbe Tanımları — Kurulum ve Test

## Eski MSKBS modelleri ve taşınan alanlar

Fotoğraflarda görünen iş alanları yeni tablolarda yalnızca aşağıdaki gibidir:

| Eski entity | Yeni entity | Özgün alanlar |
| --- | --- | --- |
| ReportTypeDef | ReportTypeDef | Code (string), Name (string) |
| RankGroupDef | RankGroupDef | Code (string), Name (string), ShortName (string?) |
| RankDef | RankDef | Code (string), Name (string), ShortName (string?), Type (int), RankGroup (Guid?) |

ObjectId, CreationDate/CreatedBy, LastUpdate/UpdatedBy, IsActive,
ConcurrencyToken gibi ortak alanlar `BaseEntity` üzerinden;
StartDate/EndDate, `EffectiveDatedEntity.ValidFrom/ValidTo` üzerinden
gelir. **Type için uydurma enum veya dropdown seçenekleri oluşturulmadı.**
Rütbe düzenleme formunda doğrudan int sayı alanıdır.

## İlişki ve kurallar

`RankDef.RankGroupId` nullable Guid ve `RankGroupDef` FK.
EF `DeleteBehavior.Restrict` ile doğrudan silme korunur.
Yeni grup seçimi aktif/silinmemiş grup gerektirir.
Düzenlenen kayıtta değişmeyen eski pasif grup seçimi korunabilir.
Aktif rütbeler bağlıyken grubu pasifleştirmek, silinmemiş rütbeler
bağlıyken grubu soft-delete yapmak engellenir. Rütbe aktifleştirilirken
ilişkili grup aktif olmalıdır.

Üç entity için ayrı EF configuration ve `KLMNDbContext`/`IKLMNDbContext`
DbSet, filtered unique Code index; her modülde ayrı
Create/Update/Delete/ChangeStatus Command, Handler, Validator ve liste/detay
Query, Handler, Validator bulunmaktadır. `Ranks` modülünün kendi
`GetRankOptions` sorgusu ve `GET /api/ranks/options` API'si vardır;
Rütbe Tanımları ekranının açılması için ayrıca RankGroups.View izni
gerekmez. Eski seçili pasif grup için options API seçimi destekler.

API: `/api/report-types`, `/api/rank-groups`, `/api/ranks`.
Her modülde GET liste/by-id, POST, PUT, PATCH status, DELETE?version,
GET export. Excel `*.Query` yetkisiyle en çok 5000 kayıt indirir.

PermissionCodes: `ReportTypes.*`, `RankGroups.*`, `Ranks.*` (View,
Query, Create, Update, Delete) Angular/backend/seed katmanlarında eşleşir.
Audit whitelist ve GetAuditLogsQueryValidator bu üç entity türünü kapsar.
Rütbenin `RankGroupId` değişikliği audit geçmişinde
`RankGroupDef` üzerinden ad/kod ile çözümlenir.

## Angular 21 + PrimeNG tanım ekranları

- Tanımlamalar → Rapor Türü Tanımları: `/app/report-types`.
- Tanımlamalar → Rütbe Grubu Tanımları: `/app/rank-groups`.
- Tanımlamalar → Rütbe Tanımları: `/app/ranks`.

Üçünde de önceki Branş ekranının sade tasarım/CRUD düzeni kullanılır:
Arama, Pasifleri dahil et, Durum kolon filtresi, p-table frontend
kolon sıralaması, kullanıcı tarafından sütun genişliği sürükleme,
yatay scroll, sabit İşlemler, Excel, popup, durum değişikliği,
soft-delete ve işlem geçmişi. Backend'de fazladan filtreleme mekanizması
veya FilterOptionsQuery oluşturulmadı.

Rütbe formunda Code/Name/ShortName/Type/RankGroupId ve geçerlilik
tarihleri bulunur. Grubu dropdown ile seçilir ve nullable olabilir.

## Migration: henüz oluşturulmadı/uygulanmadı

GitHub deposunda EF ModelSnapshot/Migrations zinciri yoktur;
yapılandırma gerçek local bağlamda ve mevcut migration geçmişini
koruyarak oluşturulmalıdır:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddRankAndReportDefinitionTables --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# SQL script doğrulandıktan sonra yalnız TEST veritabanına uygulayın.
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

Beklenen 3 yeni tablo: `ReportTypeDefs`, `RankGroupDefs`, `RankDefs`.
RankDefs `RankGroupId` nullable FK ve Restrict. `xmin` için ikinci
Version kolonu oluşturulmamalıdır.

## Test komutları

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/ReportTypes.Regression/ReportTypes.Regression.csproj
dotnet run --project KLMN/Tests/RankGroups.Regression/RankGroups.Regression.csproj
dotnet run --project KLMN/Tests/Ranks.Regression/Ranks.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Manuel senaryolar: üç modül CRUD, isActive, kod tekilliği, concurrency
409, tarih UTC, Excel, izinler, audit, Rütbe Grubu FK seçimi/silme/
pasifleştirme koruması, 3 formun mobil görünümü, kolon resize.
Bu ortamda dotnet/npm derlemesi, migration ve browser/E2E
testleri çalıştırılmadı. Kaynak kontrolleri ayrıca yapılacaktır.
