# 02 — CityDef (Şehir Tanımları) Oracle → PostgreSQL

Önce 01_CountryDef aktarımının bittiği, **gerçek mskbs."CountryDefs" tablosunda ülkelerin Id ve Code alanlarının sağlam olduğu** doğrulanmalıdır. Bu paket **hiç yeni şema/staging tablo, migration veya uygulama kodu oluşturmaz**. SQL'ler yalnızca okur; veriyi DBeaver Import Data ile siz yüklersiniz.

## İlişki nasıl kuruluyor?
KLMN yeni CityDef tablosunda `CountryId` alanı **GUID (UUID)** FK'dir; Oracle CITYDEF.COUNTRY'nin eski sayısal/GUID ObjectId'si doğrudan taşınamaz. Bu yüzden Oracle'da eski ülke referansından `COUNTRYDEF.CODE` bulunur ve **01_CountryDef export scripti ile birebir aynı** algoritmayla yeni CountryId hesaplanır:
`MD5('KLMN_COUNTRYDEF_V1|' || UPPER(TRIM(COUNTRYDEF.CODE)))` → UUID biçimi.

Şehirlere de `MD5('KLMN_CITYDEF_V1|' || UPPER(TRIM(CITYDEF.CODE)))` ile **sabit** Id üretiriz. Aynı kod aynı Id'yi verir. Eski tarih alanlarına dokunulmaz; CreatedDate aktarım anının UTC zamanıdır.

### Eski Oracle FK mutlaka doğrulanmalı!
Kaynak Oracle şemasını doğrudan göremiyoruz. **01_ORACLE_CITYDEF_EXPORT.sql şu ilişkiyi varsayıyor:**
`CITYDEF.COUNTRY = COUNTRYDEF.OBJECTID`.

Önce `00_ORACLE_CITYDEF_KONTROL.sql` ile tabloları, kolonları, varsa Oracle FK tanımını ve **eşleşmeyen ülke sayısını** kontrol edin. Kaynak alan ülkenin **Code** değerini tutuyorsa `01B_ORACLE_CITYDEF_EXPORT_COUNTRY_CODE.sql` kullanın. Eski sütun/PK ismi farklıysa export'u veritabanının gerçek kolonlarına uyarlamak gerekir; **yanlış ilişkiyle yüklemeyin.**

00 dosyasının FK kontrolünde **country_not_found = 0** olmalı. Eğer kaynakta ülkesiz şehirler bulunuyorsa `no_country` ayrıca incelenmeli. Kaynak COUNTRY dolu olduğu halde ülke bulunamayan satırlara export 'ERROR_COUNTRY_NOT_FOUND' yazar; bu dosya PostgreSQL UUID'ye geçemez, hata vermesi amaçlıdır. Hataları atlayarak import yapmayın.

## Uygulama sırası
1. **Oracle**: `00_ORACLE_CITYDEF_KONTROL.sql` çalıştırıp kaynak kolonları, tekrarlı şehir kodlarını ve ülke ilişkisini doğrulayın. `country_not_found=0`, `empty_code=0`, `empty_name=0` ve tekrarlı Code sonucu boş olmalı.
2. **PostgreSQL**: `02_POSTGRES_CITYDEF_KONTROL.sql` içindeki **2.** kontrolle ülke GUID'lerini doğrulayın: `id_mismatch_count=0` şarttır. Daha önce CountryDef elle değiştirildiyse, hedef Id formüle uymayabilir; o durumda **şehirleri henüz aktarmayın**, gerçek Id'ye göre eşlemeyi uyarlayın.
3. **Oracle**: Doğrulanmış eski FK tipine göre **01** veya **01B** export sorgusunu çalıştırın. Sonuç grid'ini Oracle uygulamanızın izin verdiği şekilde **XLS** olarak dışa aktarın.
4. **Excel**: XLS düzgün görünüyorsa **Farklı Kaydet → Unicode Metin (*.txt)** yapın. VS Code'da açıp **Save with Encoding → UTF-8** seçin. Dosyayı `CityDefs.csv` diye kaydedin; sütunlar **TAB** ayrımlı kalacak. UTF-8 olarak yeniden açıp İ/Ş/Ğ/Ü karakterlerinin düzeldiğini doğrulayın. Excel'in eski ANSI CSV seçeneği Türkçe karakterleri `?`'ye dönüştürebilir.
5. **DBeaver (PostgreSQL)**: `mskbs."CityDefs"` üzerinde sağ tık → **Import Data → CSV**; dosya `CityDefs.csv`, Encoding=**UTF-8**, Column delimiter=**TAB**, Header=**Top**, Quote=`"`, Mapping=**Existing**. CSV'de **8 ayrı başlık** görünmeli: `Id,Code,Name,ExternalCode,CountryId,CreatedDate,IsActive,IsDeleted`. Kaynak-hedef aynı adlı sütunlara eşleştirilmeli. Diğer nullable alanlar boş bırakılır. **Hataları atlayarak kısmi import yapmayın.**
6. **PostgreSQL**: `02_POSTGRES_CITYDEF_KONTROL.sql` içindeki import sonrası sorgularını çalıştırın. Şehir-ülke ilişkilerini ülke adlarıyla birlikte kontrol edin.

## İsteğe bağlı eski alanlar
Bu **ilk**, güvenli ve basit sürümde kaynak şemada gerçek adı/tipi doğrulanmamış `MernisCode`, `StartDate`, `EndDate` aktarılmıyor. Hedefte `MernisCode`, `ValidFrom`, `ValidTo` nullable'dır. Kaynak alanlar doğrulanınca bunlar için ayrı dönüşüm sorgusu eklenebilir. `CreatedBy/UpdatedBy/DeletedBy` GUID kullanıcı referansları taşınmaz; `Version` PostgreSQL'in `xmin` sistem alanıdır, import dosyasına eklenmez.

## Dikkat
- `CountryDefs` veri içermeli; `CityDefs` mevcut kayıt içeriyorsa import yapmadan önce çakışmaları kontrol edin. **TRUNCATE/DROP veya migration reset yok.**
- Kaynak ve hedef tablolarda kodların normalize/unique olması gerekir. Aktif ülke/pasif ülke durumlarını kontrol edin; yanlış bir FK, şehir seçicilerinde verilerin görünmesini etkileyebilir.
- Test DB'de deneyin ve yedek alın. PostgreSQL/Oracle bağlantıları bu ortamda erişilebilir olmadığından SQL dosyaları **gerçek verilerle çalıştırılmış değildir**.
- Gerçek verileri, CSV/XLS dosyalarını, kurum içi bağlantı bilgilerini **public GitHub'a pushlamayın**.
