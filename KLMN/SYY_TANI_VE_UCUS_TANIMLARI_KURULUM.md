# SYY Tanı Listesi (M/D/F), sevk ve uçuş tanımları — 22.10.2026

## Kaynak fotoğraf yorumları / ekranlar
- `SYYDiagnosisDef` → **SYY Tanı Listesi (M/D/F)** (`/app/syy-diagnoses`)
- `AnnotationReasonDef` → **Sevk Sebebi Tanımları** (`/app/annotation-reasons`)
- `PlaneTypeDef` → **Uçuşa Uçak Tipi Tanımları** (`/app/plane-types`)
- `FlyingMissionDef` → **Uçucu Görev Tanımları** (`/app/flying-missions`)
- `SYYDiagnosisTypeDef` → **SYY Tanı Türü Tanımları** (`/app/syy-diagnosis-types`)
- `SYYDiagnosisSliceDef` → **SYY Dilim Tanımları** (`/app/syy-diagnosis-slices`)

## Legacy alanları ve ilişkiler
`SYYDiagnosisDef`: Code:string, Name:string, SYYDiagnosisType:int,
AttachedRecNo:Guid?, Eligible:bool?, Slice:int?. `IsActive`, audit ve
xmin BaseEntity; `ValidFrom/ValidTo` EffectiveDatedEntity üzerinden gelir.
Eski StartDate/EndDate, CreationDate/LastUpdate, ConcurrencyToken tekrar yazılmaz.

- `SYYDiagnosisType` → `SYYDiagnosisTypeDefs.Code` (int alternate key, RESTRICT).
  Türler ayrı dinamik CRUD sayfasından yönetilir. Eski tür kodları
  fotoğrafta kesin görünmediğinden tür kodları **tahmin edilip seed edilmedi**.
- `Slice` → `SYYDiagnosisSliceDefs.Code` (nullable int alternate key, RESTRICT).
  Fotoğrafta açıkça verilen 1=A, 2=B, 3=C, 4=D seed kayıtları
  EF `HasData` olarak tanımlandı; **migration uygulanmadan DB'de oluşmaz**.
  Dilim ekranından yeni kodlar tanımlanabilir.
- `AttachedRecNo` → `SYYDiagnosisDefs.Id` (nullable GUID self FK, RESTRICT).
  Aynı tablodan bağlı tanı seçilir, listede bağlı tanının adı gösterilir.
  Backend yeni bağlantıda aktif tanıyı doğrular ve ancestor traversal ile
  kendini bağlama ve A→B→A benzeri döngüleri engeller.
  Alt tanılar bağlıyken üst tanı pasifleştirilemez veya silinemez.
  Bir tür/dilim kullanımdayken silme engellenir; kullanımdayken tür/dilim
  pasifleştirme de kısıtlanır.
- `Eligible` nullable bool kalır; listede Evet/Hayır/Belirsiz gösterilir.
- Branş → Madde → Dilim → Fıkra gibi hiyerarşi, gerekirse AttachedRecNo
  üzerinden kurulabilir. Tanı türü için isim/sayısal kod eşleşmeleri
  iş birimiyle doğrulanmalıdır; otomatik sabit enum varsayılmaz.

## Genel KLMN geliştirme kuralları
Tanım entity'leri EffectiveDatedEntity, int kodlu ilişki referansları
BaseEntity kalıtımıyla oluşturuldu. Benzer ekranlarda her Command/Handler/
Validator, Query/Handler/Validator ayrı dosyada; EF DbContext,
PostgreSQL RESTRICT FK, soft-delete, xmin, audit whitelist, bağımsız
PermissionCodes, Identity seed, global 404/409 exception, Excel,
Angular standalone route/menü, PrimeNG p-table kolon resize + frontend
sort, backend filtre/sayfalama, durum dialog/Toast ve audit kullanıldı.
Altı ekran ayrı menü/route açılır; yeniden kullanılabilir Angular ekran
component'i data konfigürasyonuyla aynı kullanıcı deneyimini sağlar.

## Migration ve test
GitHub deposunda EF Migrations/ModelSnapshot yok. Bu değişiklikte migration
**üretilmedi ve uygulanmadı**. Yerel migration zinciri ve mevcut veri
incelenmeden DB drop/recreate yok; özellikle self FK ve alt tablolarda
eski kodlar için güvenli taşıma planı gerekir.

`KLMN/Tests/SYYDefinitions.Regression` kaynak testleri eklendi.
Bu ortamda `dotnet` bulunmadığı ve Angular bağımlılıkları kurulmadığı için
`dotnet build`, `npm build`, PostgreSQL DB ve tarayıcı kabul testleri
**çalıştırılmadı**. Testler ayrıca koşulmalıdır.

### Önemli kabul testleri
1. Tanı Türü ve Dilim aktif seçeneklerini açıp, pasif seçili kayıtları düzenlemede görmek.
2. A-D seed kodları ve DB unique FK constraints.
3. Bağlı kayıt self-link ve döngü engeli; alt kayıt varken silme/pasifleştirme.
4. Eligible null, true, false roundtrip.
5. Eski xmin ile update/status/delete 409.
6. Farklı izne sahip kullanıcıların ekran, dropdown ve Excel erişimi.
7. ValidFrom/ValidTo ve UTC tarih, audit ve soft-delete.
