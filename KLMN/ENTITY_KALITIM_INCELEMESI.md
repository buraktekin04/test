# KLMN — Entity Kalıtımı Genel İnceleme (21.10.2026)

Bu kontrol `KLMN.Domain/Entities` altındaki **36 C# entity dosyasını**
ve ilgili Persistence Configuration türlerini kapsar.
Kod üzerinde statik kontrol yapılmıştır; `dotnet build`, PostgreSQL migration
ve uçtan uca testler bu ortamda çalıştırılmamıştır.

## Mimari hiyerarşi

- **BaseEntity**: Id, CreatedDate/CreatedBy, UpdatedDate/UpdatedBy,
  DeletedDate/DeletedBy, IsActive, IsDeleted, Version (PostgreSQL xmin).
- **EffectiveDatedEntity : BaseEntity**: ek olarak nullable UTC
  `ValidFrom` (dahil) ve `ValidTo` (hariç), `IsValidAt` helper'ı.
- Tarihli tanım tabloları: `EffectiveDatedEntityConfiguration<T>`
  ve fiziksel `ValidFrom/ValidTo` kolonları.
- Bağlantı, olay, kimlik veya tarihsiz referanslar:
  `BaseEntityConfiguration<T>`.
- AuditLog **bilinçli istisna**: audit kaydının tekrar audit edilmemesi /
  soft-delete edilmemesi için BaseEntity kullanılmıyor.

## EffectiveDatedEntity kullanan tanımlar (18)

| Modül | Entity | Durum |
|---|---|---|
| Admissions | BranchDef | Uygun |
| Admissions | HcAdmissionTypeDef | Uygun |
| Admissions | HcConnectedDecisionDef | Uygun |
| Admissions | HospitalDef | Uygun |
| Admissions | RankDef | Uygun |
| Admissions | RankGroupDef | Uygun |
| Admissions | ReasonForExaminationDef | Uygun |
| Admissions | ReportTypeDef | Uygun |
| Admissions | **ProcedureDef** | **Düzeltildi: BaseEntity → EffectiveDatedEntity** |
| Geography | CityDef | Uygun |
| Identity | User | Uygun |
| MilitaryUnit | ForcesCommandDef | Uygun |
| MilitaryUnit | MilitaryClassDef | Uygun |
| MilitaryUnit | MilitaryClassGroupDef | Uygun |
| MilitaryUnit | MilitaryOfficeDef | Uygun |
| MilitaryUnit | MilitarySchoolDef | Uygun |
| MilitaryUnit | MilitaryUnitDef | Uygun |
| PurchaseNumbers | PurchaseNumberDef | Uygun |

## BaseEntity kullanan, geçerlilik aralığı gerekmeyen varlıklar (17)

- **Admissions (6):** HospitalReportType, ProcedureTypeDef,
  ReasonForExamBranchDef, ReasonForExamProcedureDef, ReasonForExamReportType,
  ReasonForExamUnusedHcAdmissionType.
- **Geography (2):** CountryDef, TownDef.
- **Identity (8):** PasswordResetToken, Permission, RefreshToken,
  Role, RolePermission, UserAgreementPdf, UserPermission, UserRole.
- **MilitaryUnit (1):** MilitaryClassGroupRelation.

Burada `Def` son eki tek başına ValidFrom/ValidTo eklemek için yeterli
değildir. Domain ve ekran ihtiyacında geçerlilik dönemi bulunmuyorsa
ortak BaseEntity yeterlidir. Yeni gereksinim gelmeden tarihsiz tablolara
sırf isim benzerliği nedeniyle kolon eklemeyiz.

## Özel tarih alanları

`PurchaseNumberDef.HealthReportEndDate`, `User.LockoutEnd`,
`User.LastLoginDate`, `User.PasswordChangedDate`,
`PasswordResetToken.ExpiresAt`, `RefreshToken` güvenlik tarihleri
kendi iş anlamlarını taşıdıkları için `ValidFrom/ValidTo` ile
karıştırılmaz veya otomatik kaldırılmaz.

## ProcedureDef ve eşlik eden tüm katmanlar

- `ProcedureDef : EffectiveDatedEntity`: eski `StartDate/EndDate`
  property'leri **kaldırıldı**.
- `ProcedureDefConfiguration :
  EffectiveDatedEntityConfiguration<ProcedureDef>`.
  Fiziksel kolonlar `ValidFrom/ValidTo`; `StartDate/EndDate` yok.
- Create/Update Command, Handler, Validator, Response:
  `ValidFrom/ValidTo`; `ValidityPeriod.HasValidBounds` ve UTC kontrolleri.
- Angular model, reactive form, tablo ve DTO:
  `validFrom/validTo`; datetime-local dönüşümleri korunur.
- Excel başlıkları başlangıç/bitiş tarihleri olarak kalır, okunan
  C# property'leri `ValidFrom/ValidTo` olur.
- Audit whitelist `ProcedureDef` için `ValidFrom/ValidTo` kullanır.
- `ProcedureCode`, `Name`, üç `Requires*` bool ve dinamik
  `ProcedureType` ilişkisi aynen korunur.
- Regression kaynaklarına reflection temelli kalıtım/property
  tekrar testi eklendi.

## Migration güvenliği

GitHub deposunda EF Core Migrations ve ModelSnapshot görülmedi.
Henüz DB'de veri var mı / eski kolonlar oluşturuldu mu bilinmiyor.
Var olan `ProcedureDefs.StartDate` ve `EndDate` alanları varsa, yeni
`ValidFrom` ve `ValidTo` alanlarına açık migration/veri taşıma
planıyla aktarılmalı. Yerel migration geçmişi doğrulanmadan otomatik
DROP/RENAME çalıştırılmamalıdır.

## Yeni geliştirmeler için kontrol listesi

1. Önce BaseEntity ve EffectiveDatedEntity alanlarını incele.
2. Geçerlilik dönemi gerekiyorsa EffectiveDatedEntity kullan;
   ayrıca ValidFrom/ValidTo yazma.
3. Eski sistemdeki StartDate/EndDate adlarını yeni KLMN API, entity,
   UI, audit veya fiziksel DB kolonlarına taşıma.
4. Entity inheritance ve PersistenceConfiguration inheritance aynı olsun.
5. DTO, CQRS validator, Angular form, Excel, audit ve regresyon
   testi alanları birebir uyuşsun.
6. İş anlamı farklı tarihlere dokunma.
