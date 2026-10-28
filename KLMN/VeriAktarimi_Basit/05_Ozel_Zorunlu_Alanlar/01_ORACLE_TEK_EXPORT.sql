-- Oracle: özel/required bool alanları kaybetmemek için 3 tanım tek dosyada.
-- Önce 00_ORACLE_KOLON_KONTROL.sql ile ÜÇ tablonun eski kolonlarını doğrula.
-- Kaynakta bulunmayan bir tanım varsa ona ait SELECT bloğunu UNION ALL ile
-- birlikte çıkar. Zorunlu bool kolonlarının hiçbiri tahmin edilemez.
-- Dışa aktarma: XLS -> Unicode Metin (*.txt) -> VS Code UTF-8 (TAB).
SELECT 'ADMISSION' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_FLAG' END AS "IsActive",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.FORCESCOMMANDREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.FORCESCOMMANDREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "ForcesCommandRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.RANKREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.RANKREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "RankRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYCLASSREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYCLASSREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "MilitaryClassRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYUNITREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYUNITREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "MilitaryUnitRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.SENDERCHAIRREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.SENDERCHAIRREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "SenderChairRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYOFFICEREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYOFFICEREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "MilitaryOfficeRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.REGISTRYNUMBERREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.REGISTRYNUMBERREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "RegistryNumberRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYSCHOOLREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.MILITARYSCHOOLREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "MilitarySchoolRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.LASTNOMINATIONDATEREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.LASTNOMINATIONDATEREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "LastNominationDateRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.HEIGHTWEIGHTREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.HEIGHTWEIGHTREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "HeightWeightRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.NTHADMISSIONREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.NTHADMISSIONREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "NthAdmissionRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHDOCUMENTDATEREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHDOCUMENTDATEREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "DispatchDocumentDateRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHDOCUMENTNOREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHDOCUMENTNOREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "DispatchDocumentNoRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.PERTEMAPPROVALREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.PERTEMAPPROVALREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "PertemApprovalRequired",
       CAST(NULL AS VARCHAR2(20)) AS "TimeInformationRequired",
       CAST(NULL AS VARCHAR2(20)) AS "AdditionalDescriptionRequired",
       CAST(NULL AS VARCHAR2(20)) AS "NegativeDecision",
       CAST(NULL AS VARCHAR2(20)) AS "ApprovalRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DiagnosisRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchRequired",
       CAST(NULL AS VARCHAR2(20)) AS "ClassificationExamination",
       CAST(NULL AS VARCHAR2(20)) AS "Objectionable"
FROM MSKBSTEST.HCADMISSIONTYPEDEF c
UNION ALL
SELECT 'DECISION' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.NAME) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_FLAG' END AS "IsActive",
       CAST(NULL AS VARCHAR2(20)) AS "ForcesCommandRequired",
       CAST(NULL AS VARCHAR2(20)) AS "RankRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryClassRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryUnitRequired",
       CAST(NULL AS VARCHAR2(20)) AS "SenderChairRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryOfficeRequired",
       CAST(NULL AS VARCHAR2(20)) AS "RegistryNumberRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitarySchoolRequired",
       CAST(NULL AS VARCHAR2(20)) AS "LastNominationDateRequired",
       CAST(NULL AS VARCHAR2(20)) AS "HeightWeightRequired",
       CAST(NULL AS VARCHAR2(20)) AS "NthAdmissionRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchDocumentDateRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchDocumentNoRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.PERTEMAPPROVALREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.PERTEMAPPROVALREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "PertemApprovalRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.TIMEINFORMATIONREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.TIMEINFORMATIONREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "TimeInformationRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.ADDITIONALDESCRIPTIONREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.ADDITIONALDESCRIPTIONREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "AdditionalDescriptionRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.NEGATIVEDECISION))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.NEGATIVEDECISION))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "NegativeDecision",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.APPROVALREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.APPROVALREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "ApprovalRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.DIAGNOSISREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.DIAGNOSISREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "DiagnosisRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchRequired",
       CAST(NULL AS VARCHAR2(20)) AS "ClassificationExamination",
       CAST(NULL AS VARCHAR2(20)) AS "Objectionable"
FROM MSKBSTEST.HCDECISIONDEF c
UNION ALL
SELECT 'EXAM_REASON' AS "RecordType",
       UPPER(TRIM(TO_CHAR(c.CODE))) AS "Code",
       TRIM(c.DESCRIPTION) AS "Name",
       CASE
         WHEN c.ISACTIVE IS NULL THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
         WHEN UPPER(TRIM(TO_CHAR(c.ISACTIVE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
         ELSE 'INVALID_FLAG' END AS "IsActive",
       CAST(NULL AS VARCHAR2(20)) AS "ForcesCommandRequired",
       CAST(NULL AS VARCHAR2(20)) AS "RankRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryClassRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryUnitRequired",
       CAST(NULL AS VARCHAR2(20)) AS "SenderChairRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitaryOfficeRequired",
       CAST(NULL AS VARCHAR2(20)) AS "RegistryNumberRequired",
       CAST(NULL AS VARCHAR2(20)) AS "MilitarySchoolRequired",
       CAST(NULL AS VARCHAR2(20)) AS "LastNominationDateRequired",
       CAST(NULL AS VARCHAR2(20)) AS "HeightWeightRequired",
       CAST(NULL AS VARCHAR2(20)) AS "NthAdmissionRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchDocumentDateRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DispatchDocumentNoRequired",
       CAST(NULL AS VARCHAR2(20)) AS "PertemApprovalRequired",
       CAST(NULL AS VARCHAR2(20)) AS "TimeInformationRequired",
       CAST(NULL AS VARCHAR2(20)) AS "AdditionalDescriptionRequired",
       CAST(NULL AS VARCHAR2(20)) AS "NegativeDecision",
       CAST(NULL AS VARCHAR2(20)) AS "ApprovalRequired",
       CAST(NULL AS VARCHAR2(20)) AS "DiagnosisRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHREQUIRED))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.DISPATCHREQUIRED))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "DispatchRequired",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.CLASSIFICATIONEXAMINATION))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.CLASSIFICATIONEXAMINATION))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "ClassificationExamination",
       CASE
 WHEN UPPER(TRIM(TO_CHAR(c.OBJECTIONABLE))) IN ('1','TRUE','T','Y','YES','E','EVET') THEN 'true'
 WHEN UPPER(TRIM(TO_CHAR(c.OBJECTIONABLE))) IN ('0','FALSE','F','N','NO','H','HAYIR') THEN 'false'
 ELSE 'INVALID_FLAG'
 END AS "Objectionable"
FROM MSKBSTEST.REASONFOREXAMINATIONDEF c
ORDER BY "RecordType","Code";
