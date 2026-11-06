# KLMN — Provizyon Modülü: Veri Modeli, Kurulum ve Aktarım

## 06.11.2026 — Provizyon durum sözlüğü ve ekran düzenlemeleri

Provizyon durumları için **ProvisionStatusDefs** isimli referans tablosu
eklendi. Bu tablo `Code` (int PK), `Name` ve `SortOrder`
alanlarından oluşur; EF Core `HasData` ile Yeni, Rapor, İptal, Bildirim
tanımları eklenir. `Provision.CurrentState` ve `ProvisionState.State`
alanlarının tipleri ve değerleri **değişmez**; iki tablo da
`ProvisionStatusDefs.Code` ile foreign key ilişkisine bağlanır.
Mevcut provizyon ve durum geçmişi GUID ilişkileri aynı kalır.

`GET /api/provisions/statuses` yalnız yetkili kullanıcıya durum
tanımlarını getirir. Sorgulama ve Takip ekranındaki görünen durum adları
bu API'den okunur, kod-ad eşlemeleri frontend'e gömülmez.

Arayüzde Sevkli Giriş'in gereksiz üst başlığı kaldırıldı, ham GUID
metinleri kullanıcıya gösterilmez, zorunlu alan yıldızları isimle
aynı satırda tutulur. Navigasyon bağlantıları açık tonlu, klavye
odak stili bulunan buton görünümüne dönüştürüldü.

**Migration gerekiyor:** `ProvisionStatusDefs` tablosu, dört seed
satırı, `Provisions.CurrentState` ve `ProvisionStates.State`
FK'leri ile eski CHECK constraint'lerinin kaldırılması.
Migration oluşturma ve uygulama kullanıcıya bırakılmıştır.
Verileri aktarmadan önce durum değerlerinin tanımlı kodlarda olduğunu
denetleyin. Yeni migration **üretip çalıştırmadım**.


## Eski ekranların yeni karşılığı

| Eski MSKBS | KLMN yeni ekran | Route |
| --- | --- | --- |
| newprovision.component | Provizyon Sevkli Giriş | /app/provisions/with-dispatch |
| newprovisionwithfullinfo.component | Provizyon Sevksiz Giriş | /app/provisions/without-dispatch |
| provisionstatussummary.component | Provizyon Takibi (yerel durumlar) | /app/provisions/tracking |
| queryprovision.component | Provizyon Sorgulama, detay, iptal | /app/provisions/query |

Ekranlar Angular 21 / PrimeNG; backend .NET 10 CQRS/MediatR/FluentValidation.

## PostgreSQL'de yeni gereken tablolar

**Provisions** (eski `MSKBSModel.Provision`):

