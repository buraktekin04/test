# Askerî Sınıf Tanımları — KLMN

## Alan kararları
Eski MilitaryClassDef fotoğrafında görülen Type, MpvtCode ve Town
alanları kullanıcı isteği üzerine bilinçli olarak ALINMADI.
Yorum satırındaki MilitaryClassGroup da alınmadı.
Yeni MilitaryClassDef : EffectiveDatedEntity:
Code (zorunlu), Name (zorunlu), ShortName?, ExternalCode?,
bool? Active. StartDate/EndDate yerine ortak nullable UTC
ValidFrom/ValidTo kullanılır. BaseEntity.Id, CreatedDate,
CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted ve uint Version
(PostgreSQL xmin) tekrar tanımlanmaz.

Eski bool? Active, kayıt yönetimindeki BaseEntity.IsActive
ile aynı şey DEĞİLDİR. Active için Belirtilmedi/Evet/Hayır,
IsActive için ayrı durum değiştirme işlemi vardır.
Kodlar büyük harfle saklanır; silinmemiş kayıtlarda tekildir.

## Modül
GET /api/military-classes: search, includeInactive, isActive, active,
pageNumber/pageSize. Arama/sayfalama backend'de.
GET /api/military-classes/{id}: detay ve xmin.
POST /api/military-classes: Create.
PUT /api/military-classes/{id}: Update + xmin.
PATCH /api/military-classes/{id}/status: IsActive + xmin.
DELETE /api/military-classes/{id}?version=...: soft-delete.
GET /api/military-classes/export: en fazla 5000 kayıt XLSX.

Askerî Sınıf Tanımları ekranı /app/military-classes.
Liste, kolon filtreleri, yerel p-table sorting, form alan hataları,
audit, Excel ve işlem bazlı permission kontrolleri bulunur.
MilitaryClasses.View/Query/Create/Update/Delete kodları Domain,
IdentitySeedData, API ve Angular'da eşleştirilmiştir.
Sadece Create izni ekranı açar ama Update/Delete/Excel vermez.
Excel yalnız Query yetkisiyle açılır.
Geçerlilik tarihleri Angular'da ValidityPeriodUtils ile UTC'ye döner.

## Migration
GitHub'da gerçek yerel EF migrations/ModelSnapshot zinciri YOKTUR.
Canlı tabloları silmeyin; mevcut migration geçmişi üzerinden:
  cd KLMN/Backend
  dotnet ef migrations add AddMilitaryClassDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
SQL'de MilitaryClassDefs tablosu, Code partial unique index,
nullable ShortName/ExternalCode/Active, nullable UTC timestamptz
ValidFrom/ValidTo olmalı. Type/MpvtCode/Town FK veya kolon olarak
bulunmamalı. Version için normal sütun EKLEME; Npgsql xmin kullan.

## Test ve kabul
dotnet run --project KLMN/Tests/MilitaryClasses.Regression/MilitaryClasses.Regression.csproj
dotnet build KLMN/Backend/KLMN.slnx
npm --prefix KLMN/Frontend run build
.NET SDK burada yok; gerçek build/DB/E2E testleri çalıştırılmadı.
Test veri setinde Active=null/true/false, IsActive bağımsızlığı,
UTC tarih sınırları, 409 çakışması, yetki modülleri ve
soft-delete/unique kod davranışı ayrıca doğrulanmalıdır.
