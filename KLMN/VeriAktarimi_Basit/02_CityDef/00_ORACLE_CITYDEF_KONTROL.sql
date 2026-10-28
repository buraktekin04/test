-- Oracle / MSKBSTEST - sadece okur, hiçbir tabloya yazmaz.
-- 01 export dosyası şu kaynak kolonları bekliyor:
-- CITYDEF: CODE, NAME, EXTERNALCODE, COUNTRY, ISACTIVE
-- COUNTRYDEF: OBJECTID, CODE
-- Bu kolonların gerçek adlarını ve tiplerini 1-2 ile doğrulayın.

-- 1. Kaynak tablolar gerçekten var mı?
SELECT owner, table_name
FROM all_tables
WHERE owner='MSKBSTEST' AND table_name IN ('CITYDEF','COUNTRYDEF')
ORDER BY table_name;

-- 2. İlişki ve diğer alanların tipini kontrol edin.
SELECT table_name, column_name, data_type, data_length, nullable
FROM all_tab_columns
WHERE owner='MSKBSTEST'
  AND table_name IN ('CITYDEF','COUNTRYDEF')
ORDER BY table_name,column_id;

-- 3. Oracle tanımlı FK varsa kaynağını tespit edin.
SELECT fk.table_name AS child_table,
       fkc.column_name AS child_column,
       pk.table_name AS parent_table,
       pkc.column_name AS parent_column
FROM all_constraints fk
JOIN all_cons_columns fkc
 ON fkc.owner=fk.owner AND fkc.constraint_name=fk.constraint_name
JOIN all_constraints pk
 ON pk.owner=fk.r_owner AND pk.constraint_name=fk.r_constraint_name
JOIN all_cons_columns pkc
 ON pkc.owner=pk.owner AND pkc.constraint_name=pk.constraint_name
 AND pkc.position=fkc.position
WHERE fk.owner='MSKBSTEST'
  AND fk.table_name='CITYDEF'
  AND fk.constraint_type='R'
ORDER BY fkc.position;

-- 4. Kontrol: kaynak şehir sayısı, boş/uzun kod ve ad.
SELECT
 COUNT(*) AS city_count,
 SUM(CASE WHEN TRIM(TO_CHAR(c.CODE)) IS NULL THEN 1 ELSE 0 END) AS empty_code,
 SUM(CASE WHEN TRIM(c.NAME) IS NULL THEN 1 ELSE 0 END) AS empty_name,
 SUM(CASE WHEN LENGTH(TRIM(TO_CHAR(c.CODE)))>100 THEN 1 ELSE 0 END) AS long_code,
 SUM(CASE WHEN LENGTH(TRIM(c.NAME))>250 THEN 1 ELSE 0 END) AS long_name,
 SUM(CASE WHEN LENGTH(TRIM(c.EXTERNALCODE))>100 THEN 1 ELSE 0 END) AS long_external
FROM MSKBSTEST.CITYDEF c;

-- 5. Kod tekrarı hedef PostgreSQL unique index'te hata verir.
SELECT UPPER(TRIM(TO_CHAR(c.CODE))) AS normalized_code,COUNT(*) AS row_count
FROM MSKBSTEST.CITYDEF c
GROUP BY UPPER(TRIM(TO_CHAR(c.CODE)))
HAVING COUNT(*)>1
ORDER BY row_count DESC;

-- 6. Kaynak aktiflik değerlerini inceleyin.
SELECT UPPER(TRIM(TO_CHAR(c.ISACTIVE))) AS source_is_active,COUNT(*) AS row_count
FROM MSKBSTEST.CITYDEF c
GROUP BY UPPER(TRIM(TO_CHAR(c.ISACTIVE)))
ORDER BY source_is_active;

-- 7. FK kontrol: Sadece CITYDEF.COUNTRY gerçekten COUNTRYDEF.OBJECTID
-- referansıysa çalıştırın. Özellikle country_not_found = 0 olmalı.
SELECT COUNT(*) AS city_count,
 SUM(CASE WHEN c.COUNTRY IS NULL THEN 1 ELSE 0 END) AS no_country,
 SUM(CASE WHEN c.COUNTRY IS NOT NULL AND u.OBJECTID IS NULL THEN 1 ELSE 0 END) AS country_not_found,
 SUM(CASE WHEN u.OBJECTID IS NOT NULL THEN 1 ELSE 0 END) AS matched_country
FROM MSKBSTEST.CITYDEF c
LEFT JOIN MSKBSTEST.COUNTRYDEF u ON u.OBJECTID=c.COUNTRY;

-- 8. Eşleşmeyen örnekleri gösterin. Sıfır kayıt dönmeli.
SELECT c.CODE AS city_code,c.NAME AS city_name,c.COUNTRY AS old_country
FROM MSKBSTEST.CITYDEF c
LEFT JOIN MSKBSTEST.COUNTRYDEF u ON u.OBJECTID=c.COUNTRY
WHERE c.COUNTRY IS NOT NULL AND u.OBJECTID IS NULL
  AND ROWNUM<=30;

-- Not: FK COUNTRYDEF.CODE'a gidiyorsa 01 yerine 01B CODE varyantını
-- kullanın; üstteki 7-8 sorgularını da ON u.CODE=c.COUNTRY biçiminde çalıştırın.
