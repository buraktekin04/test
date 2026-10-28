-- KLMN CountryDefs icin Oracle TXT / CSV disari aktarma sorgusu.
-- Eski tablo: MSKBSTEST.COUNTRYDEF
-- Once 00_ORACLE_KONTROL.sql: CODE, NAME, EXTERNALCODE, ISACTIVE kolonlarini dogrulayin.
-- Sadece bu SELECT sonucunu DBeaver / DBViewer'da UTF-8 ayiracli TXT/CSV olarak export edin.
-- Ilk satir kolon adlari: Id,Code,Name,ExternalCode,CreatedDate,IsActive,IsDeleted.
-- Kullanici kimlikleri ve xmin / Version alinmaz.
-- GUID, normalize Code ile deterministiktir: ayni ulke her export'ta ayni Id'yi alir.
-- CreatedDate: orijinal kayit zamani degil, disari aktarma aninin UTC tarihi.

WITH kaynak AS (
    SELECT
        UPPER(TRIM(TO_CHAR(c.CODE))) AS country_code,
        TRIM(c.NAME) AS country_name,
        TRIM(c.EXTERNALCODE) AS external_code,
        CASE
            WHEN c.ISACTIVE IS NULL THEN 'true'
            WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1', 'TRUE', 'T', 'Y', 'YES', 'E', 'EVET')
                THEN 'true'
            WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0', 'FALSE', 'F', 'N', 'NO', 'H', 'HAYIR')
                THEN 'false'
            ELSE 'INVALID_ISACTIVE'
        END AS is_active_text
    FROM MSKBSTEST.COUNTRYDEF c
),
kimlik AS (
    SELECT
        k.*,
        LOWER(RAWTOHEX(
            STANDARD_HASH('KLMN_COUNTRYDEF_V1|' || k.country_code, 'MD5')
        )) AS id_hex
    FROM kaynak k
)
SELECT
    SUBSTR(id_hex, 1, 8) || '-' ||
    SUBSTR(id_hex, 9, 4) || '-' ||
    SUBSTR(id_hex, 13, 4) || '-' ||
    SUBSTR(id_hex, 17, 4) || '-' ||
    SUBSTR(id_hex, 21, 12) AS "Id",
    country_code AS "Code",
    country_name AS "Name",
    external_code AS "ExternalCode",
    TO_CHAR(
        SYS_EXTRACT_UTC(SYSTIMESTAMP),
        'YYYY-MM-DD"T"HH24:MI:SS.FF3'
    ) || '+00:00' AS "CreatedDate",
    is_active_text AS "IsActive",
    'false' AS "IsDeleted"
FROM kimlik
ORDER BY country_code;

-- Ornek metin tarih formati: 2026-10-28T10:30:00.123+00:00
-- Postgres "timestamp with time zone" icin gecerli ISO 8601 bicimi.
-- TXT yalnizca sabit sutun genislikli rapor olmamali:
-- CSV veya TAB-separated olarak ayiracli dosya olusturun.
-- Eger EXTERNALCODE veya ISACTIVE kaynakta YOKSA scripti rastgele degistirmeyin;
-- 00_ORACLE_KONTROL.sql sonucuna gore alan eslestirmesini duzeltin.
