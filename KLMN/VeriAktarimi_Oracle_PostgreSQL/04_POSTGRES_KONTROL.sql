-- 1) Yeni hedef tablolarınız hangi schema altında?
SELECT table_schema,table_name FROM information_schema.tables
WHERE table_name IN ('CountryDefs','ProcedureDefs','SYYDiagnosisDefs')
ORDER BY table_schema,table_name;

-- 2) Oracle metadata CSV'yi içe aldıktan sonra:
CALL legacy_import.auto_match_tables();
SELECT priority,target_table,source_table,source_pk,enabled
FROM legacy_import.table_map ORDER BY priority,target_table;

-- 3) Oracle'da bulunmayan yeni tabloları kontrollü kapatın.
SELECT target_table FROM legacy_import.table_map
WHERE enabled AND source_table IS NULL;
-- ÖRNEK: UPDATE legacy_import.table_map SET enabled=false WHERE target_table='YENI_TABLO';

-- 4) Eski FK alanı kod mu ID mi? Önce bu eşlemeleri elle inceleyin!
SELECT * FROM legacy_import.column_map ORDER BY target_table,target_column;
-- ÖRNEK: INSERT INTO legacy_import.column_map(target_table,target_column,source_column,mode)
-- VALUES ('MilitaryUnitDefs','ForcesCommandDef','FORCESCOMMANDDEF','legacy_id')
-- ON CONFLICT (target_table,target_column)
-- DO UPDATE SET source_column=excluded.source_column,mode=excluded.mode;

-- 5) Eşleme kontrolünden sonra staging tablolarını oluşturun.
-- CALL legacy_import.prepare_staging();

-- 6) CSV'leri staging tablolara aldığınızda satırları karşılaştırın.
SELECT * FROM legacy_import.report_counts();

-- 7) KOD KONTROLÜ (örnek, gerçek şemanız public değilse düzeltin)
-- SELECT "Code",count(*) FROM legacy_raw."COUNTRYDEF"
-- GROUP BY "Code" HAVING count(*)>1;
-- Not: quoted Oracle kolonları genellikle "CODE" / "ID" büyük harflidir.

-- 8) *** Tüm kaynak FK anlamı, veri türleri, tablo sayımları doğrulandıktan sonra ***
-- *** Boş test hedef DB'de, tek çağrı ve hata durumunda otomatik rollback: ***
-- SELECT legacy_import.load_all();

-- 9) Aktarımdan sonra:
-- SELECT * FROM legacy_import.report_counts();
