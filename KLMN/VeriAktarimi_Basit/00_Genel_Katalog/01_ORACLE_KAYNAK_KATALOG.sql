-- Oracle MSKBSTEST: sadece metadata. Gerçek kullanıcı/hasta verisi seçilmez.
-- Bu raporu XLS olarak dışa aktarabilirsiniz. Eski kaynak adları adaydır;
-- MISSING_SOURCE her zaman hata değildir: ilgili tanım KLMN'de yeni olabilir.
WITH expected AS (
SELECT 'CountryDefs' AS target, '03_CountryCityTown_Tek_Dosya' AS group_name, 'COUNTRYDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'CityDefs' AS target, '03_CountryCityTown_Tek_Dosya' AS group_name, 'CITYDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'TownDefs' AS target, '03_CountryCityTown_Tek_Dosya' AS group_name, 'TOWNDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'AnnotationReasonDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'ANNOTATIONREASONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'BranchDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'BRANCHDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'FlyingMissionDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'FLYINGMISSIONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcConnectedDecisionDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'HCCONNECTEDDECISIONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ICD10Defs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'ICD10DEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'PlaneTypeDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'PLANETYPEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReportTypeDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'REPORTTYPEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'RankGroupDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'RANKGROUPDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ProcedureTypeDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'PROCEDURETYPEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisTypeDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'SYYDIAGNOSISTYPEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisSliceDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'SYYDIAGNOSISSLICEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ForcesCommandDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'FORCESCOMMANDDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitaryClassDefs' AS target, '04_Bagimsiz_Tanimlar' AS group_name, 'MILITARYCLASSDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcAdmissionTypeDefs' AS target, 'Ozel_Alanlar' AS group_name, 'HCADMISSIONTYPEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcDecisionDefs' AS target, 'Ozel_Alanlar' AS group_name, 'HCDECISIONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReasonForExaminationDefs' AS target, 'Ozel_Alanlar' AS group_name, 'REASONFOREXAMINATIONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'RankDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'RANKDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitaryClassGroupDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'MILITARYCLASSGROUPDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitaryUnitDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'MILITARYUNITDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitaryOfficeDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'MILITARYOFFICEDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitarySchoolDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'MILITARYSCHOOLDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HospitalDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'HOSPITALDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ProcedureDefs' AS target, 'Bagli_Tanimlar' AS group_name, 'PROCEDUREDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisDefs' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'MilitaryClassGroupRelations' AS target, 'Esleme' AS group_name, 'MILITARYCLASSGROUPRELATION' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HospitalReportTypes' AS target, 'Esleme' AS group_name, 'HOSPITALREPORTTYPE' AS oracle_candidate FROM dual
UNION ALL
SELECT 'EquivalentProcedureDefs' AS target, 'Esleme' AS group_name, 'EQUIVALENTPROCEDUREDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ICD10Diagnostics' AS target, 'Esleme' AS group_name, 'ICD10DIAGNOSTIC' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisDecisions' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISDECISION' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYClassificationDefs' AS target, 'SYY_Baglilar' AS group_name, 'SYYCLASSIFICATIONDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisBranchDefs' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISBRANCHDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisProcedureDefs' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISPROCEDUREDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisRequireds' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISREQUIRED' AS oracle_candidate FROM dual
UNION ALL
SELECT 'SYYDiagnosisRequiredDetails' AS target, 'SYY_Baglilar' AS group_name, 'SYYDIAGNOSISREQUIREDDETAIL' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcConnectedDecisions' AS target, 'Karar_Muayene_Esleme' AS group_name, 'HCCONNECTEDDECISION' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcUnusedAdmissionTypes' AS target, 'Karar_Muayene_Esleme' AS group_name, 'HCUNUSEDADMISSIONTYPE' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcUnusedDecisions' AS target, 'Karar_Muayene_Esleme' AS group_name, 'HCUNUSEDDECISION' AS oracle_candidate FROM dual
UNION ALL
SELECT 'HcUnusedReasonForExams' AS target, 'Karar_Muayene_Esleme' AS group_name, 'HCUNUSEDREASONFOREXAM' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReasonForExamBranchDefs' AS target, 'Karar_Muayene_Esleme' AS group_name, 'REASONFOREXAMBRANCHDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReasonForExamProcedureDefs' AS target, 'Karar_Muayene_Esleme' AS group_name, 'REASONFOREXAMPROCEDUREDEF' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReasonForExamReportTypes' AS target, 'Karar_Muayene_Esleme' AS group_name, 'REASONFOREXAMREPORTTYPE' AS oracle_candidate FROM dual
UNION ALL
SELECT 'ReasonForExamUnusedHcAdmissionTypes' AS target, 'Karar_Muayene_Esleme' AS group_name, 'REASONFOREXAMUNUSEDHCADMISSIONTYPE' AS oracle_candidate FROM dual
)
SELECT e.group_name, e.target, e.oracle_candidate AS presumed_table,
       CASE WHEN t.table_name IS NULL THEN 'MISSING_SOURCE' ELSE 'FOUND' END AS source_status,
       c.column_name,c.data_type,c.nullable,c.column_id
FROM expected e
LEFT JOIN all_tables t ON t.owner='MSKBSTEST' AND t.table_name=e.oracle_candidate
LEFT JOIN all_tab_columns c ON c.owner=t.owner AND c.table_name=t.table_name
ORDER BY e.group_name,e.target,c.column_id;

-- FK tanımları: eski ID/kod ilişkilerinin gerçek adlarını tespit edin.
SELECT fk.table_name AS child_table,child_col.column_name AS child_column,
       pk.table_name AS parent_table,parent_col.column_name AS parent_column
FROM all_constraints fk
JOIN all_cons_columns child_col
 ON child_col.owner=fk.owner AND child_col.constraint_name=fk.constraint_name
JOIN all_constraints pk
 ON pk.owner=fk.r_owner AND pk.constraint_name=fk.r_constraint_name
JOIN all_cons_columns parent_col
 ON parent_col.owner=pk.owner AND parent_col.constraint_name=pk.constraint_name
 AND parent_col.position=child_col.position
WHERE fk.owner='MSKBSTEST' AND fk.constraint_type='R'
ORDER BY fk.table_name,child_col.position;

-- Eski Oracle tanımlarının metadata'sını ayrıca bir CSV/XLS olarak paylaşabilir,
-- kolon adları ve FK ilişkilerine göre kalan grupların birebir SQL'ini kesinleştirebilirsiniz.
