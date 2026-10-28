-- PostgreSQL TEST, tek işlem / zorunlu alanlar doğrulanır.
-- Önceden bulunan kayıtlar ve ID'leri korunur; gerçek veri temizlenmez.
BEGIN;
DO $check$
BEGIN
 IF NOT EXISTS (SELECT 1 FROM mskbs."_SpecialDefinitionsImport") THEN
  RAISE EXCEPTION '_SpecialDefinitionsImport boş';
 END IF;
 IF EXISTS (SELECT 1 FROM mskbs."_SpecialDefinitionsImport" s
 WHERE s."RecordType" IS NULL OR s."RecordType" NOT IN ('ADMISSION','DECISION','EXAM_REASON')
    OR nullif(btrim(s."Code"),'') IS NULL
    OR nullif(btrim(s."Name"),'') IS NULL
    OR s."IsActive" IS NULL OR lower(btrim(s."IsActive")) NOT IN ('true','false')
 ) THEN
   RAISE EXCEPTION 'Kaynakta tip/kod/ad/IsActive geçersiz';
 END IF;
 -- Sadece TÜM alanları birebir aynı olan tekrarlar güvenle tekilleştirilir.
 -- Aynı RecordType+Code altında Name, IsActive veya herhangi bir zorunlu
 -- bool alanı farklıysa veri kaybını önlemek için aktarım durdurulur.
 IF EXISTS (
   SELECT 1
   FROM mskbs."_SpecialDefinitionsImport" s
   GROUP BY s."RecordType",upper(btrim(s."Code"))
   HAVING count(DISTINCT to_jsonb(s))>1
 ) THEN
   RAISE EXCEPTION 'Aynı tür+Code için FARKLI içerikli kayıtlar var. 05_MUKERRER_KOD_KONTROL.sql çalıştırın';
 END IF;
 -- Import edilen CSV aynı kayıtları iki kez içeriyorsa INSERT kaynakları
 -- SELECT DISTINCT ile aynı satırı bir kez değerlendirir; staging değişmez.
  IF EXISTS (
 SELECT 1 FROM mskbs."_SpecialDefinitionsImport"
 WHERE "RecordType"='ADMISSION' AND (
 "ForcesCommandRequired" IS NULL OR lower(btrim("ForcesCommandRequired")) NOT IN ('true','false') OR
 "RankRequired" IS NULL OR lower(btrim("RankRequired")) NOT IN ('true','false') OR
 "MilitaryClassRequired" IS NULL OR lower(btrim("MilitaryClassRequired")) NOT IN ('true','false') OR
 "MilitaryUnitRequired" IS NULL OR lower(btrim("MilitaryUnitRequired")) NOT IN ('true','false') OR
 "SenderChairRequired" IS NULL OR lower(btrim("SenderChairRequired")) NOT IN ('true','false') OR
 "MilitaryOfficeRequired" IS NULL OR lower(btrim("MilitaryOfficeRequired")) NOT IN ('true','false') OR
 "RegistryNumberRequired" IS NULL OR lower(btrim("RegistryNumberRequired")) NOT IN ('true','false') OR
 "MilitarySchoolRequired" IS NULL OR lower(btrim("MilitarySchoolRequired")) NOT IN ('true','false') OR
 "LastNominationDateRequired" IS NULL OR lower(btrim("LastNominationDateRequired")) NOT IN ('true','false') OR
 "HeightWeightRequired" IS NULL OR lower(btrim("HeightWeightRequired")) NOT IN ('true','false') OR
 "NthAdmissionRequired" IS NULL OR lower(btrim("NthAdmissionRequired")) NOT IN ('true','false') OR
 "DispatchDocumentDateRequired" IS NULL OR lower(btrim("DispatchDocumentDateRequired")) NOT IN ('true','false') OR
 "DispatchDocumentNoRequired" IS NULL OR lower(btrim("DispatchDocumentNoRequired")) NOT IN ('true','false') OR
 "PertemApprovalRequired" IS NULL OR lower(btrim("PertemApprovalRequired")) NOT IN ('true','false')
 )) THEN
   RAISE EXCEPTION 'ADMISSION zorunlu boolean değerleri geçersiz veya NULL';
 END IF;
 IF EXISTS (
 SELECT 1 FROM mskbs."_SpecialDefinitionsImport"
 WHERE "RecordType"='DECISION' AND (
 "TimeInformationRequired" IS NULL OR lower(btrim("TimeInformationRequired")) NOT IN ('true','false') OR
 "AdditionalDescriptionRequired" IS NULL OR lower(btrim("AdditionalDescriptionRequired")) NOT IN ('true','false') OR
 "NegativeDecision" IS NULL OR lower(btrim("NegativeDecision")) NOT IN ('true','false') OR
 "ApprovalRequired" IS NULL OR lower(btrim("ApprovalRequired")) NOT IN ('true','false') OR
 "PertemApprovalRequired" IS NULL OR lower(btrim("PertemApprovalRequired")) NOT IN ('true','false') OR
 "DiagnosisRequired" IS NULL OR lower(btrim("DiagnosisRequired")) NOT IN ('true','false')
 )) THEN
   RAISE EXCEPTION 'DECISION zorunlu boolean değerleri geçersiz veya NULL';
 END IF;
 IF EXISTS (
 SELECT 1 FROM mskbs."_SpecialDefinitionsImport"
 WHERE "RecordType"='EXAM_REASON' AND (
 "DispatchRequired" IS NULL OR lower(btrim("DispatchRequired")) NOT IN ('true','false') OR
 "ClassificationExamination" IS NULL OR lower(btrim("ClassificationExamination")) NOT IN ('true','false') OR
 "Objectionable" IS NULL OR lower(btrim("Objectionable")) NOT IN ('true','false')
 )) THEN
   RAISE EXCEPTION 'EXAM_REASON zorunlu boolean değerleri geçersiz veya NULL';
 END IF;
 IF EXISTS (SELECT 1 FROM mskbs."_SpecialDefinitionsImport"
 WHERE "RecordType" IN ('ADMISSION','DECISION') AND btrim("Code") !~ '^[0-9]+$'
 ) THEN RAISE EXCEPTION 'Sayısal Code integer değil'; END IF;