- `Id` — eski `ObjectId`; Guid. Eski Oracle kayıt GUID'si aynen korunmalı
- `DispatchId` — eski `Dispatch`; var olan `Dispatches.Id`
- `ProvisionDate`, `ProvisionNo` (long identity), `DispatchHospitalId` (HospitalDefs.Id)
- `PhotoObjectId`, `Photo` (bytea), `WithoutDispatch` (nullable int, sevksizde 1)
- `CurrentState` (1 Yeni, 2 Rapor, 3 İptal, 4 Bildirim)
- `ObjectionableReportId` (nullable eski rapor GUID'si, **rapor entity henüz yok**, FK oluşturulmadı)
- `Description` (eski hatalı yazım `Desciption` yeni düzgün ad)
- BaseEntity audit: CreatedDate/By, UpdatedDate/By, IsActive/IsDeleted ve xmin Version

**ProvisionStates** (eski `MSKBSModel.ProvisionState`):

- `Id` — eski `ObjectId`; Guid
- `ProvisionId` — eski `Provision`; Provisions.Id
- `State` — eski enum: 1,2,3,4
- BaseEntity audit ve xmin. Eski status change için `CreatedDate` doğru sırada aktarılmalı.

**ProvisionChangedStatusSummary** eski kodda tablo değil, entegrasyon/rapor DTO'su
(`provisionRegisterNo,patientId,alimId,status`). Bilinçli olarak DB tablosu üretilmedi.

## Ayrı olarak ihtiyaç duyulan bilgi / kod

1. **Provizyon durum servisi**: `queryProvisionState` ve
   `queryProvisionSummaryState` API istek/yanıt modelleri;
   `ProvisionStatusDetail`, `ProvisionStatusSummary`,
   `ExaminatonChanged`, `ReasonExaminationChanged` yapıları.
   Bunlar yalnızca `test3` ekranında referanslanıyor. Şimdiki Takip ekranı
   gerçek veritabanı provizyonlarını ve ProvisionStates geçmişini gösterir;
   *bakanlık hasta/rapor/tetkik/imza bilgisi varmış gibi davranmaz*.
2. **İtiraz Raporu / Rapor Girişi**: `ItirazRaporInput`,
   `getReportLastMonthList`, `ProvizyonSevksizYeniGirisFoto`,
   rapor entity/API sözleşmesi ve rapora gidiş route'u.
   `ObjectionableReportId` için FK doğrulaması/itiraz raporu
   seçme ve rapor giriş işlemi bu sözleşmeler gelince bağlanır.
3. **Eski servis izinleri** (`isGetProvisionWithDispatch`,
   `isGetProvisionNotDispatch`, `isQueryProvision`,
   `isCancelProvision`, `isSaveReport`): yeni Provisions
   permission'larına ayrıldı. Rapor yetkisi gerçek API geldiğinde bağlanmalı.
4. **Sevksiz işlem semantiği**: eski Provision.Dispatch NOT NULL.
   KLMN sevksiz girişte kişi+otomatik sevk+provizyonu tek veritabanı
   SaveChanges işleminde oluşturuyor; `WithoutDispatch=1`,
   `Dispatch.AutomaticDispatch=true`. Eski backend
   `newProvisionWithoutDispatchPhoto` implementasyonu verilirse bu
   iş kuralı birebir teyit edilebilir.
5. **Durum geçişleri**: eski Provision enum 1/2/3/4 sabit.
   Rapor/Bildirim geçiş API'leri verilmedi; sadece kayıt (Yeni) ve
   açıklamalı İptal geliştirilmiştir. Eski transfer ve rapor onay
   kodları olmadan sahte geçiş eklenmemiştir.

## Veritabanı migration ve veri taşıma talimatı

- KLMN için yeni migration gerekir. **Migration oluşturma/uygulama
  kullanıcı tarafından yapılacak**; repo bu turda migration çalıştırmadı.
- Oracle `RAW(16)` GUID byte sıralaması dönüşümünü doğrulayın;
  ID'leri text üzerinden karşılaştırarak aktarım sonrası kesin eşleşme
  testleri yapın. **Yeni Guid üretmeyin**.
- Parent tablolar (`Persons`, `Dispatches`, `HospitalDefs`, varsa
  Raporlar) önce, `Provisions` sonra, `ProvisionStates` en son.
- Eski `Provision.ObjectId` -> `Provisions.Id`;
  `Provision.Dispatch` -> `Provisions.DispatchId`;
  `Provision.DispatchHospital` -> `Provisions.DispatchHospitalId`;
  `ProvisionState.ObjectId` -> `ProvisionStates.Id`;
  `ProvisionState.Provision` -> `ProvisionStates.ProvisionId`.
- Eski numaralar korunacaksa `ProvisionNo` identity sequence'ini
  en büyük aktarılmış değerden sonra başlatın; aksi takdirde çakışma olur.
- Mevcut `Dispatches.CurrentState` değerlerini eski provizyon
  ilişkilerine göre doğrulayın. Sevkli provizyon yeni kayıt sırasında
  `Dispatch.CurrentState=2` olarak güncellenir.
- İptal edilmiş eski provizyonlar silinmez; `CurrentState=3`,
  tarihçe ayrıca korunur.
- Yeni `Provisions.DispatchId` üzerinde bir aktif provizyon
  benzersizlik kuralı var. Eski sistemde aynı sevke birden fazla
  provizyon varsa aktarım öncesinde tekrar iş kuralını kesinleştirmek gerekir.
- Fotoğraf ve eski `PhotoObjectId` ilişkisi eksik kaynak
  sözleşmesine göre ayrıca teyit edilmeli.

## Test ve önemli sınır

```bash
cd KLMN/Frontend
npm run test:provisions
npm run build
cd ../Backend
dotnet build KLMN.slnx
dotnet run --project ../Tests/Provisions.Regression
```

PostgreSQL migration ve veri senaryoları gerçek ortamda ayrıca
doğrulanmadan üretime alınmamalıdır.
