# Muayene Sebebi ve İlgili Tanımlar — Kurulum

## Kaynak sınıf yorumları
- ReasonForExaminationDef: **Muayene Sebebi** (ana ekran)
- ReasonForExamReportType: **Muayene Sebebi Rapor Türleri**
- ReasonForExamUnusedHcAdmissionType: **Kullanılmayacak Hasta Grupları**
- ReasonForExamProcedureDef: **Gereken Tetkikler**
- ReasonForExamBranchDef: **Muayene gereken branşlar** (UI: Muayene Gereken Branşlar)

## Veri modeli
Ana entity: ReasonForExaminationDef : EffectiveDatedEntity.
Code, Description, DispatchRequired, ClassificationExamination, Objectionable; ValidFrom/ValidTo mirastır.
Dört ilişki BaseEntity türevidir, (ReasonForExaminationDefId, ilgili FK) ile bağlanır.
Rapor türü ReportTypeDefs, hasta grubu HcAdmissionTypeDefs, branş BranchDefs ile eşleşir.
Tetkik ProcedureDefs referansına ihtiyaç duyduğu için asgari Code/Name tablosu ve Tetkik Tanımları ekranı oluşturuldu.
**DİKKAT:** Eski ProcedureDef ekran görüntüsü sağlanmadığından bu modelin alanları kesinleştirilmedi; veri göçünden önce asıl ProcedureDef incelenmelidir.

Branş ve tetkik ilişkisinde fotoğraftaki StartingAge, EndingAge, Gender **int** olarak saklanır.
Cinsiyet kodunun eski sistem anlamı bilinmediği için numeric değer ve label "Cinsiyet Kodu" kullanıldı.
Yaş aralığında başlangıç <= bitiş; 0-130 sınırı uygulanır. Bu sınır ve 0 değerinin anlamı alan kullanıcılarıyla doğrulanmalıdır.
Yaş/cinsiyet ilişkilerinde benzersizlik muayene sebebi + referans + üç koşul değerine göredir; birden çok yaş grubuna izin verilir.
Diğer ilişkilerde benzersizlik muayene sebebi + referans çifti içindir.
Tüm FK DeleteBehavior.Restrict, soft-delete filtreli unique index, xmin, audit ve Modül permission'ı vardır.
Üst kayda aktif eşleştirme varken pasifleştirme; silinmemiş eşleştirme varken soft-delete engellenir.

## Ekranlar
- /app/reasons-for-examination — Muayene Sebebi, her alt ilişkinin aktif adları aynı tabloda
- /app/reason-exam-report-types — Muayene Sebebi Rapor Türleri
- /app/reason-exam-unused-admission-types — Kullanılmayacak Hasta Grupları
- /app/reason-exam-procedures — Gereken Tetkikler
- /app/reason-exam-branches — Muayene Gereken Branşlar
- /app/procedures — Tetkik Tanımları (eksik ProcedureDef referansı için)

Modüller kendi View/Query/Create/Update/Delete yetkilerine sahiptir. Query Excel iznidir.
Ana ekrandan alt ekranlara reasonId ile gidilir; alt ekrandaki formda sebep önceden seçilir.
Backend arama/paging; frontend PrimeNG p-table kolon sorting/resize ve durum/kolon filtreleri korunur.
Bağımsız CQRS Command/Handler/Validator ve Query sınıfları eklendi.

## Migration ve test DURUMU
Mevcut EF Migrations/ModelSnapshot zinciri GitHub'da yok. Tablolar ve indeksler için migration **üretilmedi** ve DB'ye **uygulanmadı**.
Yerel migration zinciri ve uygulanan migration listesi incelenmeli; script üretildikten sonra veri kaybı olmadan uygulanmalı.
dotnet build, npm install/build, PostgreSQL ve tarayıcı kabul testleri bu çalışmada çalıştırılamadı.
KLMN/Tests/ReasonsForExamination.Regression/ kaynak testleri eklendi fakat koşulmadı.

## Kritik kabul testleri
1. UI'da her ilişki eklenince ana tabloda hemen güncel ad görünmeli ve boşsa "Yok" yazmalı.
2. View-only, Create-only, Query-only rol senaryoları; başka modülün izni gerekmeden dropdown çalışmalı.
3. Aynı ilişki/yaş-cinsiyet kombinasyonu tekrar eklenmemeli.
4. Pasif referans mevcut güncellemede korunmalı; yeni seçimde reddedilmeli.
5. xmin eski version → 409; üst kayıt bağları ve soft-delete silme engelleri.
6. Cinsiyet kodları ve eski ProcedureDef iş alanları gerçek kullanıcılarla teyit edilmeli.

## 20.10.2026 — AsSplitQuery CS1061 düzeltmesi
Muayene Sebebi detay ve liste Handler'larında `AsSplitQuery()` çağrıları vardır.
`KLMN.Application` daha önce sadece `Microsoft.EntityFrameworkCore` 10.0.0 referansını taşıyordu.
`AsSplitQuery` metodu ise `Microsoft.EntityFrameworkCore.Relational` içerisindedir.
Bu nedenle `KLMN.Application.csproj` dosyasına Relational 10.0.0 doğrudan eklendi.
Dört koleksiyonu split query olarak getiren davranış korunmuştur.
GitHub kaynak kontrolü dışında dotnet restore/build yapılamadı; yerelde paketleri geri yükleyip derleyin.

## 20.10.2026 — Önceki asgari ProcedureDef modelinin kesinleştirilmesi
Üstteki ProcedureDef Code/Name ve EffectiveDatedEntity açıklaması ARTIK GÜNCEL DEĞİLDİR.
Yeni legacy fotoğraflarına göre fiziksel kolonlar ProcedureCode, Name,
RequiresNumericResult, RequiresReferenceRange, RequiresDeviationInfo,
StartDate, EndDate, ProcedureType olarak düzenlendi. ProcedureType artık
ayrı dinamik ProcedureTypeDefs.Code değerine FK'dir. Ayrıntı:
KLMN/PROCEDURE_DEF_TETKIK_TURU_GUNCELLEME.md.
