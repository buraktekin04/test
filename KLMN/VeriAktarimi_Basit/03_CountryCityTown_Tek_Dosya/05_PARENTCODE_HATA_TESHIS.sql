-- SQL teşhisi: COUNTRY satırları ParentCode içeremez
-- HATA SONRASI açık PostgreSQL transaction varsa önce:
ROLLBACK;

-- 1) Üç kayıt türü gerçekten ayrı ayrı aktarıldı mı?
SELECT "RecordType",count(*) AS adet
FROM mskbs."_GeographyImport"
GROUP BY "RecordType" ORDER BY "RecordType";

-- 2) COUNTRY için beklenmedik ParentCode değerleri (tırnak, boyut ve hex ile)
SELECT "Code","Name","ExternalCode","IsActive",
       quote_nullable("ParentCode") AS parent_code,
       length("ParentCode") AS karakter_sayisi,
       encode(convert_to("ParentCode",'UTF8'),'hex') AS utf8_hex
FROM mskbs."_GeographyImport"
WHERE "RecordType"='COUNTRY'
 AND nullif(btrim(coalesce("ParentCode",'')),'') IS NOT NULL
ORDER BY "Code" LIMIT 100;

-- 3) Yanlış ParentCode değerlerinin özeti
SELECT quote_nullable("ParentCode") AS parent_code,count(*) AS adet
FROM mskbs."_GeographyImport"
WHERE "RecordType"='COUNTRY'
 AND nullif(btrim(coalesce("ParentCode",'')),'') IS NOT NULL
GROUP BY "ParentCode" ORDER BY adet DESC;

-- 4) Kolonların genel olarak doğru eşlendiğinden emin ol
SELECT "RecordType","Code","Name","ExternalCode",
       quote_nullable("ParentCode") AS parent_code,"IsActive"
FROM mskbs."_GeographyImport"
ORDER BY "RecordType","Code" LIMIT 50;

-- 5) Excel'in boş ülke ParentCode alanını gerçekten NULL yerine
-- 'NULL' / '(NULL)' / '<NULL>' METNİ olarak yazdığı doğrulanırsa:
-- YALNIZCA alttaki yorum satırlarını açıp çalıştırın.
-- Başka veri (ülke kodu, şehir, GUID) varsa silmeyin; kolon mapping yanlış.
--
-- BEGIN;
-- UPDATE mskbs."_GeographyImport"
-- SET "ParentCode" = NULL
-- WHERE "RecordType" = 'COUNTRY'
--   AND upper(btrim("ParentCode")) IN ('NULL','(NULL)','<NULL>');
-- SELECT count(*) AS kalan_hata
-- FROM mskbs."_GeographyImport"
-- WHERE "RecordType"='COUNTRY'
--   AND nullif(btrim(coalesce("ParentCode",'')),'') IS NOT NULL;
-- COMMIT;
--
-- Kalan_hata sıfır olunca 03_POSTGRES_UC_TABLOYA_AKTAR.sql
-- dosyasını tekrar tek script olarak çalıştırabilirsiniz.
