# Askerî Okul Tanımları — Teknik Kurulum ve Test

## Eski modele göre alanlar
Eski MSKBS MilitarySchoolDef fotoğrafında Code, Name, nullable Guid
CityDef ve ForcesCommandDef, nullable StartDate/EndDate, ObjectId,
CreationDate, CreatedBy, LastUpdate, UpdatedBy, ConcurrencyToken
ve IsActive vardır.

KLMN: MilitarySchoolDef : EffectiveDatedEntity : BaseEntity.
Kendi alanları: Code, Name, Guid? CityId, CityDef? City,
Guid? ForcesCommandDefId, ForcesCommandDef? ForcesCommand.
ValidFrom ve ValidTo nullable/UTC, ortak EffectiveDatedEntity'den
gelir; başlangıç dahil, bitiş hariç. Audit, soft-delete, IsActive ve
uint Version (PostgreSQL xmin) BaseEntity'den gelir.
StartDate/EndDate ve ayrıca Version sütunu OLUŞTURMA.

## İlişki ve veri bütünlüğü
CityDef.MilitarySchools ve ForcesCommandDef.MilitarySchools
koleksiyonları oluşturuldu. İki nullable FK için Restrict tanımlı.
Yeni okul şehir/kuvvet bağlantıları aktif ve silinmemiş tanımlara
yapılabilir. Eski seçili pasif bağlantı değiştirilmeden
güncellenebilir; okulun yeniden aktifleştirilmesi iki bağlı
referansın da aktif olmasını gerektirir.
Bağlı silinmemiş okullar varken şehir/kuvvet soft-delete engellenir,
aktif okul bulunuyorsa şehir/kuvvet pasifleştirilemez.
Okul kodları silinmemiş okullarda büyük-küçük harf duyarsız tekildir.

## CQRS, HTTP ve izin
Her CreateMilitarySchoolDef, UpdateMilitarySchoolDef,
ChangeStatusMilitarySchoolDef ve DeleteMilitarySchoolDef için ayrı
Command/Handler/Validator dosyaları bulunur. GetMilitarySchoolDefs,
GetMilitarySchoolDefById, GetMilitarySchoolOptions ayrı sorgulardır.
- GET /api/military-schools: search, includeInactive, isActive,
  cityId, forcesCommandDefId, pageNumber, pageSize.
- GET /api/military-schools/{id}: güncel Version dahil detay.
- GET /api/military-schools/options?selectedCityId=...&
  selectedForcesCommandDefId=...: aktif şehir/kuvvet seçenekleri
  ve düzenlenen okulun seçili pasif referansları.
- POST, PUT, PATCH {id}/status, DELETE {id}?version=...
- GET /api/military-schools/export: Query izniyle Excel (max 5000).

MilitarySchools.View/Query/Create/Update/Delete hem Domain,
Angular hem IdentitySeedData'dadır. Herhangi bir modül izni ekran,
route ve liste okumaya izin verir; yalnızca Create oluşturur,
Update düzenler/durum değiştirir, Delete soft-delete, Query Excel.
Dropdown'lar ayrı Cities/ForcesCommands permission'ı istemez;
yalnızca okul iznini gerektirir, bağlı tabloların yönetim hakkını vermez.

## Angular
Tanımlamalar -> Askerî Okul Tanımları: /app/military-schools.
PrimeNG p-table, kolon sıralaması sadece yüklü sayfada; arama,
şehir/kuvvet/durum filtreleri ve sayfalama backend'de.
Reaktif form: Kod, Ad, Şehir, Kuvvet Komutanlığı,
Geçerli Başlangıç/Bitiş. Boş tarih kabul edilir, dolu tarihler
ValidityPeriodUtils ile UTC'ye çevrilir.
Alan bazlı validasyon ve kayıt bölümünde hata, Excel, audit,
aktiflik ve silme onayları vardır.

## Migration — ÖNEMLİ
GitHub reposunda mevcut yerel EF Core Migrations/ModelSnapshot yok.
Gerçek DB zincirini koruyun; önceden uygulanmış migration'ı silmeyin
ve canlı tabloları yeniden oluşturmayın. Yerel migration zincirinde
Cities ve ForcesCommands tabloları hazır olmalıdır:
  cd KLMN/Backend
  dotnet ef migrations add AddMilitarySchoolDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext

Script: MilitarySchoolDefs tablosu, iki nullable Restrict FK,
Code için !IsDeleted unique index, CityId/ForcesCommandDefId
index, nullable UTC timestamptz ValidFrom/ValidTo alanları.
Normal Version kolonu yerine PostgreSQL xmin mapping korunsun.
Legacy CityDef ve ForcesCommandDef GUID eşleşmelerini varsayıma
dayalı taşımayın. Migration bu commit'te üretilip uygulanmadı.

## Test ve kabul
Regresyon kaynağı:
  dotnet run --project KLMN/Tests/MilitarySchools.Regression/MilitarySchools.Regression.csproj
  dotnet build KLMN/Backend/KLMN.slnx
  npm --prefix KLMN/Frontend run build
Kaynak ve API/UI sözleşmelerinin statik kontrolü yapılır;
bu commit sırasında gerçek dotnet/npm build, PostgreSQL migration,
regresyon koşturma ve tarayıcı E2E yapılmış sayılmaz.
Ayrıca test: Yalnızca Create/View erişimi, 403 Excel, eşzamanlı
409, nullable FK, pasif referans, silme/pasifleştirme, UTC tarih.
