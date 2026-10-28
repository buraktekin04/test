-- SADECE CITYDEF.COUNTRY alanı eski COUNTRYDEF.CODE'u tutuyorsa!
-- Önce 00_ORACLE_CITYDEF_KONTROL.sql ile kaynak ilişkiyi doğrulayın.
-- Aşağıdaki sorgu, COUNTRYDEF.OBJECTID yerine CODE ile eşleştirir.
-- Diğer tüm kurallar 01_ORACLE_CITYDEF_EXPORT.sql ile aynıdır.
WITH kaynak AS (
 SELECT
  UPPER(TRIM(TO_CHAR(c.CODE))) AS city_code,
  TRIM(c.NAME) AS city_name,
  TRIM(c.EXTERNALCODE) AS external_code,
  CASE WHEN c.COUNTRY IS NULL THEN NULL
       WHEN u.CODE IS NULL THEN 'MISSING_COUNTRY'
       ELSE UPPER(TRIM(TO_CHAR(u.CODE))) END AS country_code,
  CASE WHEN c.ISACTIVE IS NULL THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
       WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
       ELSE 'INVALID_ISACTIVE' END AS is_active_text
 FROM MSKBSTEST.CITYDEF c
 LEFT JOIN MSKBSTEST.COUNTRYDEF u
 ON UPPER(TRIM(TO_CHAR(c.COUNTRY))) = UPPER(TRIM(TO_CHAR(u.CODE)))
),
kimlik AS (
 SELECT k.*,
  LOWER(RAWTOHEX(STANDARD_HASH('KLMN_CITYDEF_V1|'||k.city_code,'MD5'))) AS city_hex,
  CASE WHEN k.country_code IS NOT NULL THEN
    LOWER(RAWTOHEX(STANDARD_HASH('KLMN_COUNTRYDEF_V1|'||k.country_code,'MD5')))
  ELSE NULL END AS country_hex
 FROM kaynak k
)
SELECT
 SUBSTR(city_hex,1,8)||'-'||SUBSTR(city_hex,9,4)||'-'||
 SUBSTR(city_hex,13,4)||'-'||SUBSTR(city_hex,17,4)||'-'||
 SUBSTR(city_hex,21,12) AS "Id",
 city_code AS "Code",city_name AS "Name",external_code AS "ExternalCode",
 CASE WHEN country_code='MISSING_COUNTRY' THEN 'ERROR_COUNTRY_NOT_FOUND'
      WHEN country_hex IS NULL THEN NULL
      ELSE SUBSTR(country_hex,1,8)||'-'||SUBSTR(country_hex,9,4)||'-'||
           SUBSTR(country_hex,13,4)||'-'||SUBSTR(country_hex,17,4)||'-'||
           SUBSTR(country_hex,21,12) END AS "CountryId",
 TO_CHAR(SYS_EXTRACT_UTC(SYSTIMESTAMP),
     'YYYY-MM-DD"T"HH24:MI:SS.FF3')||'+00:00' AS "CreatedDate",
 is_active_text AS "IsActive",
 'false' AS "IsDeleted"
FROM kimlik ORDER BY city_code;
