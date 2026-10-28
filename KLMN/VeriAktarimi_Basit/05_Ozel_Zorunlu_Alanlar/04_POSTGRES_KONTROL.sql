SELECT "RecordType", COUNT(*) AS row_count
FROM mskbs."_SpecialDefinitionsImport"
GROUP BY "RecordType" ORDER BY "RecordType";
SELECT 'ADMISSION' AS kind, COUNT(*) AS row_count FROM mskbs."HcAdmissionTypeDefs"
UNION ALL SELECT 'DECISION',COUNT(*) FROM mskbs."HcDecisionDefs"
UNION ALL SELECT 'EXAM_REASON',COUNT(*) FROM mskbs."ReasonForExaminationDefs";

-- Kaynaktaki ve hedefteki sayılar eşleşmeli, var olan seed kayıtları varsa
-- toplam sayı daha yüksek olabilir. Gereksiz tekrar olmayacak şekilde kontrol edin.
