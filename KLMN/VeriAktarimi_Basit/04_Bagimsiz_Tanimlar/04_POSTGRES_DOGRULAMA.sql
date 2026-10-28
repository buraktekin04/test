-- PostgreSQL: aktarılan kaynak tablolara ve yüklenen kodlara bak.
SELECT "RecordType",count(*) AS rows_in_export
FROM mskbs."_SimpleDefinitionsImport" GROUP BY "RecordType"
ORDER BY "RecordType";

-- Kaynakta Türkçe karakterler düzgün mü?
SELECT "RecordType","Code","Name"
FROM mskbs."_SimpleDefinitionsImport"
WHERE "Name" LIKE '%?%'
ORDER BY "RecordType" LIMIT 50;

-- Not: Hedef tablolar farklıdır. Ülke/şehir/ilçe ve ilişkili modüller için
-- ortak katalog ve diğer grup scriptlerini kullanın.
