-- PostgreSQL test ortamı: başarı sonrası kontrol, hepsi salt okunur.
-- CSV'deki ülke/şehir/ilçe adetleri:
SELECT "RecordType",count(*) AS exported_count
FROM mskbs."_GeographyImport"
GROUP BY "RecordType" ORDER BY "RecordType";

-- Gerçek tablolardaki aktif ve silinmemiş veri:
SELECT 'COUNTRY' AS record_type,count(*) AS rows_in_database
 FROM mskbs."CountryDefs" WHERE NOT "IsDeleted"
UNION ALL SELECT 'CITY',count(*) FROM mskbs."CityDefs" WHERE NOT "IsDeleted"
UNION ALL SELECT 'TOWN',count(*) FROM mskbs."TownDefs" WHERE NOT "IsDeleted";

-- Şehir-ülke örnekleri:
SELECT c."Code" AS city_code,c."Name" AS city_name,
       p."Code" AS country_code,p."Name" AS country_name
FROM mskbs."CityDefs" c
LEFT JOIN mskbs."CountryDefs" p ON p."Id"=c."CountryId"
ORDER BY c."Code" LIMIT 30;

-- İlçe-şehir-ülke örnekleri:
SELECT t."Code" AS town_code,t."Name" AS town_name,
       c."Code" AS city_code,c."Name" AS city_name,
       p."Code" AS country_code,p."Name" AS country_name
FROM mskbs."TownDefs" t
LEFT JOIN mskbs."CityDefs" c ON c."Id"=t."CityId"
LEFT JOIN mskbs."CountryDefs" p ON p."Id"=c."CountryId"
ORDER BY t."Code" LIMIT 50;

-- Sayımlar ve ilişkiler doğru doğrulandıktan, yedek alındıktan sonra
-- sadece yardımcı tabloyu isteğe bağlı olarak kaldırabilirsiniz:
-- DROP TABLE mskbs."_GeographyImport";