END
$check$;

-- ADMISSION
INSERT INTO mskbs."HcAdmissionTypeDefs" ("Id", "Code", "Name", "CreatedDate", "IsActive", "IsDeleted", "ForcesCommandRequired", "RankRequired", "MilitaryClassRequired", "MilitaryUnitRequired", "SenderChairRequired", "MilitaryOfficeRequired", "RegistryNumberRequired", "MilitarySchoolRequired", "LastNominationDateRequired", "HeightWeightRequired", "NthAdmissionRequired", "DispatchDocumentDateRequired", "DispatchDocumentNoRequired", "PertemApprovalRequired")
SELECT md5('KLMN_SPECIAL_ADMISSION_V1|' || upper(btrim(s."Code")))::uuid,
       btrim(s."Code")::integer,
       s."Name",
       now(),
       lower(btrim(s."IsActive"))::boolean,
       false,
       lower(btrim(s."ForcesCommandRequired"))::boolean,
       lower(btrim(s."RankRequired"))::boolean,
       lower(btrim(s."MilitaryClassRequired"))::boolean,
       lower(btrim(s."MilitaryUnitRequired"))::boolean,
       lower(btrim(s."SenderChairRequired"))::boolean,
       lower(btrim(s."MilitaryOfficeRequired"))::boolean,
       lower(btrim(s."RegistryNumberRequired"))::boolean,
       lower(btrim(s."MilitarySchoolRequired"))::boolean,
       lower(btrim(s."LastNominationDateRequired"))::boolean,
       lower(btrim(s."HeightWeightRequired"))::boolean,
       lower(btrim(s."NthAdmissionRequired"))::boolean,
       lower(btrim(s."DispatchDocumentDateRequired"))::boolean,
       lower(btrim(s."DispatchDocumentNoRequired"))::boolean,
       lower(btrim(s."PertemApprovalRequired"))::boolean
FROM (SELECT DISTINCT * FROM mskbs."_SpecialDefinitionsImport") s
WHERE s."RecordType"='ADMISSION'
 AND NOT EXISTS (
  SELECT 1 FROM mskbs."HcAdmissionTypeDefs" x
  WHERE x."Code"::text=btrim(s."Code")::integer::text
    AND NOT x."IsDeleted"
 );

-- DECISION
INSERT INTO mskbs."HcDecisionDefs" ("Id", "Code", "Name", "CreatedDate", "IsActive", "IsDeleted", "TimeInformationRequired", "AdditionalDescriptionRequired", "NegativeDecision", "ApprovalRequired", "PertemApprovalRequired", "DiagnosisRequired")
SELECT md5('KLMN_SPECIAL_DECISION_V1|' || upper(btrim(s."Code")))::uuid,
       btrim(s."Code")::integer,
       s."Name",
       now(),
       lower(btrim(s."IsActive"))::boolean,
       false,
       lower(btrim(s."TimeInformationRequired"))::boolean,
       lower(btrim(s."AdditionalDescriptionRequired"))::boolean,
       lower(btrim(s."NegativeDecision"))::boolean,
       lower(btrim(s."ApprovalRequired"))::boolean,
       lower(btrim(s."PertemApprovalRequired"))::boolean,
       lower(btrim(s."DiagnosisRequired"))::boolean
FROM (SELECT DISTINCT * FROM mskbs."_SpecialDefinitionsImport") s
WHERE s."RecordType"='DECISION'
 AND NOT EXISTS (
  SELECT 1 FROM mskbs."HcDecisionDefs" x
  WHERE x."Code"::text=btrim(s."Code")::integer::text
    AND NOT x."IsDeleted"
 );

-- EXAM_REASON
INSERT INTO mskbs."ReasonForExaminationDefs" ("Id", "Code", "Description", "CreatedDate", "IsActive", "IsDeleted", "DispatchRequired", "ClassificationExamination", "Objectionable")
SELECT md5('KLMN_SPECIAL_EXAM_REASON_V1|' || upper(btrim(s."Code")))::uuid,
       upper(btrim(s."Code")),
       s."Name",
       now(),
       lower(btrim(s."IsActive"))::boolean,
       false,
       lower(btrim(s."DispatchRequired"))::boolean,
       lower(btrim(s."ClassificationExamination"))::boolean,
       lower(btrim(s."Objectionable"))::boolean
FROM (SELECT DISTINCT * FROM mskbs."_SpecialDefinitionsImport") s
WHERE s."RecordType"='EXAM_REASON'
 AND NOT EXISTS (
  SELECT 1 FROM mskbs."ReasonForExaminationDefs" x
  WHERE x."Code"::text=upper(btrim(s."Code"))
    AND NOT x."IsDeleted"
 );

COMMIT;
