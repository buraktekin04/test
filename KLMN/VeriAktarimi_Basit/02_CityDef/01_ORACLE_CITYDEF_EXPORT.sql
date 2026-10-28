-- Oracle (MSKBSTEST) -> mskbs."CityDefs" için doğrudan TXT/CSV export.
--
-- ÖNKOŞUL: 00_ORACLE_CITYDEF_KONTROL.sql ile
-- CITYDEF.COUNTRY -> COUNTRYDEF.OBJECTID ilişkisini KESİNLİKLE doğrulayın.
-- Eğer eski COUNTRY alanı ülke Code tutuyorsa 01B dosyasını kullanın.
-- Dışa aktarmadan önce country_not_found = 0 ve mükerrer Code = 0 olmalı.
--
-- Ülke CountryId'si, 01_CountryDef'de kullanılan AYNI GUID algoritmasından:
-- MD5('KLMN_COUNTRYDEF_V1|' || UPPER(TRIM(COUNTRYDEF.CODE)))
-- Şehir Id'si: MD5('KLMN_CITYDEF_V1|' || UPPER(TRIM(CITYDEF.CODE)))
-- Eski sayısal ObjectId'yi hedef Guid FK olarak taşımıyoruz.
-- Özellikle UTF-8 + TAB ayracını ve ilk satır başlıklarını koruyun.

WITH kaynak AS (
    SELECT
        UPPER(TRIM(TO_CHAR(c.CODE))) AS city_code,
        TRIM(c.NAME) AS city_name,
        TRIM(c.EXTERNALCODE) AS external_code,
        CASE
            WHEN c.COUNTRY IS NULL THEN NULL
            WHEN u.OBJECTID IS NULL THEN 'MISSING_COUNTRY'
            ELSE UPPER(TRIM(TO_CHAR(u.CODE)))
        END AS country_code,
        CASE
            WHEN c.ISACTIVE IS NULL THEN 'true'
            WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET')
                THEN 'true'
            WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR')
                THEN 'false'
            ELSE 'INVALID_ISACTIVE'
        END AS is_active_text
    FROM MSKBSTEST.CITYDEF c
    LEFT JOIN MSKBSTEST.COUNTRYDEF u
      ON u.OBJECTID = c.COUNTRY
),
kimlik AS (
    SELECT
        k.*,
        LOWER(RAWTOHEX(STANDARD_HASH('KLMN_CITYDEF_V1|' || k.city_code,'MD5'))) AS city_hex,
        CASE WHEN k.country_code IS NOT NULL THEN
          LOWER(RAWTOHEX(STANDARD_HASH('KLMN_COUNTRYDEF_V1|' || k.country_code,'MD5')))
        ELSE NULL END AS country_hex
    FROM kaynak k
)
SELECT
    SUBSTR(city_hex,1,8)||'-'||SUBSTR(city_hex,9,4)||'-'||
    SUBSTR(city_hex,13,4)||'-'||SUBSTR(city_hex,17,4)||'-'||
    SUBSTR(city_hex,21,12) AS "Id",
    city_code AS "Code",
    city_name AS "Name",
    external_code AS "ExternalCode",
    CASE WHEN country_code='MISSING_COUNTRY' THEN 'ERROR_COUNTRY_NOT_FOUND'
         WHEN country_hex IS NULL THEN NULL
         ELSE SUBSTR(country_hex,1,8)||'-'||SUBSTR(country_hex,9,4)||'-'||
              SUBSTR(country_hex,13,4)||'-'||SUBSTR(country_hex,17,4)||'-'||
              SUBSTR(country_hex,21,12)
    END AS "CountryId",
    TO_CHAR(SYS_EXTRACT_UTC(SYSTIMESTAMP),
        'YYYY-MM-DD"T"HH24:MI:SS.FF3') || '+00:00' AS "CreatedDate",
    is_active_text AS "IsActive",
    'false' AS "IsDeleted"
FROM kimlik
ORDER BY city_code;

-- Bu ilk aktarım temel alanları taşır; CityDef.MernisCode,
-- ValidFrom/ValidTo, CreatedBy/UpdatedBy/DeletedBy/UpdatedDate/DeletedDate
-- nullable olduğundan bilinçli olarak CSV'de bulunmaz (NULL kalır).
-- MernisCode/StartDate/EndDate kaynak kolonları doğrulanırsa
-- ikinci aşamada veri kaybı olmadan eklenebilir.
