# Branş Tanımları ve tüm p-table kolon sürükleme standardı

## Model ve mimari

Eski MSKBSModel.BranchDef fotoğrafındaki sadece Code (string) ve
Name (string) alanları KLMN.Domain.Entities.Admissions.BranchDef
üzerinde oluşturuldu. ObjectId, CreationDate/CreatedBy,
LastUpdate/UpdatedBy, ConcurrencyToken, IsActive bilgileri
BaseEntity tarafından; StartDate/EndDate ise EffectiveDatedEntity
ValidFrom/ValidTo üzerinden sağlanır. Kopya alan EKLENMEZ.

EF mapping `BranchDefs` ve silinmemiş kodlar için benzersiz indeks.
KLMNDbContext ve IKLMNDbContext veri kümesi, Branches içinde
ayrı Create/Update/Delete/ChangeStatus Command/Handler/Validator;
GetBranchDefs/GetBranchDefById Query/Handler/Validator; optimistic
xmin concurrency, soft-delete ve 404/409 ProblemDetails.
PermissionCodes.Branches.View/Query/Create/Update/Delete (seed + frontend),
audit whitelist (Code, Name, ValidFrom, ValidTo, IsActive).

HTTP: `/api/branches` üzerinde GET liste, GET /{id}, POST, PUT /{id},
PATCH /{id}/status, DELETE /{id}?version=... ve GET /export.
Excel Query izni ve 5000 kayıt sınırı ile korunur.

## Angular: Tanımlamalar → Branş Tanımları

`/app/branches`: Diğer tanım ekranlarıyla aynı filtre, arama,
front-end p-table sort, backend sayfalama, popup, status, soft-delete,
Excel ve audit. Form: Kod, Ad, Geçerli Başlangıç ve Bitiş.

## KALICI KARAR: Tüm p-table listelerinde kolon çizgileri + resize

Mevcut **16 adet p-table** HTML listesi `[resizableColumns]="true"`
ve `columnResizeMode="expand"` alır. Yeniden boyutlandırılabilir
bütün `th` etiketlerine `pResizableColumn` eklenir. Sağda sabit
`sticky-actions-col` İşlemler başlığı sabit tutulur, resize edilmez.
Tüm ekranlarda dikey ayırıcı çizgi, kullanıcının sürükleyebileceği
kolon handle'ı ve tablo kapsayıcısında yatay scroll bulunur.
Bu ayarlar `Frontend/src/styles.scss` içinde ortak tutulur ve bundan
sonra oluşturulan tüm p-table'larda zorunludur.

Bağlı Kurul Karar Tanımları'nda uzun Karar Adı metni artık
kesilmez; `white-space:normal`, `overflow-wrap:anywhere`
ile alt satıra geçer. İlk kolon başlangıç genişliği 370px'tir;
kullanıcı başlık çizgisini sürükleyerek değiştirebilir.

## Migration ve test

GitHub çalışma ortamında gerçek EF migration snapshot/DB
erişimi doğrulanamadı. Yalnız mevcut migration geçmişini
koruyarak yerel geliştirme ortamında yeni migration oluşturun:

```powershell
cd KLMN/Backend
dotnet ef migrations add AddBranchDefinitions --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
dotnet ef migrations script --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
# Script incelemesi sonrasında TEST veritabanında:
dotnet ef database update --project KLMN.Persistence --startup-project KLMN.Api --context KLMNDbContext
```

```powershell
dotnet build KLMN/Backend/KLMN.slnx
dotnet run --project KLMN/Tests/Branches.Regression/Branches.Regression.csproj
npm --prefix KLMN/Frontend run build
```

Kabul: Branş CRUD/soft-delete/xmin/izin/audit/Excel;
16 tablo için kolon resize, dikey çizgi, yatay scroll ve sticky
İşlemler; uzun karar adının çok satırlı görünmesi.
Gerçek build/test, migration ve tarayıcı entegrasyonu bu
çalışma ortamında çalıştırılmadı.
