-- SADECE DENETİM / READ-ONLY: migration oluşturmaz, indeks değiştirmez.
-- Bu sorguyu yerel PostgreSQL veritabanında çalıştırarak eski/koşulsuz
-- unique indeksleri kontrol ediniz. Üretim veritabanına otomatik uygulamayınız.
-- EF Core modelindeki standart Code alanları: IsDeleted = FALSE kısmî unique.
SELECT
 schemaname, tablename, indexname, indexdef,
 CASE
   WHEN indexdef ~* '(\(\s*"?(id)"?\s*\))' THEN 'PK/ID (kontrol dışı)'
   WHEN tablename IN ('ProcedureTypeDefs','SYYDiagnosisTypeDefs','SYYDiagnosisSliceDefs')
     THEN 'FK ALTERNATE KEY - ayrı model/migration tasarımı gerekir'
   WHEN indexdef ILIKE '%WHERE%' AND indexdef ILIKE '%IsDeleted%'
     THEN 'OK - silinmiş satır benzersizliğe katılmıyor'
   ELSE 'İNCELE - soft-delete filtresi olmadan UNIQUE olabilir'
 END AS audit_status
FROM pg_indexes
WHERE schemaname = current_schema()
  AND indexdef ILIKE '%UNIQUE%'
ORDER BY tablename,indexname;
-- Yanlış unique indeksleri migration üzerinden, yedek ve bağımlılık analizi
-- yapmadan DROP/RECREATE etmeyiniz. Üç alternatif anahtarın FK referansları
-- (ProcedureDef.ProcedureType, SYYDiagnosisDef.SYYDiagnosisType / Slice)
-- salt IsDeleted filtresiyle unique hale getirilemez.
