# KLMN — Oracle → PostgreSQL tanım verisi aktarımı

Bu klasör **yalnızca eski MSKBS'nin tanım/eşleştirme verilerini** aktarabilmek için hazırlanmıştır. Kod deposuna hiçbir gerçek veri, parola, personel/sağlık kaydı koymayın.

> **Önemli:** Yeni KLMN'de `BaseEntity.Id` GUID'dir. Oracle kaynak kolonları ve gerçek ilk migration bu ortamda görülmediğinden, eski tablo adları otomatik eşleştirilir ama **eşleşmeler elle doğrulanmadan hedefe yüklenmez**. Özellikle FK alanının eski kayıt ID'si mi yoksa kodu mu tuttuğunu doğrulayın. Önce boş **test** PostgreSQL veritabanında deneyin. Scriptler Oracle/PostgreSQL üzerinde fiilen test edilememiştir.

## Dosyalar
- `01_ORACLE_ENVANTER.sql` — Oracle tablo/sütun envanteri; **yalnızca metadata** CSV'si üretir.
- `02_POSTGRES_HAZIRLIK.sql` — Ayrı `legacy_import` ve `legacy_raw` şemaları, 45 tanım/eşleme tablosu manifesti, eşleme ayarları.
- `03_POSTGRES_FONKSIYONLAR.sql` — CSV'ler için staging tabloları üretir; sonradan alan / ID / FK dönüşümleri ile kontrollü aktarım.
- `04_POSTGRES_KONTROL.sql` — Kaynak-hedef eşlemeler, önkoşullar, satır sayıları ve inceleme sorguları.
- `05_ORACLE_EXPORT_LISTESI.sql` — Oracle'da kopyalanabilir `SELECT *` cümleleri; **otomatik veri indirme yapmaz**.

## Önerilen basit işlem sırası (DBeaver / DBViewer)
1. **Güvenli bir Oracle bağlantısında**, `01_ORACLE_ENVANTER.sql` içindeki **ikinci** sorguyu çalıştırın. `:ESKI_SEMA` alanına Oracle şema sahibini yazın. Sonuç grid'ini UTF-8 **CSV**, kolon başlıkları açık olacak biçimde `oracle_columns.csv` olarak dışa aktarın. Bu dosya yalnızca tablo/kolon adlarını içerir.
2. **Yeni test PostgreSQL veritabanında** KLMN'nin *kendi* `InitialCreate` migration'ı uygulanmış olmalı. `02_POSTGRES_HAZIRLIK.sql` ve `03_POSTGRES_FONKSIYONLAR.sql` dosyalarını çalıştırın. **Migration üretmeyin; veritabanını sıfırlamayın.**
3. PostgreSQL `legacy_import.oracle_columns` tablosuna CSV'yi DBeaver `Import Data > CSV` ile, başlık adına göre aktarın. `column_id` bir tamsayıdır. Bu işlem tanım verisi değil, yalnızca Oracle kolon envanteridir.
4. PostgreSQL'de `CALL legacy_import.auto_match_tables();` çalıştırın. `04_POSTGRES_KONTROL.sql` sorgularıyla eşleşen **Oracle kaynak tablo adlarını**, hedef tablo/kolon ve FK planını kontrol edin. İsimleri farklı olanları `UPDATE legacy_import.table_map SET source_table = 'GERCEK_ORACLE_TABLO_ADI' WHERE target_table = '...';` ile düzeltin. Oracle'da olmayan yeni ekranlara karşılık gelenler için `enabled=false` yapın. Kaynakta aynı adla birden çok olasılık varsa otomatik eşlenmez.
5. `CALL legacy_import.prepare_staging();` çalıştırın. `legacy_raw` altında eski Oracle tablolarının kolonlarıyla **tamamı TEXT** olan boş staging tabloları oluşur.
6. `05_ORACLE_EXPORT_LISTESI.sql` çıktısından **yalnızca eşlediğiniz kaynak tabloların** `SELECT *` sorgularını Oracle'da çalıştırıp **her tablo için ayrı CSV** dışa aktarın. DBeaver'da ilgili `legacy_raw."ESKI_TABLO"` üzerine `Import Data > CSV` ile yükleyin. Başlıkları kolon adına göre eşleyin. Tarihler ISO formatında (örn. `2026-10-28 09:30:00` ya da timezone içeren ISO), boş değerler NULL, CSV UTF-8/virgül/çift tırnak olmalıdır. **Excel XLSX yerine CSV tercih edin:** kodlarda baştaki sıfır ve büyük kimlikler bozulmasın.
7. `SELECT * FROM legacy_import.report_counts();` ile staging sayımlarını kontrol edin. Önce tüm eşlemeleri, `legacy_import.column_map` dönüşümlerini, legacy ID/FK kaynak tiplerini doğrulayın. **Kimlik alanı bulunmayan kaynak tabloların `source_pk` alanını gerçek benzersiz anahtarına göre doldurun.**
8. **Yalnızca testte**, hedef tanım tabloları boşken: `SELECT legacy_import.load_all();` çalıştırın. PostgreSQL tek SQL komutunu atomik çalıştırır: herhangi bir ilişkisel/kısıt hatasında bu çağrıdaki tüm aktarım geri alınır. Herhangi bir `TRUNCATE`, `DROP`, silme, `UPDATE` ile mevcut hedef veri ezme, migration değiştirme işlemi yapılmaz.
9. Kontrol sorgularını yeniden çalıştırın; FK, benzersiz kod, tarih, aktif/pasif, Türkçe karakterler ve kullanıcı arayüzündeki örnek kayıtları kontrol edin. Test başarılıysa **ayrı yedekli gerçek ortama** kontrollü tekrar uygulayın.

