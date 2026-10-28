-- ORACLE / MSKBSTEST, yalnız Code-Name-IsActive taşıyan bağımsız tanımlar.
-- ALTTAKİ tablolardan Oracle'da bulunmayanları KONTROL dosyasıyla görün.
-- Oracle'da hiç bulunmayan yeni tanımların SELECT bloklarını ve
-- takip eden UNION ALL ifadelerini sorgudan çıkarmadan ÇALIŞTIRMAYIN.
-- Tek sonuç: RecordType, Code, Name, IsActive. İlişki/UUID içermez.
-- Source EXTERNALCODE/SHORTNAME gibi ek kolonlar ayrıca doğrulanmalıdır.
SELECT 'ANNOTATION_REASON' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.ANNOTATIONREASONDEF c
UNION ALL
SELECT 'BRANCH' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.BRANCHDEF c
UNION ALL
SELECT 'FLYING_MISSION' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.FLYINGMISSIONDEF c
UNION ALL
SELECT 'CONNECTED_DECISION' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.HCCONNECTEDDECISIONDEF c
UNION ALL
SELECT 'ICD10' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.ICD10DEF c
UNION ALL
SELECT 'PLANE_TYPE' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.PLANETYPEDEF c
UNION ALL
SELECT 'REPORT_TYPE' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.REPORTTYPEDEF c
UNION ALL
SELECT 'RANK_GROUP' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.RANKGROUPDEF c
UNION ALL
SELECT 'PROCEDURE_TYPE' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.PROCEDURETYPEDEF c
UNION ALL
SELECT 'SYY_TYPE' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.SYYDIAGNOSISTYPEDEF c
UNION ALL
SELECT 'SYY_SLICE' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.SYYDIAGNOSISSLICEDEF c
UNION ALL
SELECT 'FORCES_COMMAND' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.FORCESCOMMANDDEF c
UNION ALL
SELECT 'MILITARY_CLASS' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_ISACTIVE'
       END AS "IsActive"
FROM MSKBSTEST.MILITARYCLASSDEF c
ORDER BY "RecordType","Code";
