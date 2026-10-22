# SYY Bağımlı Tanım Ekranları — 22.10.2026

## Fotoğraflardaki legacy class'lar ve UI karşılıkları

| Legacy | Angular ekran adı | KLMN tablosu |
|---|---|---|
| SYYClassificationDef | SYY Tanı Sınıflandırma Çizelgesi | SYYClassificationDefs |
| SYYDiagnosisBranchDef | SYY Gereken Branşlar | SYYDiagnosisBranchDefs |
| SYYDiagnosisProcedureDef | SYY Gereken Tetkikler | SYYDiagnosisProcedureDefs |
| SYYDiagnosisRequired | SYY Onay Gerektiren Maddeler | SYYDiagnosisRequireds |
| SYYDiagnosisRequiredDetail | Aynı ekranın alt madde açıklamaları | SYYDiagnosisRequiredDetails |

**Dört bağımsız Angular ekranı** bulunmaktadır. Kural detayları ayrı iş tablosudur
ancak ayrı bir tanım ekranı değildir: ana kural popup'ındaki sıralı açıklama
satırlarından eklenir, düzenlenir, silinir. Bu iki tabloyu tekleştirmek
birden fazla maddeyi tek metne ya da JSON dizisine gömmeyi gerektireceğinden
normalizasyon ve audit açısından önerilmez.

## İlişkiler

- SYYClassificationDef: SYYDiagnosisDefId -> SYYDiagnosisDefs.Id,
  ForcesCommandDef -> ForcesCommandDefs.Id,
  RankGroup -> RankGroupDefs.Id,
  MilitaryClassGroup -> MilitaryClassGroupDefs.Id,
  HcDecision -> HcConnectedDecisionDefs.Id. **Yeni kuvvet, rütbe,
  sınıf grubu veya karar tablosu oluşturulmaz.**
- SYYDiagnosisBranchDef: SYYDiagnosisDefId -> SYYDiagnosisDefs.Id,
  BranchDefId -> BranchDefs.Id. StartingAge, EndingAge ve Gender eski
  sistemdeki int alanları olarak korunur (0..130 yaş; Gender=0..20 geçici
  validator aralığı, cinsiyetin iş anlamı teyit edilmeli).
- SYYDiagnosisProcedureDef: SYYDiagnosisDefId -> SYYDiagnosisDefs.Id,
  ProcedureDefId -> ProcedureDefs.Id. Aynı yaş/cinsiyet kuralı.
- SYYDiagnosisRequired: SYYDiagnosisDefId -> SYYDiagnosisDefs.Id,
  MustAll/MustOneOf nullable bool. İkisi aynı anda true olamaz.
  Bir tanıya birbirinden bağımsız **birden fazla kural** bağlanabilir.
- SYYDiagnosisRequiredDetail: SYYDiagnosisRequiredId ->
  SYYDiagnosisRequireds.Id, Description string (en çok 1500),
  SortOrder int. Bir kural için 1..50 madde;
  detaylar ana kuralın create/update/delete isteğinde birlikte işlenir.

Bütün FK'ler **Restrict**, tüm tablolar **BaseEntity** audit, aktiflik,
soft-delete, concurrency `xmin` kullanır. Sadece SYYClassificationDef
geçerlilik penceresine sahip olduğundan **EffectiveDatedEntity** kalıtımı
alır; eski StartDate/EndDate yerine `ValidFrom/ValidTo` kullanılır.
Kural/detail tarihli entity değildir; fazladan kolon oluşturulmaz.

Kuvvet ile sınıf grubunun aynı kuruma bağlı olduğu backend'de
doğrulanır. Yeni seçilen bağlı kayıtların aktif olması aranır.
SYY tanısına bağlı etkin ilişkiler varsa tanı pasifleştirilemez;
herhangi bir bağlı ilişki varsa soft-delete engellenir.
Cinsiyet kodu için hatalı erkek/kadın eşlemesi varsayılmadı.

## Ekran bağımsızlığı

Her ekran için **ayrı component.ts, component.html, component.scss,
model.ts ve service.ts** oluşturulmuştur. Mevcut SYY Tanı Listesi'ndeki
satırlardan dört bağımsız sayfaya `syyDiagnosisId` filtresiyle gidilebilir.
Her sayfa kendi permission kodlarını kullanır; seçenekler aynı sayfanın
`options` endpoint'inden gelir. Kolon sıralama Angular PrimeNG p-table,
filtre/sayfalama API tarafındadır; kolon genişliği değiştirilebilir,
işlemler confirm popup ve toast mesajıyla yönetilir. Audit/Excel eklendi.

`/app/syy-classifications`
`/app/syy-diagnosis-branches`
`/app/syy-diagnosis-procedures`
`/app/syy-diagnosis-requireds`

## Migrasyon ve doğrulama durumu

GitHub deposunda mevcut EF Core migration zinciri / ModelSnapshot yok.
Yeni tabloların fiziksel olarak açılması için yerel migration geçmişi
kontrol edilip **veri kaybı olmadan** migration hazırlanmalıdır. Bu
commit içinde migration üretilmedi veya PostgreSQL'e uygulanmadı.

`Tests/SYYDefinitions.Regression` kaynaklarına entity kalıtımı,
validator ve xmin regresyonları eklendi. Gerçek `dotnet build`,
`ng build`, PostgreSQL entegrasyon ve browser testleri bu ortamda
çalıştırılamadığı için **başarılı kabul testleri olarak raporlanamaz**.
Test için temel senaryolar:
- SYY tanısının tüm FK dropdownları ve isim görünümü.
- Kuvvet/sınıf grubu uyumsuzluğu, FK silme ve pasifleştirme reddi.
- Aynı tanıya çoklu kural eklenebilir; duplicate branş/tetkik ilişkisi 409.
- MustAll/MustOneOf, en az bir açıklama, madde silme ve audit.
- Eski xmin ile eşzamanlı update ve yalnız detay değişiminde xmin yenileme.
- Permission ayrımı, Excel, filtre, sort, sütun genişlikleri.
