# KLMN — CountryDef, CityDef, TownDef tek dosyada

**Tek Oracle sorgusu → tek Excel/XLS → tek UTF-8 TAB ayrımlı CSV → DBeaver'da tek import → PostgreSQL'de tek aktarım SQL'i.**

Üç gerçek tabloya tek CSV'nin aynı anda doğrudan import edilmesi mümkün olmadığı için **yalnızca bir yardımcı tablo** kullanılır: `mskbs."_GeographyImport"`. Yeni `legacy` şeması, migration veya uygulama kodu eklenmez; aktarım sonunda yardımcı tabloyu silebilirsiniz.

## 1) Eski kaynak ilişkisi
Oracle bağlantısında `00_ORACLE_KAYNAK_KONTROL.sql` çalıştırın. `01_ORACLE_TEK_SORGU_EXPORT.sql` şu eski FK'leri varsayar:
- `CITYDEF.COUNTRY = COUNTRYDEF.OBJECTID`
- `TOWNDEF.CITYDEF = CITYDEF.OBJECTID`

**Bunları gerçek Oracle şemasını görmeden kesin doğrulayamıyoruz.** Kaynakta `CITYDEF` yerine `CITY` ya da parent PK `CODE` ise scripti gerçek ilişkiye göre değiştirin. Tahminle import yapmayın. 00 dosyasındaki iki `missing_parent` sayısı sıfır olmalı.

## 2) Tek Oracle export
`01_ORACLE_TEK_SORGU_EXPORT.sql` sorgusunu çalıştırın. Çıktıdaki altı başlık: `RecordType, Code, Name, ExternalCode, ParentCode, IsActive`. `RecordType`: COUNTRY, CITY, TOWN. `ParentCode` şehir için ülkenin, ilçe için şehrin **Code** alanıdır; eski ObjectId değildir.

Oracle uygulamasından sonuçları **XLS** olarak dışa aktarın. Excel'de Türkçe harfler doğruysa **Farklı Kaydet → Unicode Metin (*.txt)** kullanın. Dosyayı VS Code'da açıp **Save with Encoding → UTF-8** ile kaydedin; `Geography.csv` adını verebilirsiniz. Dosya TAB ayrımlı kalmalıdır. UTF-8 olarak kapatıp açarak Türkçe karakterleri kontrol edin.

## 3) Tek DBeaver import
PostgreSQL **test** veritabanında `02_POSTGRES_TEK_IMPORT_TABLOSU.sql` çalıştırın. Hedefler hâlihazırda `mskbs` şemasında olmalı. `mskbs."_GeographyImport"` tablosunun sıfır kayıtlı olduğundan emin olun. DBeaver'da bu **yardımcı tabloya** sağ tıklayın → Import Data → CSV:
- Dosya: `Geography.csv`
- Encoding: UTF-8
- Column delimiter: TAB (\t)
- Header position: Top
- Mapping: Existing
- Kolonlar: tam 6 ayrı sütun
- Alakasız biçimde `CountryDefs`, `CityDefs` veya `TownDefs` tablolarına **doğrudan** CSV yüklemeyin.

## 4) Tek SQL ile üç tabloyu doldur
DBeaver SQL Editor'da `03_POSTGRES_UC_TABLOYA_AKTAR.sql` dosyasını **tek script olarak** çalıştırın. SQL kaynak doğrulamaları yapar, ülke → şehir → ilçe sırasıyla INSERT uygular ve en sonunda FK eşleşmesini denetler. Hata olursa transaction'ı **ROLLBACK** edin; `COMMIT` başarıyla gerçekleşmediyse işlem tamamlanmış sayılmaz.

- Önceden aktarılmış `CountryDefs` kayıtlarının `Id` değerleri **korunur**; varsa aynı `Code` yeniden eklenmez.
- Şehir `CountryId` değerini doğrudan PostgreSQL'de `CountryDefs.Code` ile bulur. İlçe `CityId` değerini PostgreSQL'de `CityDefs.Code` ile bulur. **İlişkiyi hesaplanan GUID'ye göre tahmin etmez.**
- Yeni şehir ve ilçeler için sabit GUID üretilir; `CreatedDate=now()`, `IsActive` Oracle'dan, `IsDeleted=false`.
- Mevcut kayıtların adı/aktifliği **güncellenmez**. Kod çakışmaları kısmi unique indeks nedeniyle veri tabanında hata verebilir; kaynak kodlarının tekil olması şarttır.
- `ShortCode`, `IsTownRequired`, `MernisCode`, `ValidFrom`, `ValidTo` ve nullable audit kolonları eksik kaynak tanımı nedeniyle NULL/default bırakılır; `xmin` PostgreSQL'e aittir. Eski kayıtlardaki tarih/mernis bilgileri ayrıca doğrulanırsa sonraki sürüme eklenebilir.

## 5) Doğrulama / temizlik
`04_POSTGRES_DOGRULAMA_VE_TEMIZLIK.sql` dosyasındaki sayım ve join kontrollerini çalıştırın. Beklenen ülke-şehir-ilçe satır sayılarını kıyaslayın. Gerçek tablo ve FK'lerin doğru olduğunu doğruladıktan sonra isteğe bağlı:
```sql
DROP TABLE mskbs."_GeographyImport";
```
Bu yalnızca yardımcı tabloyu kaldırır, üç asıl tabloyu **silmez**.

**Güvenlik:** Kaynak Oracle/PG veritabanı bu ortamda erişilebilir değil ve SQL'ler çalıştırılarak sınanmadı. İlk denemeyi test veritabanında yapın. Orijinal XLS/CSV/kişisel-kurumsal verileri herkese açık GitHub deposuna göndermeyin. Production'da yedeksiz import yapmayın.
