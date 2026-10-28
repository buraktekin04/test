-- KLMN: Soft-delete kod tekrar kullanımını engelleyen üç Code alternate-key FK'sini GUID'ye taşı.
-- PostgreSQL / şema mevcut search_path üzerinden bulunur (mskbs için SET search_path TO mskbs).
-- Önce yedek alın. SADECE test veritabanında doğrulandıktan sonra üretime planlı uygulayın.
-- Çalıştırma öncesi: SELECT current_schema(); -- hedef tablolar burada olmalıdır.
-- Mevcut EF migration geçmişi farklıysa bu scripti migration Up/Down içine UYARLAYIN;
-- hem scripti hem aynı DDL'yi içeren bir migration'ı üst üste uygulamayın.
BEGIN;
SET LOCAL lock_timeout = '15s';
SET LOCAL statement_timeout = '120s';

-- 1. Kalıcı FK alanları: mevcut numeric Code kolonu API/geriye uyumluluk için korunur.
ALTER TABLE "ProcedureDefs" ADD COLUMN IF NOT EXISTS "ProcedureTypeDefId" uuid;
ALTER TABLE "SYYDiagnosisDefs" ADD COLUMN IF NOT EXISTS "SYYDiagnosisTypeDefId" uuid;
ALTER TABLE "SYYDiagnosisDefs" ADD COLUMN IF NOT EXISTS "SYYDiagnosisSliceDefId" uuid;

-- 2. Eski alternate key hâlâ benzersizken tek-anlamlı mevcut ilişkilere GUID atayın.
UPDATE "ProcedureDefs" p SET "ProcedureTypeDefId" = t."Id"
  FROM "ProcedureTypeDefs" t
 WHERE p."ProcedureTypeDefId" IS NULL AND p."ProcedureType"=t."Code";
UPDATE "SYYDiagnosisDefs" d SET "SYYDiagnosisTypeDefId" = t."Id"
  FROM "SYYDiagnosisTypeDefs" t
 WHERE d."SYYDiagnosisTypeDefId" IS NULL AND d."SYYDiagnosisType"=t."Code";
UPDATE "SYYDiagnosisDefs" d SET "SYYDiagnosisSliceDefId" = s."Id"
  FROM "SYYDiagnosisSliceDefs" s
 WHERE d."SYYDiagnosisSliceDefId" IS NULL AND d."Slice"=s."Code";

-- 3. Kaybolacak/yanlış bağlanacak satırları sessizce bırakmayın.
DO $guard$
BEGIN
 IF EXISTS (SELECT 1 FROM "ProcedureDefs" WHERE "ProcedureTypeDefId" IS NULL) THEN
  RAISE EXCEPTION 'ProcedureDefs: ProcedureTypeDefId eksik, geçiş iptal edildi';
 END IF;
 IF EXISTS (SELECT 1 FROM "SYYDiagnosisDefs" WHERE "SYYDiagnosisTypeDefId" IS NULL) THEN
  RAISE EXCEPTION 'SYYDiagnosisDefs: SYYDiagnosisTypeDefId eksik, geçiş iptal edildi';
 END IF;
 IF EXISTS (SELECT 1 FROM "SYYDiagnosisDefs" WHERE "Slice" IS NOT NULL AND "SYYDiagnosisSliceDefId" IS NULL) THEN
  RAISE EXCEPTION 'SYYDiagnosisDefs: Slice mevcut ama GUID FK boş, geçiş iptal edildi';
 END IF;
END
$guard$;

ALTER TABLE "ProcedureDefs" ALTER COLUMN "ProcedureTypeDefId" SET NOT NULL;
ALTER TABLE "SYYDiagnosisDefs" ALTER COLUMN "SYYDiagnosisTypeDefId" SET NOT NULL;

-- 4. Önce eski numeric Code FK'si, sonra onu tutan koşulsuz AK kaldırılır.
-- Varsayılan EF adları farklıysa bu aşama HATA VERİR ve tüm transaction geri döner.
ALTER TABLE "ProcedureDefs"
 DROP CONSTRAINT IF EXISTS "FK_ProcedureDefs_ProcedureTypeDefs_ProcedureType";
ALTER TABLE "SYYDiagnosisDefs"
 DROP CONSTRAINT IF EXISTS "FK_SYYDiagnosisDefs_SYYDiagnosisTypeDefs_SYYDiagnosisType";
