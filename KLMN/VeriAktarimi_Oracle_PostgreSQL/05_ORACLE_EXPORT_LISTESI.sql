-- Oracle SQL Editor: eşleştirilmiş tablolara ait sorguları buradan kopyalayın.
-- :ESKI_SEMA girilir. Bu sorgu kendisi veri export etmez; birer SELECT üretir.
-- PostgreSQL'deki legacy_import.table_map eşlemesini kontrol ettikten sonra,
-- aşağıdan sadece gerçekten gerekli eski tabloların SELECT çıktısını CSV'ye aktarın.
SELECT table_name,
       'SELECT * FROM "' || owner || '"."' || table_name || '";' AS export_sql
FROM all_tables
WHERE owner = UPPER(:ESKI_SEMA)
ORDER BY table_name;

-- Örnek: Üstte üretilen SELECT * FROM "ESKI_SEMA"."COUNTRYDEF";
-- sorgusunu yeni SQL sekmesinde çalıştırıp DBeaver Export Data > CSV seçin.
-- Eski kayıt ID / FK değerlerinin hücrelerde bilimsel gösterime dönüşmediğini
-- ve başta sıfır olan kodların bozulmadığını kontrol edin.
