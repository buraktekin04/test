-- Oracle'da (MSKBSTEST bağlantısında) ilk önce bu sorguları çalıştırın.
-- Kullanılan örnek kaynak tablo: MSKBSTEST.COUNTRYDEF
-- Tablo farklı adlandırılmışsa 01_ORACLE_COUNTRYDEF_EXPORT.sql içinde düzeltin.

-- 1) Tablo gerçekten var mı?
SELECT owner, table_name
FROM all_tables
WHERE owner = 'MSKBSTEST'
  AND upper(table_name) LIKE '%COUNTRY%';

-- 2) Kolonları kontrol: CODE, NAME, EXTERNALCODE, ISACTIVE var mı?
SELECT owner, table_name, column_name, data_type, nullable
FROM all_tab_columns
WHERE owner = 'MSKBSTEST'
  AND upper(table_name) = 'COUNTRYDEF'
ORDER BY column_id;

-- 3) Boş kod/ad ve uzunluk hataları (aktarmadan önce sıfır olmalı).
SELECT
  COUNT(*) AS total_count,
  SUM(CASE WHEN TRIM(TO_CHAR(c.CODE)) IS NULL THEN 1 ELSE 0 END) AS missing_code,
  SUM(CASE WHEN TRIM(c.NAME) IS NULL THEN 1 ELSE 0 END) AS missing_name,
  SUM(CASE WHEN LENGTH(TRIM(TO_CHAR(c.CODE))) > 100 THEN 1 ELSE 0 END) AS long_code,
  SUM(CASE WHEN LENGTH(TRIM(c.NAME)) > 250 THEN 1 ELSE 0 END) AS long_name,
  SUM(CASE WHEN LENGTH(TRIM(c.EXTERNALCODE)) > 100 THEN 1 ELSE 0 END) AS long_external_code
FROM MSKBSTEST.COUNTRYDEF c;

-- 4) KLMN Code büyük harf/trim kullandığı için aynı koddan tek kayıt kalmalı.
SELECT UPPER(TRIM(TO_CHAR(c.CODE))) AS normalized_code, COUNT(*) AS row_count
FROM MSKBSTEST.COUNTRYDEF c
GROUP BY UPPER(TRIM(TO_CHAR(c.CODE)))
HAVING COUNT(*) > 1;

-- 5) Eski aktiflik değerleri: script true/false'a çevirebiliyor mu?
SELECT TRIM(UPPER(TO_CHAR(c.ISACTIVE))) AS oracle_isactive, COUNT(*) AS row_count
FROM MSKBSTEST.COUNTRYDEF c
GROUP BY TRIM(UPPER(TO_CHAR(c.ISACTIVE)))
ORDER BY oracle_isactive;
