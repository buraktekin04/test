-- PostgreSQL TEST veritabaninda calistirin.
-- Yeni tablo EF migration ile zaten olusmus olmali.
-- Bu dosya INSERT/UPDATE/DELETE/DROP yapmaz; sadece kontrol eder.

-- 1) Hedef tablo hangi schema'da?
SELECT table_schema,table_name
FROM information_schema.tables
WHERE table_name = 'CountryDefs';

-- 2) Hedef sutunlari incele (Version fiziksel kolon degildir, xmin'dir).
SELECT table_schema,column_name,data_type,is_nullable,column_default
FROM information_schema.columns
WHERE table_name = 'CountryDefs'
ORDER BY ordinal_position;

-- Asagidaki ornekler hedef 'public' schema'daysa calisir.
-- Tablo 'mskbs' altindaysa "public" yerine "mskbs" yazin.

-- 3) Import'tan ONCE test hedefinin bos oldugunu kontrol et:
SELECT COUNT(*) AS before_import FROM public."CountryDefs";

-- 4) Import'tan SONRA kayit sayisi ve temel alan kontrolu:
SELECT
    COUNT(*) AS total_count,
    COUNT(DISTINCT "Id") AS distinct_ids,
    COUNT(DISTINCT UPPER(TRIM("Code"))) AS distinct_codes,
    COUNT(*) FILTER (WHERE "CreatedDate" IS NULL) AS missing_created_date,
    COUNT(*) FILTER (WHERE "Code" IS NULL OR TRIM("Code") = '') AS missing_code,
    COUNT(*) FILTER (WHERE "Name" IS NULL OR TRIM("Name") = '') AS missing_name
FROM public."CountryDefs";

-- 5) Aynı normalize kodu birden cok kayit tasiyor mu?
SELECT UPPER(TRIM("Code")) AS normalized_code, COUNT(*) AS row_count
FROM public."CountryDefs"
WHERE "IsDeleted" = false
GROUP BY UPPER(TRIM("Code"))
HAVING COUNT(*) > 1;

-- 6) KLMN'nin ulke listesinde ornekleri gor:
SELECT "Id", "Code", "Name", "ExternalCode", "ShortCode",
       "IsTownRequired", "CreatedDate", "IsActive", "IsDeleted"
FROM public."CountryDefs"
ORDER BY "Code"
LIMIT 20;
