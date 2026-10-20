# Sağlık Tesisi ve Sağlık Tesisi Rapor Türleri — Teknik Kurulum

## MSKBS eski kaynak modelleri

**HospitalDef (Sağlık Tesisi Tanımları)** — Code(string), Name(string), CityDef(Guid?), RefereeHospital(Guid?). Eski ObjectId, CreatedBy, CreationDate, UpdatedBy, LastUpdate, IsActive ve ConcurrencyToken alanları yeni KLMN `BaseEntity`'den; StartDate/EndDate, `EffectiveDatedEntity.ValidFrom/ValidTo` üzerinden gelir. Çift audit/aktif/Version alanı açılmadı.

**HospitalReportType (Tesisin Verebileceği Rapor Türleri)** — HospitalDef(Guid) ve ReportType(Guid) **zorunlu**. Bunun dışında Kod/Ad veya tarih iş alanı yoktur; doğrudan `BaseEntity` kalıtımı kullanılır. Mevcut `ReportTypeDef` tanımına bağlanır.

## Tablolar ve ilişkiler

`HospitalDefs`
- `CityId` nullable FK → mevcut `CityDefs.Id`.
- `RefereeHospitalId` nullable self-FK → `HospitalDefs.Id`. A→B→A, A→B→C→A veya A→A döngüsü engellenir.
- FK'lerde `DeleteBehavior.Restrict`; yeni seçimlerin aktif/silinmemiş olma koşulu. Düzenlemede eski pasif seçim korunabilir.
- Şehir silme/pasifleştirme, ona bağlı tesis varken kısıtlanır.
- Hakem olarak kullanılan tesis ve aktif eşleştirmeleri bulunan tesis pasifleştirilemez; silinmemiş bağlı kayıtları olan tesis silinemez.

`HospitalReportTypes`
- `HospitalDefId` required FK → `HospitalDefs.Id`.
- `ReportTypeDefId` required FK → `ReportTypeDefs.Id`.
- Soft-delete olmayan kayıtlar arasında `(HospitalDefId, ReportTypeDefId)` benzersiz indeks; aynı çift tekrar eklenemez.
- Yeni eşleştirmede seçilen tesis ve rapor türü aktif olmalı; düzenlemede eski pasif seçim korunabilir.
- Eşleştirme yeniden aktifleştirilirken iki ilişkinin aktifliği doğrulanır.
- İlişkili rapor türü silinemez; aktif eşleştirme bağlı rapor türü pasifleştirilemez.
- Her entity için `BaseEntity.Version` PostgreSQL `xmin`, `IsActive`, audit ve soft-delete ortaktır.

## Mimari ve ekranlar

Her modül bağımsız Create/Update/Delete/ChangeStatus CQRS Command/Handler/Validator; GetList/GetById Query/Handler/Validator ve DTO içerir. Seçimler için, başka ekranın izni gerekmemesi amacıyla modülün kendi permission koduna tabi `GET /api/hospitals/options` ve `GET /api/hospital-report-types/options` uçları bulunur.

- **Tanımlamalar → Sağlık Tesisi Tanımları:** `/app/hospitals`, Code, Name, şehir dropdown, hakem sağlık tesisi dropdown, geçerlilik başlangıç/bitiş. Excel, p-table filtre/sort/resize, audit, CRUD, aktif/pasif, silme.
- **Tanımlamalar → Sağlık Tesisi Rapor Türleri:** `/app/hospital-report-types`. Sağlık Tesisi seç, Rapor Türü seç; mevcut eşleştirmeleri tabloda görüntüle. Aynı standarda sahip liste, Excel, CRUD, status ve işlem geçmişi.

Backend izinler: `Hospitals.View/Query/Create/Update/Delete`, `HospitalReportTypes.View/Query/Create/Update/Delete`. Angular route/menu, IdentitySeedData ve API permission karşılıkları eklendi. Audit whitelisti, sorgu tür filtresi ve şehir/tesis/rapor türü GUID adlandırması güncellendi.

## Migration

`HospitalDefs` ve `HospitalReportTypes` için gerçek EF migration gerekir. GitHub'da güncel EF ModelSnapshot/migration zinciri yok; uydurma migration üretilmedi. Mevcut yerel geçmiş üzerinden:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddHospitalAndHospitalReportTypes --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# SQL incelemesi ve yalnız test veritabanında:
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

İki tablo, `CityId`/self-FK ve rapor türü FK'leri, soft-delete filtered unique pair, ortak BaseEntity + `xmin` beklenir. Mevcut CityDef ve ReportTypeDef fiziksel tabloları yeniden oluşturulmamalıdır.

## Derleme / kabul

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/Hospitals.Regression/Hospitals.Regression.csproj
dotnet run --project KLMN/Tests/HospitalReportTypes.Regression/HospitalReportTypes.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Manuel kabul: tesis/hakem/şehir FK, self-ref döngü, seçili pasif dropdown, unique eşleştirme (409), CRUD/xmin, 403/404/409, şehir ve rapor türü silme/pasifleştirme, audit adları, Excel, mobil dropdown, p-table resize ve yatay kaydırma. Gerçek build, DB migration ve uçtan uca tarayıcı testleri **bu ortamda yürütülmedi**.
