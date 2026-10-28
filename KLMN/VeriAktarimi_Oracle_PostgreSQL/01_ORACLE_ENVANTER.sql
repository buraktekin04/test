-- Oracle SQL Editor / DBeaver. :ESKI_SEMA için Oracle schema owner girin.
-- 1) Önce kaynak tabloları gözle inceleyin (NUM_ROWS istatistik olabilir, kesin satır adedi değil).
SELECT owner, table_name, num_rows
FROM all_tables
WHERE owner = UPPER(:ESKI_SEMA)
ORDER BY table_name;

-- 2) Bu sonucu oracle_columns.csv dosyasına UTF-8 + header olarak export edin.
-- Kolon adlarını DBeaver'da PostgreSQL legacy_import.oracle_columns ile eşleştirin.
SELECT c.owner AS "owner",
       c.table_name AS "table_name",
       c.column_name AS "column_name",
       c.column_id AS "column_id"
FROM all_tab_columns c
JOIN all_tables t ON t.owner = c.owner AND t.table_name = c.table_name
WHERE c.owner = UPPER(:ESKI_SEMA)
ORDER BY c.table_name, c.column_id;

-- 3) Kaynaktaki birincil anahtar kolonlarını görüp PostgreSQL table_map.source_pk'yi doğrulayın.
SELECT acc.table_name, acc.column_name, acc.position
FROM all_constraints ac
JOIN all_cons_columns acc ON acc.owner = ac.owner
 AND acc.constraint_name = ac.constraint_name
WHERE ac.owner = UPPER(:ESKI_SEMA)
  AND ac.constraint_type = 'P'
ORDER BY acc.table_name, acc.position;
