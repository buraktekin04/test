-- SADECE DENETİM / READ-ONLY: migration oluşturmaz, indeks değiştirmez.
-- Bu sorguyu yerel PostgreSQL veritabanında çalıştırarak eski/koşulsuz
-- unique indeksleri kontrol ediniz. Üretim veritabanına otomatik uygulamayınız.
-- EF Core modelindeki standart Code alanları: IsDeleted = FALSE kısmî unique.
SELECT
 schemaname, tablename, indexname, indexdef,
 CASE
   WHEN indexdef ~* '(\(\s*"?(id)"?\s*\))' THEN 'PK/ID (kontrol dışı)'
   WHEN tablename IN ('ProcedureTypeDefs','SYYDiagnosisTypeDefs','SYYDiagnosisSliceDefs')
     THEN 'GUID FK GEÇİŞİ SONRASINDA PARTIAL UNIQUE OLMALI - 20261028 scriptini kontrol et'
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

-- Not: Sadece Code bulunan üç tarihsel AK'nin yeni GUID FK ile değiştirilmesi
-- yeterli değildir; aynı tablo için eski koşulsuz index de kalmamalıdır.
-- Aşağıda gerçekten kalan eski Code alternate-key constraint'lerini görün:
SELECT conrelid::regclass AS table_name, conname AS constraint_name,
       pg_get_constraintdef(oid) AS constraint_definition
FROM pg_constraint
WHERE contype='u' AND conname IN (
 'AK_ProcedureTypeDefs_Code','AK_SYYDiagnosisTypeDefs_Code','AK_SYYDiagnosisSliceDefs_Code');
