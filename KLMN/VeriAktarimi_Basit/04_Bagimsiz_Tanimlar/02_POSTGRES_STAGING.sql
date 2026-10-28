-- PostgreSQL TEST: tek CSV için tek yardımcı tablo.
CREATE TABLE IF NOT EXISTS mskbs."_SimpleDefinitionsImport" (
 "RecordType" text,
 "Code" text,
 "Name" text,
 "IsActive" text
);
SELECT count(*) AS staged_count FROM mskbs."_SimpleDefinitionsImport";