## Dönüşüm kuralları
- Hedef `Id` kolonları için `md5('KLMN_LEGACY_V1|' || hedef_tablo || '|' || eski_ID)::uuid` kullanılır. Bu kimlik **deterministiktir**, aynı kaynak satır her zaman aynı GUID'yi üretir; Oracle ID'leri sayısal olsa da PostgreSQL UUID ile bağlanır. Eski ID boş/tekrarlıysa aktarım durur.
- Normal sütunlar, eski/yeni sütun adları altçizgi/büyük-küçük harf farkları giderilerek otomatik bulunur. Yeniden adlandırmalar için `column_map` kullanın.
- GUID FK değerleri varsayılan olarak **eski satır ID'si** kabul edilir. FK gerçekte `Code` saklıyorsa `column_map.mode='code'` kullanılmalıdır. Bu üç özel dönüşüm **örnek olarak** ön tanımlıdır: `ProcedureDefs.ProcedureTypeDefId ← ProcedureType`, `SYYDiagnosisDefs.SYYDiagnosisTypeDefId ← SYYDiagnosisType`, `SYYDiagnosisDefs.SYYDiagnosisSliceDefId ← Slice`. Kaynak kodun ne sakladığını teyit edin.
- Aynı tabloyu gösteren nullable self-FK (`AttachedRecNo`, `ConnectedTopUnity`, `RefereeHospitalId`) kayıtlar eklendikten sonra bağlanır.
- Bool için `1/0`, `Y/N`, `E/H`, `TRUE/FALSE`, `EVET/HAYIR` çevrilir. Tanınmayan değer hata verir.
- Oracle tarihleri timezone içermiyorsa `legacy_import.settings.source_timezone` değeriyle yorumlanır (varsayılan `Europe/Istanbul`). **Kaynak tarihlerin gerçek saat dilimini** doğrulamadan aktarım yapmayın. `CreatedDate` kaynakta yoksa aktarım anı kaydedilir.
- `xmin` / `Version` PostgreSQL tarafından yönetilir; **asla** import edilmez. `CreatedBy/UpdatedBy/DeletedBy` kullanıcı GUID'leri yeni sisteme taşınmaz, bu alanlar boş bırakılır.
- Zorunlu hedef alan kaynakta yoksa, FK hedefi bulunamazsa, boş olmayan hedef tabloda kayıt varsa veya tür dönüşümü/unique indeks başarısız olursa işlem durur. Boş olmayan hedefe otomatik upsert/çakışma bastırma **yoktur**.

## Kapsam ve sınır
Manifest KLMN'de mevcut tanım ve ilişki tablolarını kapsar. **Kullanıcılar, roller, izinler, parola hash'leri, token'lar, audit logları, sağlık/personel işlem verileri** kapsam dışıdır. Identity seed ve erişim yetkileri yeni KLMN'de oluşturulmalıdır. Bu paket **Oracle şemasına bağlanamadığından** kolonların anlamını, eski enum/karar kodlarını, tarih formatlarını ve gerçekten hangi tabloların aktarılacağını tek başına teyit edemez. `source_pk` ve FK eşlemesini Oracle envanterine göre elle gözden geçirmek şarttır.

### Özellikle incelenecek FK alanları
`MilitaryUnitDefs.ForcesCommandDef`, `MilitaryUnitDefs.ConnectedTopUnity`, `MilitaryClassGroupDefs.ForcesCommandId`, `SYYClassificationDefs` alanları ve yeni SYY/Procedure GUID ilişkileri eski Oracle'daki **sayısal kod** değerini tutuyorsa `code` eşlemesi yapın. Yeni hedef Foreign Key ilişkileri EF migration'dan okunur; **eski alanın anlamını** otomatik kesinleştirmek mümkün değildir.

## PostgreSQL şemasını belirleme
```sql
SELECT table_schema, table_name
FROM information_schema.tables
WHERE table_name IN ('CountryDefs', 'ProcedureDefs', 'SYYDiagnosisDefs');
```
Tüm tablolar `mskbs` altındaysa `UPDATE legacy_import.settings SET value='mskbs' WHERE key='target_schema';`, `public` altındaysa `public` kullanın.

## Veri güvenliği
Oracle envanter CSV'sini ve eski sistemden aldığınız **gerçek verileri public GitHub'a pushlamayın**. Bu klasöre `.gitignore` konuldu. Sadece kurumca yetkilendirilmiş, gerekli tanım verileri taşınmalı; aktarım dosyaları kurumun onaylı kapalı ortamında tutulmalı ve iş bitince güvenle silinmelidir.
