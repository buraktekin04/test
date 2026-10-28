-- PostgreSQL test veritabanı; yalnızca SELECT, veri değiştirmez.
-- Hedef şema: mskbs (ekran görüntüsüne göre). Farklıysa düzenleyin.

-- 1) CityDefs tablosu ve kolon tipleri.
SELECT table_schema,table_name
FROM information_schema.tables WHERE table_name IN ('CountryDefs','CityDefs');
SELECT table_schema,column_name,data_type,is_nullable,column_default
FROM information_schema.columns
WHERE table_schema='mskbs' AND table_name='CityDefs'
ORDER BY ordinal_position;

-- 2) EN ÖNEMLİ KONTROL: Daha önce aktardığınız CountryDefs.Id
-- daha önce hazırladığımız CountryDef Oracle export GUID formülü ile
-- eşleşiyor mu? Eşleşmeyen satır sayısı 0 OLMALI!
SELECT COUNT(*) AS total_countries,
 COUNT(*) FILTER (
   WHERE "Id" <> md5('KLMN_COUNTRYDEF_V1|' || UPPER(TRIM("Code")))::uuid
 ) AS id_mismatch_count,
 COUNT(*) FILTER (WHERE "IsDeleted") AS deleted_countries
FROM mskbs."CountryDefs";

-- Eşleşmeyen ülkeler: SIFIR kayıt dönmeli.
SELECT "Code","Name","Id",
 md5('KLMN_COUNTRYDEF_V1|' || UPPER(TRIM("Code")))::uuid AS expected_id
FROM mskbs."CountryDefs"
WHERE "Id" <> md5('KLMN_COUNTRYDEF_V1|' || UPPER(TRIM("Code")))::uuid;

-- 3) Tekrar import etmeden ÖNCE hedefte kayıt var mı?
SELECT COUNT(*) AS cities_before_import FROM mskbs."CityDefs";

-- 4) Import sonrası kayıt/ID, ülke FK, durum kontrolü.
SELECT COUNT(*) AS cities_after_import,
 COUNT(DISTINCT "Id") AS distinct_ids,
 COUNT(DISTINCT UPPER(TRIM("Code"))) AS distinct_codes,
 COUNT(*) FILTER (WHERE "CountryId" IS NULL) AS cities_without_country,
 COUNT(*) FILTER (WHERE "CreatedDate" IS NULL) AS missing_created_date
FROM mskbs."CityDefs";

-- 5) FK bağı: referansı eksik şehir varsa (normalde DB FK reddeder).
SELECT ci."Code" AS city_code,ci."Name" AS city_name,
 ci."CountryId", co."Code" AS country_code,co."Name" AS country_name
FROM mskbs."CityDefs" ci
LEFT JOIN mskbs."CountryDefs" co ON co."Id"=ci."CountryId"
ORDER BY ci."Code"
LIMIT 30;

-- 6) Bağlı olduğu ülke silinmiş/pasif şehirler (iş kurallarına göre kontrol).
SELECT ci."Code",ci."Name" AS city_name,
 co."Code" AS country_code,co."Name" AS country_name,
 co."IsActive",co."IsDeleted"
FROM mskbs."CityDefs" ci
JOIN mskbs."CountryDefs" co ON co."Id"=ci."CountryId"
WHERE co."IsDeleted" OR (ci."IsActive" AND NOT co."IsActive");

-- 7) Yanlışlıkla tekrar edilen şehir kodlarını ara.
SELECT UPPER(TRIM("Code")) AS normalized_code,COUNT(*) AS row_count
FROM mskbs."CityDefs" WHERE "IsDeleted"=false
GROUP BY UPPER(TRIM("Code")) HAVING COUNT(*)>1;
