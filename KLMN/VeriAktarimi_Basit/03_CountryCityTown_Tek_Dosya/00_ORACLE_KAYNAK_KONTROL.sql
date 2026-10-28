-- ORACLE / MSKBSTEST - salt okunur. Önce tek sefer kontrol.
-- Bu sürümde varsayılan eski ilişkiler:
--   CITYDEF.COUNTRY -> COUNTRYDEF.OBJECTID
--   TOWNDEF.CITYDEF -> CITYDEF.OBJECTID
-- Kaynak bağlantınızda farklıysa 01_... SQL'i çalıştırmadan uyarlayın.

SELECT table_name,column_name,data_type,nullable
FROM all_tab_columns
WHERE owner='MSKBSTEST'
  AND table_name IN ('COUNTRYDEF','CITYDEF','TOWNDEF')
ORDER BY table_name,column_id;

-- Kaynak Oracle FK kısıtları varsa gerçek bağlantılarını listeler.
SELECT fk.table_name AS child_table,
       child_col.column_name AS child_column,
       pk.table_name AS parent_table,
       parent_col.column_name AS parent_column
FROM all_constraints fk
JOIN all_cons_columns child_col
  ON child_col.owner=fk.owner AND child_col.constraint_name=fk.constraint_name
JOIN all_constraints pk
  ON pk.owner=fk.r_owner AND pk.constraint_name=fk.r_constraint_name
JOIN all_cons_columns parent_col
  ON parent_col.owner=pk.owner AND parent_col.constraint_name=pk.constraint_name
 AND parent_col.position=child_col.position
WHERE fk.owner='MSKBSTEST'
  AND fk.table_name IN ('CITYDEF','TOWNDEF') AND fk.constraint_type='R'
ORDER BY fk.table_name,child_col.column_name;

-- Kaynak veri miktarı: bu sayıların toplamı export toplamına eşit olmalı.
SELECT 'COUNTRY' AS entity,COUNT(*) AS row_count FROM MSKBSTEST.COUNTRYDEF
UNION ALL
SELECT 'CITY',COUNT(*) FROM MSKBSTEST.CITYDEF
UNION ALL
SELECT 'TOWN',COUNT(*) FROM MSKBSTEST.TOWNDEF;

-- Eski FK varsayımlarına göre eşleşmeyen kayıtlar (sıfır beklenir).
-- Sadece OBJECTID ve COUNTRY/CITYDEF kolonları doğrulanmışsa çalıştırın.
SELECT 'CITY -> COUNTRY' AS relation,COUNT(*) AS missing_parent
FROM MSKBSTEST.CITYDEF c
LEFT JOIN MSKBSTEST.COUNTRYDEF p ON p.OBJECTID=c.COUNTRY
WHERE c.COUNTRY IS NOT NULL AND p.OBJECTID IS NULL
UNION ALL
SELECT 'TOWN -> CITY',COUNT(*)
FROM MSKBSTEST.TOWNDEF t
LEFT JOIN MSKBSTEST.CITYDEF c ON c.OBJECTID=t.CITYDEF
WHERE t.CITYDEF IS NOT NULL AND c.OBJECTID IS NULL;
