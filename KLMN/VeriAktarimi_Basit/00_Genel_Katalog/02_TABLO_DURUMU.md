# KLMN Veri Aktarımı — 45 tanım/eşleştirme tablosunun durumu

Bu katalog GitHub'daki **güncel KLMN.Domain entity ve Persistence DbContext** üzerinden düzenlendi. **Kaynak Oracle tablo adları sadece adaydır.** Oracle `ALL_TABLES / ALL_TAB_COLUMNS / ALL_CONSTRAINTS` sonuçları görülmeden eski tablonun gerçekten böyle olduğu varsayılamaz.

| PostgreSQL hedef | İş grubu | Gereken ilişki/özel veri | Oracle aday | Durum |
|---|---|---|---|---|
| `CountryDefs` | 03_CountryCityTown_Tek_Dosya | - | `COUNTRYDEF` | mevcut |
| `CityDefs` | 03_CountryCityTown_Tek_Dosya | CountryDefs | `CITYDEF` | mevcut |
| `TownDefs` | 03_CountryCityTown_Tek_Dosya | CityDefs | `TOWNDEF` | mevcut |
| `AnnotationReasonDefs` | 04_Bagimsiz_Tanimlar | - | `ANNOTATIONREASONDEF` | yeni |
| `BranchDefs` | 04_Bagimsiz_Tanimlar | - | `BRANCHDEF` | yeni |
| `FlyingMissionDefs` | 04_Bagimsiz_Tanimlar | - | `FLYINGMISSIONDEF` | yeni |
| `HcConnectedDecisionDefs` | 04_Bagimsiz_Tanimlar | - | `HCCONNECTEDDECISIONDEF` | yeni |
| `ICD10Defs` | 04_Bagimsiz_Tanimlar | - | `ICD10DEF` | yeni |
| `PlaneTypeDefs` | 04_Bagimsiz_Tanimlar | - | `PLANETYPEDEF` | yeni |
| `ReportTypeDefs` | 04_Bagimsiz_Tanimlar | - | `REPORTTYPEDEF` | yeni |
| `RankGroupDefs` | 04_Bagimsiz_Tanimlar | - | `RANKGROUPDEF` | yeni |
| `ProcedureTypeDefs` | 04_Bagimsiz_Tanimlar | - | `PROCEDURETYPEDEF` | yeni |
| `SYYDiagnosisTypeDefs` | 04_Bagimsiz_Tanimlar | - | `SYYDIAGNOSISTYPEDEF` | yeni |
| `SYYDiagnosisSliceDefs` | 04_Bagimsiz_Tanimlar | - | `SYYDIAGNOSISSLICEDEF` | yeni |
| `ForcesCommandDefs` | 04_Bagimsiz_Tanimlar | - | `FORCESCOMMANDDEF` | yeni |
| `MilitaryClassDefs` | 04_Bagimsiz_Tanimlar | - | `MILITARYCLASSDEF` | yeni |
| `HcAdmissionTypeDefs` | Ozel_Alanlar | 14 zorunlu boolean | `HCADMISSIONTYPEDEF` | kaynak_teyidi |
| `HcDecisionDefs` | Ozel_Alanlar | 6 zorunlu boolean | `HCDECISIONDEF` | kaynak_teyidi |
| `ReasonForExaminationDefs` | Ozel_Alanlar | 3 zorunlu boolean, Description | `REASONFOREXAMINATIONDEF` | kaynak_teyidi |
| `RankDefs` | Bagli_Tanimlar | RankGroupDefs | `RANKDEF` | kaynak_teyidi |
| `MilitaryClassGroupDefs` | Bagli_Tanimlar | ForcesCommandDefs | `MILITARYCLASSGROUPDEF` | kaynak_teyidi |
| `MilitaryUnitDefs` | Bagli_Tanimlar | ForcesCommandDefs;MilitaryUnitDefs | `MILITARYUNITDEF` | kaynak_teyidi |
| `MilitaryOfficeDefs` | Bagli_Tanimlar | CityDefs | `MILITARYOFFICEDEF` | kaynak_teyidi |
| `MilitarySchoolDefs` | Bagli_Tanimlar | CityDefs;ForcesCommandDefs | `MILITARYSCHOOLDEF` | kaynak_teyidi |
| `HospitalDefs` | Bagli_Tanimlar | CityDefs;HospitalDefs | `HOSPITALDEF` | kaynak_teyidi |
| `ProcedureDefs` | Bagli_Tanimlar | ProcedureTypeDefs;3 bool | `PROCEDUREDEF` | kaynak_teyidi |
| `SYYDiagnosisDefs` | SYY_Baglilar | SYYDiagnosisTypeDefs;SYYDiagnosisSliceDefs;self | `SYYDIAGNOSISDEF` | kaynak_teyidi |
| `MilitaryClassGroupRelations` | Esleme | ForcesCommandDefs;MilitaryClassDefs;MilitaryClassGroupDefs | `MILITARYCLASSGROUPRELATION` | kaynak_teyidi |
| `HospitalReportTypes` | Esleme | HospitalDefs;ReportTypeDefs | `HOSPITALREPORTTYPE` | kaynak_teyidi |
| `EquivalentProcedureDefs` | Esleme | ProcedureDefs;ProcedureDefs | `EQUIVALENTPROCEDUREDEF` | kaynak_teyidi |
| `ICD10Diagnostics` | Esleme | ICD10Defs;SYYDiagnosisDefs;HcAdmissionTypeDefs | `ICD10DIAGNOSTIC` | kaynak_teyidi |
| `SYYDiagnosisDecisions` | SYY_Baglilar | SYYDiagnosisDefs;HcDecisionDefs;HcAdmissionTypeDefs;MilitaryClassDefs | `SYYDIAGNOSISDECISION` | kaynak_teyidi |
| `SYYClassificationDefs` | SYY_Baglilar | SYYDiagnosisDefs;ForcesCommandDefs;RankGroupDefs;MilitaryClassGroupDefs;HcDecisionDefs | `SYYCLASSIFICATIONDEF` | kaynak_teyidi |
| `SYYDiagnosisBranchDefs` | SYY_Baglilar | SYYDiagnosisDefs;BranchDefs;age/gender | `SYYDIAGNOSISBRANCHDEF` | kaynak_teyidi |
| `SYYDiagnosisProcedureDefs` | SYY_Baglilar | SYYDiagnosisDefs;ProcedureDefs;age/gender | `SYYDIAGNOSISPROCEDUREDEF` | kaynak_teyidi |
| `SYYDiagnosisRequireds` | SYY_Baglilar | SYYDiagnosisDefs;MustAll/MustOneOf | `SYYDIAGNOSISREQUIRED` | kaynak_teyidi |
| `SYYDiagnosisRequiredDetails` | SYY_Baglilar | SYYDiagnosisRequireds;SortOrder | `SYYDIAGNOSISREQUIREDDETAIL` | kaynak_teyidi |
| `HcConnectedDecisions` | Karar_Muayene_Esleme | HcDecisionDefs;HcConnectedDecisionDefs | `HCCONNECTEDDECISION` | kaynak_teyidi |
| `HcUnusedAdmissionTypes` | Karar_Muayene_Esleme | HcDecisionDefs;HcAdmissionTypeDefs | `HCUNUSEDADMISSIONTYPE` | kaynak_teyidi |
| `HcUnusedDecisions` | Karar_Muayene_Esleme | HcDecisionDefs;HcDecisionDefs | `HCUNUSEDDECISION` | kaynak_teyidi |
| `HcUnusedReasonForExams` | Karar_Muayene_Esleme | HcDecisionDefs;ReasonForExaminationDefs | `HCUNUSEDREASONFOREXAM` | kaynak_teyidi |
| `ReasonForExamBranchDefs` | Karar_Muayene_Esleme | ReasonForExaminationDefs;BranchDefs;age/gender | `REASONFOREXAMBRANCHDEF` | kaynak_teyidi |
| `ReasonForExamProcedureDefs` | Karar_Muayene_Esleme | ReasonForExaminationDefs;ProcedureDefs;age/gender | `REASONFOREXAMPROCEDUREDEF` | kaynak_teyidi |
| `ReasonForExamReportTypes` | Karar_Muayene_Esleme | ReasonForExaminationDefs;ReportTypeDefs | `REASONFOREXAMREPORTTYPE` | kaynak_teyidi |
| `ReasonForExamUnusedHcAdmissionTypes` | Karar_Muayene_Esleme | ReasonForExaminationDefs;HcAdmissionTypeDefs | `REASONFOREXAMUNUSEDHCADMISSIONTYPE` | kaynak_teyidi |

