-- Tek Oracle SELECT sonucu = 3 tabloya ait 6 kolonlu TEK dosya.
-- MSKBSTEST.CITYDEF.COUNTRY -> COUNTRYDEF.OBJECTID
-- MSKBSTEST.TOWNDEF.CITYDEF -> CITYDEF.OBJECTID
-- Bu ilişki alanlarının doğruluğunu 00_ORACLE_KAYNAK_KONTROL.sql ile kontrol edin.
-- Oracle tablo adları/alanları farklıysa bu sorguyu düzenleyin.
--
-- RecordType, Code, Name, ExternalCode, ParentCode, IsActive
-- COUNTRY: ParentCode NULL
-- CITY: ParentCode = CountryDef.Code (eski ObjectId değil!)
-- TOWN: ParentCode = CityDef.Code (eski ObjectId değil!)
-- Id üretimi ve gerçek FK eşlemesi PostgreSQL'de yapılır.
--
-- Her satır tek bir kaynak tanımı temsil eder; join sebebiyle tekrar yoktur.
-- İlişkisi bulunamayanlar hata belirteci üretir, asla sessizce NULL'a çevrilmez.
WITH source_rows AS
(
 SELECT
    1 AS sort_order,
    'COUNTRY' AS record_type,
    UPPER(TRIM(TO_CHAR(p.CODE))) AS row_code,
    TRIM(p.NAME) AS row_name,
    TRIM(p.EXTERNALCODE) AS external_code,
    CAST(NULL AS VARCHAR2(100)) AS parent_code,
    CASE
       WHEN p.ISACTIVE IS NULL THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(p.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(p.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
       ELSE 'INVALID_STATUS'
    END AS is_active
 FROM MSKBSTEST.COUNTRYDEF p

 UNION ALL

 SELECT
    2,'CITY',
    UPPER(TRIM(TO_CHAR(c.CODE))),
    TRIM(c.NAME),
    TRIM(c.EXTERNALCODE),
    CASE
      WHEN c.COUNTRY IS NULL THEN NULL
      WHEN p.OBJECTID IS NULL THEN '__MISSING_COUNTRY__'
      ELSE UPPER(TRIM(TO_CHAR(p.CODE)))
    END,
    CASE
       WHEN c.ISACTIVE IS NULL THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
       ELSE 'INVALID_STATUS'
    END
 FROM MSKBSTEST.CITYDEF c
 LEFT JOIN MSKBSTEST.COUNTRYDEF p ON p.OBJECTID=c.COUNTRY

 UNION ALL

 SELECT
    3,'TOWN',
    UPPER(TRIM(TO_CHAR(t.CODE))),
    TRIM(t.NAME),
    TRIM(t.EXTERNALCODE),
    CASE
      WHEN t.CITYDEF IS NULL THEN NULL
      WHEN c.OBJECTID IS NULL THEN '__MISSING_CITY__'
      ELSE UPPER(TRIM(TO_CHAR(c.CODE)))
    END,
    CASE
       WHEN t.ISACTIVE IS NULL THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(t.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(t.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
       ELSE 'INVALID_STATUS'
    END
 FROM MSKBSTEST.TOWNDEF t
 LEFT JOIN MSKBSTEST.CITYDEF c ON c.OBJECTID=t.CITYDEF
)
SELECT
    record_type AS "RecordType",
    row_code AS "Code",
    row_name AS "Name",
    external_code AS "ExternalCode",
    parent_code AS "ParentCode",
    is_active AS "IsActive"
FROM source_rows
ORDER BY sort_order,row_code;

-- Burası gerçek XLS çıkışı içindir; Oracle uygulaması doğrudan CSV
-- vermiyorsa XLS -> Excel Unicode Metin *.txt -> VS Code UTF-8 kullanın.
-- Uzantıyı CSV yaparken içerik TAB ayrımlı kalır.
