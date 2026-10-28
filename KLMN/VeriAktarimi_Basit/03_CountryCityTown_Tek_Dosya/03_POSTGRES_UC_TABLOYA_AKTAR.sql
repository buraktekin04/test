-- TEST VERİTABANINDA kullanın. Üç gerçek tablo:
-- mskbs."CountryDefs", mskbs."CityDefs", mskbs."TownDefs".
--
-- Başlamadan: "02_POSTGRES_TEK_IMPORT_TABLOSU.sql" ile oluşturulan
-- mskbs."_GeographyImport" içine tek dosyayı DBeaver'dan yükleyin.
-- Bu script INSERT yapar, UPDATE/DELETE/TRUNCATE yapmaz.
-- Var olan country ID'sini saklar; city/town gerçek parent ID'sini
-- hedef tablodaki Code'a bakarak bağlar.
-- Tek transaction'da çalıştırın (DBeaver: Execute SQL Script).
-- Oracle kodları UNIQUE olmalıdır, kod değişikliği ID eşlemesini etkiler.

BEGIN;

-- Bu kontrol herhangi bir değişiklik yapılmadan önce bütün kaynak hatalarını durdurur.
DO $check$
DECLARE bad_count bigint;
BEGIN
 IF to_regclass('mskbs."_GeographyImport"') IS NULL THEN
   RAISE EXCEPTION 'Önce _GeographyImport tablosunu oluşturun';
 END IF;
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport"
   WHERE "RecordType" IS NULL
      OR "RecordType" NOT IN ('COUNTRY','CITY','TOWN')
      OR "Code" IS NULL OR btrim("Code")=''
      OR "Name" IS NULL OR btrim("Name")=''
      OR length("Code")>100 OR length("Name")>250
      OR length(coalesce("ExternalCode",''))>100
      OR "IsActive" IS NULL OR lower(btrim("IsActive")) NOT IN ('true','false')
      OR "ParentCode" IN ('__MISSING_COUNTRY__','__MISSING_CITY__')
 ) THEN
   RAISE EXCEPTION 'Kaynakta eksik kod/ad, uzun alan, geçersiz aktiflik veya bulunamayan eski FK var. _GeographyImport tablosunu inceleyin.';
 END IF;
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport"
   GROUP BY "RecordType",upper(btrim("Code"))
   HAVING count(*)>1
 ) THEN
   RAISE EXCEPTION 'Aynı RecordType için tekrar eden Code bulundu; önce kaynağı düzeltin.';
 END IF;
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport"
   WHERE "RecordType"='COUNTRY' AND nullif(btrim(coalesce("ParentCode",'')),'') IS NOT NULL
 ) THEN
   RAISE EXCEPTION 'COUNTRY satırları ParentCode içeremez.';
 END IF;
 -- Daha önce yüklenen CountryDefs ile gelen dosyanın kodları karşılaştırılır.
 -- Code çakışması eski/yeni kaydı birleştirir, mevcut Country ID DEĞİŞMEZ.
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   JOIN mskbs."CountryDefs" c
     ON upper(btrim(c."Code"))=upper(btrim(s."Code"))
   WHERE s."RecordType"='COUNTRY' AND NOT c."IsDeleted"
   GROUP BY s."Code" HAVING count(*)>1
 ) THEN
   RAISE EXCEPTION 'Hedef CountryDefs içinde mükerrer aktif ülke Code var.';
 END IF;
END
$check$;

-- 1) ÜLKELER: mevcut CountryDefs kayıtları/Id'leri korunur.
INSERT INTO mskbs."CountryDefs"
 ("Id","Code","Name","ExternalCode","CreatedDate","IsActive","IsDeleted")
SELECT
 md5('KLMN_COUNTRYDEF_V1|' || upper(btrim(s."Code")))::uuid,
 upper(btrim(s."Code")),s."Name",nullif(s."ExternalCode",''),
 now(),lower(btrim(s."IsActive"))::boolean,false
FROM mskbs."_GeographyImport" s
WHERE s."RecordType"='COUNTRY'
 AND NOT EXISTS (
   SELECT 1 FROM mskbs."CountryDefs" x
   WHERE upper(btrim(x."Code"))=upper(btrim(s."Code")) AND NOT x."IsDeleted"
 );

-- 2) Şehirler için bütün ülke kodları artık bulunmalı.
DO $parent_country$
BEGIN
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   WHERE s."RecordType"='CITY'
     AND nullif(btrim(coalesce(s."ParentCode",'')),'') IS NOT NULL
     AND NOT EXISTS (
       SELECT 1 FROM mskbs."CountryDefs" p
       WHERE upper(btrim(p."Code"))=upper(btrim(s."ParentCode"))
         AND NOT p."IsDeleted"
     )
 ) THEN
   RAISE EXCEPTION 'CITY kayıtlarında PostgreSQL CountryDefs içinde bulunmayan ParentCode var. Transaction rollback.';
 END IF;
END
$parent_country$;

INSERT INTO mskbs."CityDefs"
 ("Id","Code","Name","ExternalCode","CountryId","CreatedDate","IsActive","IsDeleted")
