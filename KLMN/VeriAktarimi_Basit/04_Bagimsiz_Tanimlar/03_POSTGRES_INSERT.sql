-- PostgreSQL / TEST. Kaynak: mskbs."_SimpleDefinitionsImport".
-- Bu SQL yalnız INSERT uygular; mevcut hedef kayıtlarını asla değiştirmez.
-- DBeaver'da tek dosyayı CSV formatı, UTF-8, TAB delimiter, Header Top,
-- Mapping Existing ile tek staging tablosuna yükleyin.
BEGIN;

DO $transfer$
DECLARE m record; affected bigint; total bigint := 0; t text;
BEGIN
 IF NOT EXISTS (SELECT 1 FROM mskbs."_SimpleDefinitionsImport") THEN
  RAISE EXCEPTION 'Basit tanımlar staging tablosu boş';
 END IF;
 IF EXISTS (
 SELECT 1 FROM mskbs."_SimpleDefinitionsImport" s
 WHERE s."RecordType" IS NULL OR s."RecordType" NOT IN (
 'ANNOTATION_REASON', 'BRANCH', 'FLYING_MISSION', 'CONNECTED_DECISION', 'ICD10', 'PLANE_TYPE', 'REPORT_TYPE', 'RANK_GROUP', 'PROCEDURE_TYPE', 'SYY_TYPE', 'SYY_SLICE', 'FORCES_COMMAND', 'MILITARY_CLASS'
 ) OR nullif(btrim(s."Code"),'') IS NULL
   OR nullif(btrim(s."Name"),'') IS NULL
   OR length(btrim(s."Code"))>100
   OR s."IsActive" IS NULL
   OR lower(btrim(s."IsActive")) NOT IN ('true','false')
 ) THEN
  RAISE EXCEPTION 'Kaynakta tip, Code, Name, IsActive bozuk, script durduruldu';
 END IF;
 IF EXISTS (
 SELECT 1 FROM mskbs."_SimpleDefinitionsImport" s
 GROUP BY s."RecordType", upper(btrim(s."Code")) HAVING count(*)>1
 ) THEN
  RAISE EXCEPTION 'Kaynakta tip+Code tekrar ediyor';
 END IF;
 FOR m IN SELECT * FROM (VALUES
   ('ANNOTATION_REASON','AnnotationReasonDefs','text'),
   ('BRANCH','BranchDefs','text'),
   ('FLYING_MISSION','FlyingMissionDefs','text'),
   ('CONNECTED_DECISION','HcConnectedDecisionDefs','text'),
   ('ICD10','ICD10Defs','text'),
   ('PLANE_TYPE','PlaneTypeDefs','text'),
   ('REPORT_TYPE','ReportTypeDefs','text'),
   ('RANK_GROUP','RankGroupDefs','text'),
   ('PROCEDURE_TYPE','ProcedureTypeDefs','integer'),
   ('SYY_TYPE','SYYDiagnosisTypeDefs','integer'),
   ('SYY_SLICE','SYYDiagnosisSliceDefs','integer'),
   ('FORCES_COMMAND','ForcesCommandDefs','text'),
   ('MILITARY_CLASS','MilitaryClassDefs','text')
 ) AS v(record_type,target_table,code_kind)
 WHERE EXISTS (SELECT 1 FROM mskbs."_SimpleDefinitionsImport" s WHERE s."RecordType"=v.record_type)
 LOOP
  IF to_regclass(format('mskbs.%I',m.target_table)) IS NULL THEN
    RAISE EXCEPTION 'Yeni KLMN tablosu bulunamadı: %',m.target_table;
  END IF;
  IF m.code_kind='integer' AND EXISTS (
    SELECT 1 FROM mskbs."_SimpleDefinitionsImport" s
    WHERE s."RecordType"=m.record_type AND btrim(s."Code") !~ '^[0-9]+$'
  ) THEN
    RAISE EXCEPTION 'Sayısal kod içermesi gereken tabloya metin geldi: %',m.target_table;
  END IF;

  -- Karşılık gelen mevcut Code korunur; silinmiş eski kaydın Id'si
  -- hash ile çakışırsa PostgreSQL PK kısıtı hata vererek rollback yapar.
  EXECUTE format(
    'INSERT INTO mskbs.%I ("Id","Code","Name","CreatedDate","IsActive","IsDeleted") ' ||
    'SELECT md5(%L || upper(btrim(s."Code")))::uuid, ' ||
    ' %s, s."Name", now(), lower(btrim(s."IsActive"))::boolean, false ' ||
    'FROM mskbs."_SimpleDefinitionsImport" s WHERE s."RecordType"=%L ' ||
    'AND NOT EXISTS (SELECT 1 FROM mskbs.%I x ' ||
    'WHERE x."Code"::text= %s AND NOT x."IsDeleted")',
    m.target_table,
    'KLMN_MASTER_'||m.record_type||'_V1|',
    CASE WHEN m.code_kind='integer' THEN 'btrim(s."Code")::integer'
         ELSE 'upper(btrim(s."Code"))' END,
    m.record_type,
    m.target_table,
    CASE WHEN m.code_kind='integer' THEN 'btrim(s."Code")::integer::text'
         ELSE 'upper(btrim(s."Code"))' END
  );
  GET DIAGNOSTICS affected = ROW_COUNT;
  total:=total+affected;
  RAISE NOTICE '%: yeni kayıt=%',m.target_table,affected;
 END LOOP;
 RAISE NOTICE 'Toplam yeni kayıt=%',total;
END
$transfer$;

COMMIT;
