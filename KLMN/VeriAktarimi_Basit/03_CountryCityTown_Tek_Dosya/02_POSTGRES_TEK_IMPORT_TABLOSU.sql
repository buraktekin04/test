-- PostgreSQL TEST veritabanı / mskbs; DBeaver CSV import için
-- tek yardımcı tablo. Kalıcı legacy schema YOK.
-- Bu tabloya veri import edilmeden önce mevcut veri varsa kontrol edin;
-- üzerine eklemek farklı aktarım partilerini karıştırabilir.
CREATE TABLE IF NOT EXISTS mskbs."_GeographyImport" (
  "RecordType" text,
  "Code" text,
  "Name" text,
  "ExternalCode" text,
  "ParentCode" text,
  "IsActive" text
);

SELECT COUNT(*) AS staged_rows FROM mskbs."_GeographyImport";

-- DBeaver > mskbs._GeographyImport > Import Data > CSV:
-- delimiter TAB, Encoding UTF-8, Header Top, Mapping Existing.
-- Bu tabloda tam 6 kolon bulunmalıdır.
