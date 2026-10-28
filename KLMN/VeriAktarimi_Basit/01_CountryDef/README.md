# 01 — CountryDef (Oracle → PostgreSQL, doğrudan CSV/TXT import)

**Ek legacy/staging şeması yoktur.** PostgreSQL'deki gerçek `CountryDefs` tablosuna DBeaver / DBViewer üzerinden doğrudan **Import Data → CSV** yaparız. Var olan KLMN migration'larına ve koduna dokunulmaz.

## Kaynak / hedef
- Oracle: `MSKBSTEST.COUNTRYDEF` (ad ve sütunları 00 sorgusunda kontrol et)
- PostgreSQL: gerçek şema altındaki `"CountryDefs"` (çoğu kurulumda `public`; sorguyla teyit et)
- Hedefe aktarılan kolonlar: `Id,Code,Name,ExternalCode,CreatedDate,IsActive,IsDeleted`
- `ShortCode,IsTownRequired`: yeni alanlar, kaynak verisi doğrulanmadığından NULL
- `CreatedBy,UpdatedBy,UpdatedDate,DeletedDate,DeletedBy`: NULL; kaynak kullanıcı GUID'lerini taşımıyoruz
- `Version`: fiziksel sütun değil, PostgreSQL tarafından yönetilen `xmin`; dosyada **yok**

## Adımlar
1. Oracle'da `00_ORACLE_KONTROL.sql` çalıştır. Tablo/kolon adlarını ve tekrarlı `Code` değerlerini kontrol et. Hatalar sıfır olmalı. `ISACTIVE` yalnızca 0/1, true/false, Y/N, E/H gibi desteklenen değerleri içermeli.
2. Oracle'da **yalnızca** `01_ORACLE_COUNTRYDEF_EXPORT.sql` içindeki ana `WITH ... SELECT` sorgusunu çalıştır. Sonuç grid'ini **Export Data → CSV** ile export et. Dosya uzantısı `.txt` olabilir; **kolonları ayıran** bir format seç (tercihen CSV virgül; alternatif TAB). Encoding **UTF-8**, **Include column headers** açık, metin kaçış/çift tırnak ayarı doğru olmalı. Bir metinde virgül, noktalı virgül, çift tırnak veya satır sonu varsa CSV quoting şarttır; sabit genişlikli 'TXT rapor' formatı uygun değildir.
3. PostgreSQL test bağlantısında `02_POSTGRES_COUNTRYDEF_KONTROL.sql` içindeki ilk sorguyla gerçek şemayı kontrol et. `CountryDefs` tablosu migration ile mevcut ve test ortamında **boş** olmalı.
4. DBeaver'da bu gerçek tabloya sağ tıkla → **Import Data → CSV** → az önce dışa attığın `.txt`/CSV dosyasını seç. Önizlemede **7 kolon** görünmeli. Delimiter/encoding ve ilk satır başlık ayarını export ile aynı yap. Dosya kolonlarını aynı adlı hedef alanlara bağla. Diğer PostgreSQL alanlarını **eşleme/atla** (NULL/default kalacak).
5. `CreatedDate` zaman dilimli tarih alanıdır. Örnek CSV: `2026-10-28T10:30:00.123+00:00`. DBeaver tarih formatı isterse ISO 8601 için `yyyy-MM-dd'T'HH:mm:ss.SSSXXX` biçimini seç. `IsActive` ve `IsDeleted` için `true/false` desteklenmelidir. CSV boş hücreleri NULL olarak yorumlanmalıdır.
6. Önizlemede GUID'ler `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` biçiminde mi, Türkçe karakterler bozuluyor mu kontrol et. **Hataları atlayarak kısmi yükleme** yapma.
7. Import sonrası `02_POSTGRES_COUNTRYDEF_KONTROL.sql` kontrol sorgularını çalıştır ve Oracle satır sayısıyla kıyasla.

## GUID / sonraki CityDef aktarımı
GUID `CountryDef.Code` üzerinden **deterministik** MD5 UUID olarak üretilir; aynı normalize kod için aynı GUID. Formülün girdisi: `KLMN_COUNTRYDEF_V1|` + `UPPER(TRIM(Code))`. Kodun normalize edilmiş hâli tekil olmalıdır; kodda yapılacak değişiklik ID eşlemesini etkiler.

**CityDef aktarımında** Oracle'ın ülke kodunu PostgreSQL `CountryDefs.Code` ile eşleyip `CountryId` için yeni ülke GUID'si kullanacağız. Oracle'ın eski ObjectId/numara kimliğini GUID FK'ye doğrudan yazmayacağız.

## Güvenlik / önkoşullar
- Tablo/kolon isimleri gerçek Oracle şemasına göre doğrulanmalıdır. `EXTERNALCODE` / `ISACTIVE` kaynağında farklı ad kullanılıyorsa ilk sorguyu düzenlemek gerekir.
- Oracle `CreatedDate` tarihi burada **korunmaz**, aktarımın UTC zamanı yazılır. Eski `CreationDate` geçmişini aktarmak istersen önce tarih biçimini ve timezone'u doğrulamalıyız.
- Oracle'da silinmiş kayıt filtresi bilinmediği için **tüm CountryDef satırları** `IsDeleted=false` olarak gelir. Yinelenen kodlar hedefte unique index hatasına neden olur; kontrol sorgusu bunu önceden bulur.
- Hedefte halihazırda kayıt varsa bu yöntem doğrudan birleşim/upsert yapmaz. Önce mevcut kayıtların korunması ve kod çakışmalarının çözümü planlanmalıdır. **Asla veritabanını silmeyin/TRUNCATE etmeyin.**
- Bu scriptler ortamın Oracle/PostgreSQL bağlantısında fiilen çalıştırılmamıştır. İlk aktarımı boş **test** veritabanında uygulayın.
- Gerçek verileri, aktardığınız TXT/CSV'leri veya bağlantı parolalarını **public GitHub'a yüklemeyin**.
