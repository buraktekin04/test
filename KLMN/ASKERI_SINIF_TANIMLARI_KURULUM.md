# Askerî Sınıf Tanımları — Güncel Mimari ve Kurulum

## Entity ve alan kararları
Yeni MilitaryClassDef : EffectiveDatedEntity; kendine özgü alanları
yalnızca Code, Name, ShortName? ve ExternalCode? olarak tutulur.
Kullanılmayan Type, MpvtCode, Town ve yorumdaki MilitaryClassGroup
ALINMAZ. StartDate/EndDate tekrar yazılmaz; nullable/UTC
ValidFrom ve ValidTo ortak EffectiveDatedEntity'den miras alınır.

Aktif/pasif DURUMU yalnızca BaseEntity.IsActive ile yönetilir.
Önceki sürümde yanlışlıkla eklenen bool? Active kaldırılmıştır.
Create/Update formunda aktiflik input'u YOKTUR. Kullanıcı listede
Aktifleştir/Pasifleştir butonlarıyla ayrı ChangeStatusMilitaryClassDef
komutunu çağırır (MilitaryClasses.Update). Soft-delete, CreatedDate,
CreatedBy, UpdatedDate, UpdatedBy, IsDeleted ve PostgreSQL xmin Version
BaseEntity mekanizmasına dahildir; bir kez daha tanımlanmaz.

## Backend / yetki
- CreateMilitaryClassDefCommand, UpdateMilitaryClassDefCommand,
  ChangeStatusMilitaryClassDefCommand, DeleteMilitaryClassDefCommand
  farklı dosyalarda Handler ve FluentValidation ile düzenlenmiştir.
- Liste GetMilitaryClassDefsQuery, detay GetMilitaryClassDefByIdQuery.
- GET /api/military-classes?search=&includeInactive=&isActive=&pageNumber=&pageSize=
- GET /api/military-classes/{id}
- POST /api/military-classes
- PUT /api/military-classes/{id}
- PATCH /api/military-classes/{id}/status
- DELETE /api/military-classes/{id}?version=...
- GET /api/military-classes/export (maksimum 5000 satır; Query izni)

MilitaryClasses.View/Query/Create/Update/Delete yetkileri korunur.
Herhangi bir izin liste/menü/route erişimini sağlar. Ekleme yalnız
Create; düzenleme ve durum değişimi yalnız Update; silme Delete;
Excel aktarma Query izni gerektirir. Kodlar silinmemiş kayıtlarda
tekildir. Update/status/delete uint xmin çakışması 409,
silme soft-delete ve audit interceptor üzerinden gerçekleştirilir.

## Angular
Menü: Tanımlamalar > Askerî Sınıf Tanımları.
Route: /app/military-classes.
Form alanları: Kod, Ad, Kısa Ad, Harici Kod, Geçerli Başlangıç/Bitiş.
Durum seçimi ekleme/düzenleme popup'ında YOKTUR.
Liste: tek IsActive Durum kolonu/filtresi; butondan aktif/pasif.
Arama, durum filtresi ve sayfalama backend'de; PrimeNG kolon
sıralaması sadece yüklü sayfada frontend'de çalışır.
Form hataları alanın altında ve kaydetme bölümünde gösterilir.
Excel ve audit korunur. Tarihler ValidityPeriodUtils ile UTC'ye döner.

## Migration / Veritabanı
GitHub kaynaklarında mevcut gerçek yerel EF Migrations/ModelSnapshot
bulunmuyor. Bu commit'te veritabanına müdahale edilmedi.
Henüz tablolar oluşturulmadıysa güncel entity ile
MilitaryClassDefs tablosunu mevcut migration zincirinden oluşturun.
Sütunlar: BaseEntity ve ortak ValidFrom/ValidTo ile Code,
Name, ShortName?, ExternalCode?. Ayrı Active sütunu YOKTUR.
Eğer önceki sürümün migration'ı DB'ye zaten uygulanmışsa
geçmiş migration'ları silmek yerine yeni bir migration ile
sadece gereksiz Active kolonunun kaldırılmasını değerlendirin.
Her durumda SQL script'ini inceleyin ve veri kaybı riskini
kontrol edin; canlı tabloyu sıfırlamayın.
  cd KLMN/Backend
  dotnet ef migrations add RemoveMilitaryClassDuplicateActive --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
  dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
Migration adı gereksinime göre değiştirilebilir. PostgreSQL xmin için
normal Version kolonu açılmaz.

## Test ve kabul
  dotnet run --project KLMN/Tests/MilitaryClasses.Regression/MilitaryClasses.Regression.csproj
  dotnet build KLMN/Backend/KLMN.slnx
  npm --prefix KLMN/Frontend run build
Regresyon kaynakları yalnızca IsActive DTO projeksiyonunu test eder;
status/delete/integration için gerçek DB testleri ayrıca gereklidir.
Bu işlemde .NET ve Angular derlemesi veya gerçek PostgreSQL/browser
testleri çalıştırılmamıştır. Aktif/pasif, permission (Create-only,
Update-only, Query-only), concurrency, Excel ve soft-delete kabul
testleri lokal geliştirme ortamında çalıştırılmalıdır.
