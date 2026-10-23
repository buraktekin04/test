# ICD10 Tanımları ve ICD10 SYY Tanı İstatistik Tanımları

**Tarih:** 23.10.2026

## Kullanıcı fotoğraflarındaki legacy sınıflar

| Legacy model | Ekran adı | Yeni tablo |
|---|---|---|
| `ICD10Def` | ICD10 Tanımları | `ICD10Defs` |
| `ICD10Diagnostic` | ICD10 SYY Tanı İstatistik Tanımları | `ICD10Diagnostics` |

### ICD10Def

- `Code`: `string`; 32 karakter, zorunlu ve uppercase.
- `Name`: `string`; 500 karakter, zorunlu.
- Ortak `BaseEntity`: `Id`, `CreatedDate`, `CreatedBy`, `UpdatedDate`,
  `UpdatedBy`, `IsActive`, `IsDeleted`, `Version` (PostgreSQL `xmin`).
- Aktif ve pasif silinmemiş kodlar arasında unique index; soft-delete
  edilen kodun yeniden kullanımı mümkündür.
- Eski modelde tarih alanı olmadığı için `EffectiveDatedEntity` kullanılmadı.

### ICD10Diagnostic

| Alan | Tip | Kaynak |
|---|---|---|
| `ICD10DefId` | Guid | `ICD10Defs.Id` |
| `SYYDiagnosisDefId` | Guid | `SYYDiagnosisDefs.Id` |
| `HcAdmissionTypeDefId` | Guid | `HcAdmissionTypeDefs.Id` |
| `Amount` | int | 0 ve üzeri tam sayı |

- Üç FK **Restrict** ve bağımsız lookup API seçenekleri.
- Silinmemiş satırlarda üçlü benzersiz index.
- `Amount >= 0` için uygulama doğrulaması ve PostgreSQL check constraint.
- Seçili pasif ilişkiler düzenlemede korunur, yeni pasif referans seçilemez.
- ICD10, SYY Tanısı, Başvuru Grubu ile ilgili mevcut silme/durum
  command'lerinde aktif/pasif ve silinmiş bağımlılık kontrolleri.
- Her değişiklik mevcut audit ve optimistic concurrency akışına girer.

## Angular: birbirinden tamamen ayrı iki ekran

- `features/icd10-defs/icd10-def-list/`, `models/`, `services/`
- `features/icd10-diagnostics/icd10-diagnostic-list/`, `models/`, `services/`

İkisi için ayrı `.component.ts/.html/.scss`, model ve servis mevcuttur.
Listeleme: API search + status + pagination; kolon başlığı PrimeNG client sort,
resize, wrap; duruma geçiş / silme confirm popup; audit ve Excel.
ICD10 listesinde istatistik ikonuyla `icd10DefId` filtre parametresi
gönderilir. İstatistik ekranında üç referans ve adet düzenlenir.

## Bağımsız izinler

- `ICD10Defs.View|Query|Create|Update|Delete`
- `ICD10Diagnostics.View|Query|Create|Update|Delete`

Başlangıç permission seed, menü görünürlüğü, route permission guard,
controller attribute ve dropdown endpoint'i uyumludur.

## Derleme ve migration uyarısı

GitHub deposunda EF Core Migrations ve ModelSnapshot yok.
**Yeni tablolar için migration oluşturulmadı ve uygulanmadı.**
Yerel veritabanı / migration geçmişi kontrol edilerek veri kaybı
oluşturmayan migration hazırlanmalı. Gerçek .NET/Angular derlemesi,
PostgreSQL bağlantılı entegrasyon ve tarayıcı testleri bu ortamda
çalıştırılmadı; sonuçlar doğrulanmış kabul edilmemeli.

Beklenen kabul senaryoları: ICD10 kod tekrarları, silinen kodu yeniden
kullanma, miktar negatif/tam sayı validasyonu, yinelenen üçlü ilişki
409, eski xmin 409, pasif referans düzenleme, Excel, Excel kayıt limiti,
frontend menü/permission görünürlüğü, FK silme/durum engeli.
