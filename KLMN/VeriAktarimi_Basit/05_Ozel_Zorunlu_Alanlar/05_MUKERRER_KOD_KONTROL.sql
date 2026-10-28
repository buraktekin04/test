-- PostgreSQL / KLMN - mükerrer Code tanısı (gerçek tablolara dokunmaz)
-- Transfer sırasında hata alındıysa yeni SQL editöründe inceleyin.
-- DBeaver transaction hata durumunda kalmışsa önce ROLLBACK;
ROLLBACK;

-- 1. Tekrar eden kayıtlar, kaç TAMAMEN FARKLI içerik barındırıyor?
SELECT
  "RecordType",
  upper(btrim("Code")) AS "Code",
  count(*) AS toplam_satir,
  count(DISTINCT to_jsonb(s)) AS farkli_icerik_sayisi,
  CASE WHEN count(DISTINCT to_jsonb(s))=1
       THEN 'AYNI SATIR TEKRARLANMIS - yeni 03 scripti otomatik tekillestirir'
       ELSE 'AYNI KOD, FARKLI ICERIK - kaynagi kontrol et' END AS durum
FROM mskbs."_SpecialDefinitionsImport" s
GROUP BY "RecordType",upper(btrim("Code"))
HAVING count(*)>1
ORDER BY "RecordType","Code";

-- 2. Sorunlu kodlar için alan değerlerini satır bazında karşılaştırın.
SELECT s.*
FROM mskbs."_SpecialDefinitionsImport" s
JOIN (
 SELECT "RecordType",upper(btrim("Code")) AS normalized_code
 FROM mskbs."_SpecialDefinitionsImport"
 GROUP BY "RecordType",upper(btrim("Code"))
 HAVING count(DISTINCT to_jsonb("_SpecialDefinitionsImport"))>1
) d ON s."RecordType"=d."RecordType"
   AND upper(btrim(s."Code"))=d.normalized_code
ORDER BY s."RecordType",s."Code";

-- 3. CSV yanlışlıkla iki kez import edildiyse:
-- Veriler birebir aynıysa staging'i DELETE/TRUNCATE yapmaya gerek yok.
-- Güncellenmiş 03_POSTGRES_INSERT.sql, sadece tam satır tekrarlarını
-- SELECT DISTINCT ile bir kez değerlendirir.
-- Aynı kod altındaki içerikler farklıysa otomatik seçim YAPILMAZ.
