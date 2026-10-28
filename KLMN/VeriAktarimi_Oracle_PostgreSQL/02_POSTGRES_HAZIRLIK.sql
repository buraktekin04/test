-- SADECE PostgreSQL / test veritabanında. Mevcut EF migration'larına dokunmaz.
CREATE SCHEMA IF NOT EXISTS legacy_import;
CREATE SCHEMA IF NOT EXISTS legacy_raw;

CREATE TABLE IF NOT EXISTS legacy_import.settings (
  key text PRIMARY KEY,
  value text NOT NULL
);
INSERT INTO legacy_import.settings(key,value) VALUES
  ('target_schema','public'),
  ('source_timezone','Europe/Istanbul')
ON CONFLICT (key) DO NOTHING;

-- Oracle'dan CSV olarak alınan METADATA. Kişi/hasta verisi burada tutulmaz.
CREATE TABLE IF NOT EXISTS legacy_import.oracle_columns (
  owner text NOT NULL,
  table_name text NOT NULL,
  column_name text NOT NULL,
  column_id integer NOT NULL,
  PRIMARY KEY (owner,table_name,column_name)
);

-- Kaynak tablo adları gerçek Oracle sözlüğünden auto_match_tables ile bulunur.
CREATE TABLE IF NOT EXISTS legacy_import.table_map (
  target_table text PRIMARY KEY,
  source_table text,
  source_pk text DEFAULT 'ID',
  priority integer NOT NULL DEFAULT 50,
  enabled boolean NOT NULL DEFAULT true
);
INSERT INTO legacy_import.table_map(priority,target_table) VALUES
    (10, 'CountryDefs'),
    (10, 'ForcesCommandDefs'),
    (10, 'ReportTypeDefs'),
    (10, 'RankGroupDefs'),
    (10, 'MilitaryClassDefs'),
    (10, 'BranchDefs'),
    (10, 'HcDecisionDefs'),
    (10, 'HcAdmissionTypeDefs'),
    (10, 'ProcedureTypeDefs'),
    (10, 'SYYDiagnosisTypeDefs'),
    (10, 'SYYDiagnosisSliceDefs'),
    (10, 'ICD10Defs'),
    (10, 'PlaneTypeDefs'),
    (10, 'AnnotationReasonDefs'),
    (10, 'HcConnectedDecisionDefs'),
    (10, 'ReasonForExaminationDefs'),
    (20, 'CityDefs'),
    (20, 'RankDefs'),
    (20, 'MilitaryClassGroupDefs'),
    (20, 'FlyingMissionDefs'),
    (30, 'TownDefs'),
    (30, 'MilitaryOfficeDefs'),
    (30, 'MilitarySchoolDefs'),
    (30, 'MilitaryUnitDefs'),
    (30, 'HospitalDefs'),
    (30, 'ProcedureDefs'),
    (30, 'SYYDiagnosisDefs'),
    (40, 'MilitaryClassGroupRelations'),
    (40, 'HospitalReportTypes'),
    (40, 'EquivalentProcedureDefs'),
    (40, 'ICD10Diagnostics'),
    (40, 'SYYDiagnosisDecisions'),
    (40, 'SYYClassificationDefs'),
    (40, 'SYYDiagnosisBranchDefs'),
    (40, 'SYYDiagnosisProcedureDefs'),
    (40, 'SYYDiagnosisRequireds'),
    (40, 'HcConnectedDecisions'),
    (50, 'SYYDiagnosisRequiredDetails'),
    (50, 'ReasonForExamBranchDefs'),
    (50, 'ReasonForExamProcedureDefs'),
    (50, 'ReasonForExamReportTypes'),
    (50, 'ReasonForExamUnusedHcAdmissionTypes'),
    (50, 'HcUnusedReasonForExams'),
    (50, 'HcUnusedAdmissionTypes'),
    (50, 'HcUnusedDecisions')
ON CONFLICT (target_table) DO NOTHING;

-- mode='direct': aynı değer / doğal cast
-- mode='legacy_id': Oracle FK eski satır kimliği -> deterministik yeni UUID
-- mode='code': Oracle FK eski Code -> yeni referans tablosunda Code ile ara
-- mode='skip': bu hedef sütuna veri taşınmaz (nullable / default sütun)
CREATE TABLE IF NOT EXISTS legacy_import.column_map (
 target_table text NOT NULL,
 target_column text NOT NULL,
 source_column text,
 mode text NOT NULL CHECK (mode IN ('direct','legacy_id','code','skip')),
 PRIMARY KEY(target_table,target_column),
 FOREIGN KEY(target_table) REFERENCES legacy_import.table_map(target_table)
);

-- Eski enum/sayısal Code ilişkisi yeni Guid FK'ye taşınan örnekler.
-- Oracle'da bu kolonların gerçekten Code olup olmadığını kontrol edin!
INSERT INTO legacy_import.column_map(target_table,target_column,source_column,mode) VALUES
 ('ProcedureDefs','ProcedureTypeDefId','ProcedureType','code'),
 ('SYYDiagnosisDefs','SYYDiagnosisTypeDefId','SYYDiagnosisType','code'),
 ('SYYDiagnosisDefs','SYYDiagnosisSliceDefId','Slice','code')
ON CONFLICT (target_table,target_column) DO NOTHING;

-- Kaynak ve hedef schema kontrol:
SELECT table_schema,table_name FROM information_schema.tables
WHERE table_name IN ('CountryDefs','ProcedureDefs','SYYDiagnosisDefs')
ORDER BY table_schema,table_name;
-- Gerekirse: UPDATE legacy_import.settings SET value='mskbs' WHERE key='target_schema';