SELECT
 md5('KLMN_CITYDEF_V1|' || upper(btrim(s."Code")))::uuid,
 upper(btrim(s."Code")),s."Name",nullif(s."ExternalCode",''),
 p."Id",now(),lower(btrim(s."IsActive"))::boolean,false
FROM mskbs."_GeographyImport" s
LEFT JOIN mskbs."CountryDefs" p
 ON upper(btrim(p."Code"))=upper(btrim(s."ParentCode"))
 AND NOT p."IsDeleted"
WHERE s."RecordType"='CITY'
 AND NOT EXISTS (
   SELECT 1 FROM mskbs."CityDefs" x
   WHERE upper(btrim(x."Code"))=upper(btrim(s."Code")) AND NOT x."IsDeleted"
 );

-- 3) İlçeler için bütün şehir kodları artık bulunmalı.
DO $parent_city$
BEGIN
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   WHERE s."RecordType"='TOWN'
     AND nullif(btrim(coalesce(s."ParentCode",'')),'') IS NOT NULL
     AND NOT EXISTS (
       SELECT 1 FROM mskbs."CityDefs" p
       WHERE upper(btrim(p."Code"))=upper(btrim(s."ParentCode"))
         AND NOT p."IsDeleted"
     )
 ) THEN
   RAISE EXCEPTION 'TOWN kayıtlarında PostgreSQL CityDefs içinde bulunmayan ParentCode var. Transaction rollback.';
 END IF;
END
$parent_city$;

INSERT INTO mskbs."TownDefs"
 ("Id","Code","Name","ExternalCode","CityId","CreatedDate","IsActive","IsDeleted")
SELECT
 md5('KLMN_TOWNDEF_V1|' || upper(btrim(s."Code")))::uuid,
 upper(btrim(s."Code")),s."Name",nullif(s."ExternalCode",''),
 p."Id",now(),lower(btrim(s."IsActive"))::boolean,false
FROM mskbs."_GeographyImport" s
LEFT JOIN mskbs."CityDefs" p
 ON upper(btrim(p."Code"))=upper(btrim(s."ParentCode"))
 AND NOT p."IsDeleted"
WHERE s."RecordType"='TOWN'
 AND NOT EXISTS (
   SELECT 1 FROM mskbs."TownDefs" x
   WHERE upper(btrim(x."Code"))=upper(btrim(s."Code")) AND NOT x."IsDeleted"
 );

-- Bütün satırların hedefte eşleştiğini ve mevcut FK'leri kontrol edin.
-- Önceden mevcut şehir/ilçe varsa, onların ilişkileri otomatik değişmez:
-- eşleşmeyen ilişki raporu hata vererek commit'i engeller.
DO $final_check$
BEGIN
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   WHERE (s."RecordType"='COUNTRY' AND NOT EXISTS (
      SELECT 1 FROM mskbs."CountryDefs" d
      WHERE upper(btrim(d."Code"))=upper(btrim(s."Code")) AND NOT d."IsDeleted"))
   OR (s."RecordType"='CITY' AND NOT EXISTS (
      SELECT 1 FROM mskbs."CityDefs" d
      WHERE upper(btrim(d."Code"))=upper(btrim(s."Code")) AND NOT d."IsDeleted"))
   OR (s."RecordType"='TOWN' AND NOT EXISTS (
      SELECT 1 FROM mskbs."TownDefs" d
      WHERE upper(btrim(d."Code"))=upper(btrim(s."Code")) AND NOT d."IsDeleted"))
 ) THEN
   RAISE EXCEPTION 'Kaynak kayıtlardan hedefe aktarılamayan var; işlem iptal.';
 END IF;
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   JOIN mskbs."CityDefs" d
     ON upper(btrim(d."Code"))=upper(btrim(s."Code")) AND NOT d."IsDeleted"
   LEFT JOIN mskbs."CountryDefs" p
     ON upper(btrim(p."Code"))=upper(btrim(s."ParentCode")) AND NOT p."IsDeleted"
   WHERE s."RecordType"='CITY' AND d."CountryId" IS DISTINCT FROM p."Id"
 ) THEN
   RAISE EXCEPTION 'Mevcut CityDefs ile dosyadaki ülke ilişkisi uyuşmuyor; hiçbir kayıt kalıcı yazılmadı.';
 END IF;
 IF EXISTS (
   SELECT 1 FROM mskbs."_GeographyImport" s
   JOIN mskbs."TownDefs" d
     ON upper(btrim(d."Code"))=upper(btrim(s."Code")) AND NOT d."IsDeleted"
   LEFT JOIN mskbs."CityDefs" p
     ON upper(btrim(p."Code"))=upper(btrim(s."ParentCode")) AND NOT p."IsDeleted"
   WHERE s."RecordType"='TOWN' AND d."CityId" IS DISTINCT FROM p."Id"
 ) THEN
   RAISE EXCEPTION 'Mevcut TownDefs ile dosyadaki şehir ilişkisi uyuşmuyor; hiçbir kayıt kalıcı yazılmadı.';
 END IF;
END
$final_check$;

COMMIT;
