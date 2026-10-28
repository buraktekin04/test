# 04 — Bağımsız tanımları topluca taşı

Bu paket KLMN'deki **13 basit Code + Name tanım tablosunu** tek Oracle sorgu sonucu / tek CSV / tek PostgreSQL aktarım adımıyla kapsar.

**ÖNEMLİ:** Gerçek Oracle bağlantısına erişemiyoruz. Kaynak tablolardan bazıları eski sistemde hiç bulunmayabilir veya CODE/NAME/ISACTIVE adları farklı olabilir. Önce **00_ORACLE_KONTROL.sql** ile tablo/kolonları mutlaka doğrula. Olmayan tabloların SELECT bloklarını 01 sorgusundan çıkarmadan çalıştırma. Sadece mevcut doğrulanmış eski tabloları dışa aktar.

1. Oracle'da `00_ORACLE_KONTROL.sql` çalıştır.
2. `01_ORACLE_TEK_EXPORT.sql` içinden mevcut kaynaklara ait SELECT bloklarıyla tek sonuç alıp XLS aktar.
3. Excel'de **Unicode Metin (.txt)** seç → VS Code'da **Save with Encoding → UTF-8** → `Definitions.csv`. İçerik TAB ayrımlı kalır.
4. PostgreSQL test bağlantısında `02_POSTGRES_STAGING.sql` çalıştır, geçici yardımcı `mskbs."_SimpleDefinitionsImport"` tabloya DBeaver CSV Import Data ile dosyayı yükle. 4 ayrı kolon görünmeli.
5. `03_POSTGRES_INSERT.sql` dosyasını **tek script** olarak çalıştır. Kod tekrarı, isim boşluğu, boolean ve sayısal kod kontrolleri vardır. Başarıyla COMMIT edilmezse PostgreSQL işlemini ROLLBACK yap.
6. `04_POSTGRES_DOGRULAMA.sql` dosyasını çalıştır; kontrol sonrası yardımcı tabloyu isteğe bağlı kaldır.

**İlgili sınıflar:** AnnotationReasonDefs, BranchDefs, FlyingMissionDefs, HcConnectedDecisionDefs, ICD10Defs, PlaneTypeDefs, ReportTypeDefs, RankGroupDefs, ProcedureTypeDefs, SYYDiagnosisTypeDefs, SYYDiagnosisSliceDefs, ForcesCommandDefs, MilitaryClassDefs.

**Veri bütünlüğü:** Eski Oracle kaynağında ek ExternalCode, ShortName gibi alanlar varsa bu sürüm onları **aktarmıyor**; aktarım öncesi kolon envanterine göre genişletmek gerekir. Eski gerçek CreatedDate/audit verileri taşınmaz; `CreatedDate=now()`, `IsDeleted=false`, `IsActive` eski kaynaktan alınır. `Id` hedef bazında deterministik MD5 UUID'dir. Önceden eklenmiş mevcut kayıtları **güncellemez**, mevcut ID'leri korur. Silinmiş kayıtlar için yeniden kod kullanımı özel incelenmelidir. Başka tabloların ilişkileri bozulmasın diye gerçek tablolar temizlenmez.

**Not:** Aşağıdaki listede zorunlu çoklu boolean, alt tablo veya FK içeren tanımlar YOKTUR: HcAdmissionTypeDef, HcDecisionDef, ReasonForExaminationDef, ProcedureDef, SYYDiagnosisDef vb. Bunlar ilişkili/özel aktarım gruplarına ayrılmalıdır.

Bu scriptler gerçek Oracle/PostgreSQL verisiyle test edilmemiştir; yalnızca test ortamında ve yedekle kullanın. Gerçek CSV/XLS ve kurumsal veri public GitHub deposuna konmaz.
