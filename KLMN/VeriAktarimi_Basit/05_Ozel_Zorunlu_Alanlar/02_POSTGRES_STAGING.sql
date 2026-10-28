CREATE TABLE IF NOT EXISTS mskbs."_SpecialDefinitionsImport" (
 "RecordType" text,
 "Code" text,
 "Name" text,
 "IsActive" text,
 "ForcesCommandRequired" text,
 "RankRequired" text,
 "MilitaryClassRequired" text,
 "MilitaryUnitRequired" text,
 "SenderChairRequired" text,
 "MilitaryOfficeRequired" text,
 "RegistryNumberRequired" text,
 "MilitarySchoolRequired" text,
 "LastNominationDateRequired" text,
 "HeightWeightRequired" text,
 "NthAdmissionRequired" text,
 "DispatchDocumentDateRequired" text,
 "DispatchDocumentNoRequired" text,
 "PertemApprovalRequired" text,
 "TimeInformationRequired" text,
 "AdditionalDescriptionRequired" text,
 "NegativeDecision" text,
 "ApprovalRequired" text,
 "DiagnosisRequired" text,
 "DispatchRequired" text,
 "ClassificationExamination" text,
 "Objectionable" text
);
-- Dosyayı bu tabloya tek seferde DBeaver CSV / UTF-8 / TAB ile aktarın.
-- 4 temel + 22 boolean = 26 sütun.
SELECT COUNT(*) AS staged_count FROM mskbs."_SpecialDefinitionsImport";
