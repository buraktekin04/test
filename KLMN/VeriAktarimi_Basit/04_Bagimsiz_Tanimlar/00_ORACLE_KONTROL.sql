-- Oracle / MSKBSTEST - önce tablolar gerçekten var mı ve kolonları neler?
WITH expected AS (
 SELECT 'ANNOTATIONREASONDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'BRANCHDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'FLYINGMISSIONDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'HCCONNECTEDDECISIONDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'ICD10DEF' AS table_name FROM dual
 UNION ALL
 SELECT 'PLANETYPEDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'REPORTTYPEDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'RANKGROUPDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'PROCEDURETYPEDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'SYYDIAGNOSISTYPEDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'SYYDIAGNOSISSLICEDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'FORCESCOMMANDDEF' AS table_name FROM dual
 UNION ALL
 SELECT 'MILITARYCLASSDEF' AS table_name FROM dual
)
SELECT x.table_name,
 CASE WHEN t.table_name IS NULL THEN 'MISSING_SOURCE' ELSE 'FOUND' END AS status,
 LISTAGG(c.column_name, ', ') WITHIN GROUP (ORDER BY c.column_id) AS columns_found
FROM expected x
LEFT JOIN all_tables t ON t.owner='MSKBSTEST' AND t.table_name=x.table_name
LEFT JOIN all_tab_columns c ON c.owner=t.owner AND c.table_name=t.table_name
GROUP BY x.table_name,t.table_name
ORDER BY x.table_name;

-- Aktarılan alanlar en az CODE, NAME, ISACTIVE içermeli.
-- Kaynağı bulunmayan yeni tanım tabloları için veri import etmeyin; KLMN'de
-- tanımları uygulamanın kendi arayüzünden oluşturmak gerekebilir.
