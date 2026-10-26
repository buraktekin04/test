# Kurul Karar Tanımları ve Dört Alt Eşleştirme — 26.10.2026

## Ekran ve eski MSKBS model eşleştirmesi

| Eski model | Yeni entity | Angular ekran / API |
| --- | --- | --- |
| HcDecisionDef | HcDecisionDef | Kurul Karar Tanımları — `/app/hc-decisions` |
| HcUnusedReasonForExam | HcUnusedReasonForExam | Kullanılmayacak Muayene Sebepleri — `/app/hc-unused-reasons` |
| HcUnusedAdmissionType | HcUnusedAdmissionType | Kullanılmayacak Başvuru Grupları — `/app/hc-unused-admission-types` |
| HcUnusedDecision | HcUnusedDecision | Birlikte Verilmeyecek Kararlar — `/app/hc-unused-decisions` |
| HcConnectedDecision | HcConnectedDecision | Bağlı Kararlar — `/app/hc-decision-connections` |

Bu beş ekran, Tanımlamalar > Kurul Kararları menü grubundadır.
Her ekranın bağımsız component, HTML, SCSS, model ve servisi vardır; muayene sebebi CRUD/filter/dialog örüntüsü korunur.

## Alanlar

**HcDecisionDef:** `Code int`, `Name string`, `TimeInformationRequired`,
`AdditionalDescriptionRequired`, `NegativeDecision`, `ApprovalRequired`,
`PertemApprovalRequired`, `DiagnosisRequired` (altısı `bool`).
Eski `StartDate/EndDate` ortak `EffectiveDatedEntity.ValidFrom/ValidTo` olarak karşılanır.
`Code` 0 veya üzeri ve silinmemiş kayıtlar arasında benzersizdir.

Alt dört tablo ortak `BaseEntity` kullanır, ekstra audit/aktiflik/xmin alanı içermez.
Her biri `HcDecisionDefId` ve şunlardan birini tutar:
- `ReasonForExaminationDefId` (mevcut Muayene Sebebi)
- `HcAdmissionTypeDefId` (mevcut Başvuru Grubu)
- `UnusedHcDecisionDefId` (aynı HcDecisionDef tablosuna self-FK, kendisiyle eşleştirme yasak)
- `HcConnectedDecisionDefId` (mevcut Bağlı Kurul Karar Tanımı)

Tüm bağlantılar `DeleteBehavior.Restrict`; aktif kayıtların çiftleri unique partial index ile korunur.
Yeni bağlantılarda aktif ve silinmemiş FK zorunludur. Değiştirilmemiş tarihsel pasif seçimler formda korunur.
Üst kayıtlar, bağlı aktif eşleştirmeleri varken pasifleştirilemez; silinmemiş bağlantıları varken silinemez.
Her işlem ayrı CQRS command/handler/validator dosyasında; DTO ve exception sınıfları da ayrı dosyadadır.
EF Core Select projeksiyonları QueryHandler'da, frontend kolon sıralaması yalnız PrimeNG p-table'dadır.

## Yetkiler ve audit

`HcDecisions`, `HcUnusedReasons`, `HcUnusedAdmissionTypes`,
`HcUnusedDecisions`, `HcDecisionConnections` modüllerinde ayrı ayrı
View/Query/Create/Update/Delete izinleri vardır. Bir modülün herhangi bir aktif izni
menü ve listeye erişim sağlar; kaydetme, durum değişimi, silme ve Excel için
ilgili işlem izni ayrıca gereklidir. Audit whitelist, audit tür doğrulayıcısı,
ProblemDetails 404/409 eşlemesi ve PostgreSQL xmin korunur.

## Veritabanı ve test kabulü

**Migration bu committe OLUŞTURULMADI / UYGULANMADI.**
Depo, yerel EF Core migration ve ModelSnapshot geçmişini içermiyor.
Canlı/yerel geçmiş korunarak gerçek proje makinesinde bu beş tablo için
migration üretilmeli; oluşan SQL ve mevcut FK'ler incelenerek yalnız test
veritabanında uygulanmalı. Snapshot olmadan `InitialCreate`, `Drop/Create`,
DB reset veya körlemesine `Database.MigrateAsync` yapılmaz.

Derleme komutları:
```powershell
cd KLMN/Backend
dotnet restore KLMN.slnx
dotnet build KLMN.slnx
dotnet run --project ../Tests/HcDecisions.Regression
cd ../Frontend
npm install
npm run build
```

GitHub üzerinde kaynak dosyaları, route/permission eşleşmesi ve import yolları
statik olarak kontrol edildi. .NET SDK ve PostgreSQL bu çalışma ortamında
olmadığından **gerçek dotnet build/test, Angular build, migration ve browser CRUD
testleri koşulmuş veya başarılı olarak kabul edilmiş değildir.**
Bu kontroller, ilgili ekip ortamında tamamlanıp gerekirse bir takip düzeltmesiyle
kayda geçirilmelidir.

## Kabul listesi

1. Ana karar CRUD, altı boolean, kod benzersizliği, UTC tarih aralığı.
2. Dört bağımsız alt ekranın CRUD, durum/filtre, Excel ve audit geçmişi.
3. Ana karar tablosunda aktif alt kayıt isimleri ve ilişkili ekrana filtreli geçiş.
4. 401/403/409, tek yetki (yalnız Create), aktif/pasif referans, eski xmin testi.
5. Başvuru grubu, muayene sebebi ve bağlı kararın silme/pasifleştirme korumaları.
6. Responsive PrimeNG listeler, dialog, sütun resize/sort ve UI karşılaştırması.