ALTER TABLE "SYYDiagnosisDefs"
 DROP CONSTRAINT IF EXISTS "FK_SYYDiagnosisDefs_SYYDiagnosisSliceDefs_Slice";

ALTER TABLE "ProcedureTypeDefs" DROP CONSTRAINT IF EXISTS "AK_ProcedureTypeDefs_Code";
ALTER TABLE "SYYDiagnosisTypeDefs" DROP CONSTRAINT IF EXISTS "AK_SYYDiagnosisTypeDefs_Code";
ALTER TABLE "SYYDiagnosisSliceDefs" DROP CONSTRAINT IF EXISTS "AK_SYYDiagnosisSliceDefs_Code";

-- 5. GUID FK: silinmiş eski tipe bağlı eski tetkikler doğru kaydı göstermeye devam eder.
ALTER TABLE "ProcedureDefs" ADD CONSTRAINT "FK_ProcedureDefs_ProcedureTypeDefs_ProcedureTypeDefId"
 FOREIGN KEY ("ProcedureTypeDefId") REFERENCES "ProcedureTypeDefs" ("Id") ON DELETE RESTRICT;
ALTER TABLE "SYYDiagnosisDefs" ADD CONSTRAINT "FK_SYYDiagnosisDefs_SYYDiagnosisTypeDefs_SYYDiagnosisTypeDefId"
 FOREIGN KEY ("SYYDiagnosisTypeDefId") REFERENCES "SYYDiagnosisTypeDefs" ("Id") ON DELETE RESTRICT;
ALTER TABLE "SYYDiagnosisDefs" ADD CONSTRAINT "FK_SYYDiagnosisDefs_SYYDiagnosisSliceDefs_SYYDiagnosisSliceDefId"
 FOREIGN KEY ("SYYDiagnosisSliceDefId") REFERENCES "SYYDiagnosisSliceDefs" ("Id") ON DELETE RESTRICT;

CREATE INDEX IF NOT EXISTS "IX_ProcedureDefs_ProcedureTypeDefId" ON "ProcedureDefs" ("ProcedureTypeDefId");
CREATE INDEX IF NOT EXISTS "IX_SYYDiagnosisDefs_SYYDiagnosisTypeDefId" ON "SYYDiagnosisDefs" ("SYYDiagnosisTypeDefId");
CREATE INDEX IF NOT EXISTS "IX_SYYDiagnosisDefs_SYYDiagnosisSliceDefId" ON "SYYDiagnosisDefs" ("SYYDiagnosisSliceDefId");

-- 6. Silinmemiş (aktif veya pasif) kodlar benzersiz, silinmişlerin kodu serbest.
-- Önceki aynı adlı koşulsuz bağımsız indeks varsa değiştir (constraint-owned ise HATA verir).
DROP INDEX IF EXISTS "IX_ProcedureTypeDefs_Code";
DROP INDEX IF EXISTS "IX_SYYDiagnosisTypeDefs_Code";
DROP INDEX IF EXISTS "IX_SYYDiagnosisSliceDefs_Code";
CREATE UNIQUE INDEX "IX_ProcedureTypeDefs_Code" ON "ProcedureTypeDefs" ("Code") WHERE "IsDeleted" = FALSE;
CREATE UNIQUE INDEX "IX_SYYDiagnosisTypeDefs_Code" ON "SYYDiagnosisTypeDefs" ("Code") WHERE "IsDeleted" = FALSE;
CREATE UNIQUE INDEX "IX_SYYDiagnosisSliceDefs_Code" ON "SYYDiagnosisSliceDefs" ("Code") WHERE "IsDeleted" = FALSE;

COMMIT;
-- SONRASINDA doğrulayın:
-- SELECT tablename,indexname,indexdef FROM pg_indexes
-- WHERE tablename IN ('ProcedureTypeDefs','SYYDiagnosisTypeDefs','SYYDiagnosisSliceDefs') AND indexdef ILIKE '%unique%';
-- SELECT COUNT(*) FROM "ProcedureDefs" WHERE "ProcedureTypeDefId" IS NULL;
-- SELECT COUNT(*) FROM "SYYDiagnosisDefs" WHERE "SYYDiagnosisTypeDefId" IS NULL;