## Genel aktarım sırası

1. Coğrafya: **03_CountryCityTown_Tek_Dosya** — kullanıcı tarafından başarıyla uygulandığı bildirildi; varsa mevcut GUID'leri koru.
2. Bağımsız, Code/Name temelli tanımlar: **04_Bagimsiz_Tanimlar** — tek dosya/tek import; yalnız doğrulanan eski tablolar.
3. Zorunlu boolean veya Description içeren üç özel tanım: HcAdmissionTypeDefs, HcDecisionDefs, ReasonForExaminationDefs.
4. Tek/çoklu FK içeren temel tanımlar: Rank, ClassGroup, Unit, Office, School, Hospital, Procedure.
5. SYY ana tanıları ve ilişkili tanımlar.
6. İki veya daha fazla FK içeren eşleştirmeler ve sağlık kurulu/muayene kuralları.

**Kritik:** 3–6 için Oracle kaynak kolonlarının ve FK'nin eski ObjectId mi Code mu olduğunun belirlenmesi gerekir. Metadata doğrulanmadan tahmini `INSERT` betikleri kullanılmamalı. Bu tablolar henüz otomatik aktarılabilir olarak işaretlenmemiştir.

**Kapsam dışı:** Kullanıcılar, roller, izinler, parola/refresh token, audit ve kişi/sağlık işlem kayıtları; bunların taşınması ayrı erişim/güvenlik ve tarihçe planı gerektirir.

**Güvenlik:** Veri dolu gerçek tablolarda TRUNCATE/DROP yok. Public GitHub'a gerçek Oracle export dosyası, parolalar veya kurumsal kayıtlar yüklenmez.
