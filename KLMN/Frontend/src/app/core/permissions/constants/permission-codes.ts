/**
 * Backend KLMN.Domain.Constants.PermissionCodes karşılıklarıdır.
 * UI görünürlüğü içindir; gerçek authorization kararı backend'de verilir.
 */
export const PermissionCodes = {
  Users: {
    View: 'Users.View',
    Query: 'Users.Query',
    Create: 'Users.Create',
    Update: 'Users.Update',
    Delete: 'Users.Delete',
    Activate: 'Users.Activate',
    AssignRole: 'Users.AssignRole',
    ManagePermissions: 'Users.ManagePermissions',
    ViewDocuments: 'Users.ViewDocuments',
    ManageDocuments: 'Users.ManageDocuments'
  },
  Roles: {
    View: 'Roles.View',
    Query: 'Roles.Query',
    Create: 'Roles.Create',
    Update: 'Roles.Update',
    Delete: 'Roles.Delete',
    ManagePermissions: 'Roles.ManagePermissions'
  },
  AuditLogs: {
    View: 'AuditLogs.View'
  },
  ForcesCommands: {
    View: 'ForcesCommands.View', Query: 'ForcesCommands.Query',
    Create: 'ForcesCommands.Create', Update: 'ForcesCommands.Update', Delete: 'ForcesCommands.Delete'
  },
  Countries: {
    View: 'Countries.View', Query: 'Countries.Query',
    Create: 'Countries.Create', Update: 'Countries.Update', Delete: 'Countries.Delete'
  },
  /** İlçe Tanımları için UI izin kodları. */
  /** Askerî Okul Tanımları işlem izinleri. */
  /** Askerî Sınıf Tanımları işlemlerinin istemci permission kodları. */
  /** Askerî sınıf grubu tanımlama işlem yetkileri. */
  MilitaryClassGroups: {
    View: 'MilitaryClassGroups.View', Query: 'MilitaryClassGroups.Query',
    Create: 'MilitaryClassGroups.Create', Update: 'MilitaryClassGroups.Update',
    Delete: 'MilitaryClassGroups.Delete'
  },
  /** Kuvvet-sınıf-grup eşleştirme yetkileri; tanım yönetiminden bağımsızdır. */
  MilitaryClassGroupRelations: {
    View: 'MilitaryClassGroupRelations.View', Query: 'MilitaryClassGroupRelations.Query',
    Create: 'MilitaryClassGroupRelations.Create', Update: 'MilitaryClassGroupRelations.Update',
    Delete: 'MilitaryClassGroupRelations.Delete'
  },
  MilitaryClasses: {
    View: 'MilitaryClasses.View', Query: 'MilitaryClasses.Query',
    Create: 'MilitaryClasses.Create', Update: 'MilitaryClasses.Update',
    Delete: 'MilitaryClasses.Delete'
  },
  /** Askerlik Şubesi Tanımları için eylem bazlı izin kodları. */
  /** Başvuru Grubu Tanımları eylem bazlı izin kodları. */
  /** Bağlı kurul kararı tanımlarının yetki kodları. */
  /** Branş Tanımları eylem bazlı izinleri. */
  /** Rapor Türü yetki kodları. */
  /** Sağlık Tesisi tanım izinleri. */
  /** ReasonsForExaminations işlemleri backend ile aynı yetki kodlarını kullanır. */
  /** Kurul Karar Tanımları için eylem bazlı izinler. */
  HcDecisions: {
    View:'HcDecisions.View',Query:'HcDecisions.Query',Create:'HcDecisions.Create',
    Update:'HcDecisions.Update',Delete:'HcDecisions.Delete'
  },
  /** Kullanılmayacak Muayene Sebepleri için eylem bazlı izinler. */
  HcUnusedReasons: {
    View:'HcUnusedReasons.View',Query:'HcUnusedReasons.Query',Create:'HcUnusedReasons.Create',
    Update:'HcUnusedReasons.Update',Delete:'HcUnusedReasons.Delete'
  },
  /** Kullanılmayacak Başvuru Grupları için eylem bazlı izinler. */
  HcUnusedAdmissionTypes: {
    View:'HcUnusedAdmissionTypes.View',Query:'HcUnusedAdmissionTypes.Query',Create:'HcUnusedAdmissionTypes.Create',
    Update:'HcUnusedAdmissionTypes.Update',Delete:'HcUnusedAdmissionTypes.Delete'
  },
  /** Birlikte Verilmeyecek Kararlar için eylem bazlı izinler. */
  HcUnusedDecisions: {
    View:'HcUnusedDecisions.View',Query:'HcUnusedDecisions.Query',Create:'HcUnusedDecisions.Create',
    Update:'HcUnusedDecisions.Update',Delete:'HcUnusedDecisions.Delete'
  },
  /** Bağlı Kararlar için eylem bazlı izinler. */
  HcDecisionConnections: {
    View:'HcDecisionConnections.View',Query:'HcDecisionConnections.Query',Create:'HcDecisionConnections.Create',
    Update:'HcDecisionConnections.Update',Delete:'HcDecisionConnections.Delete'
  },
  ReasonsForExaminations: {
    View: 'ReasonsForExaminations.View', Query: 'ReasonsForExaminations.Query',
    Create: 'ReasonsForExaminations.Create', Update: 'ReasonsForExaminations.Update', Delete: 'ReasonsForExaminations.Delete'
  },
  /** ReasonForExamReportTypes işlemleri backend ile aynı yetki kodlarını kullanır. */
  ReasonForExamReportTypes: {
    View: 'ReasonForExamReportTypes.View', Query: 'ReasonForExamReportTypes.Query',
    Create: 'ReasonForExamReportTypes.Create', Update: 'ReasonForExamReportTypes.Update', Delete: 'ReasonForExamReportTypes.Delete'
  },
  /** ReasonForExamUnusedHcAdmissionTypes işlemleri backend ile aynı yetki kodlarını kullanır. */
  ReasonForExamUnusedHcAdmissionTypes: {
    View: 'ReasonForExamUnusedHcAdmissionTypes.View', Query: 'ReasonForExamUnusedHcAdmissionTypes.Query',
    Create: 'ReasonForExamUnusedHcAdmissionTypes.Create', Update: 'ReasonForExamUnusedHcAdmissionTypes.Update', Delete: 'ReasonForExamUnusedHcAdmissionTypes.Delete'
  },
  /** ReasonForExamProcedureDefs işlemleri backend ile aynı yetki kodlarını kullanır. */
  ReasonForExamProcedureDefs: {
    View: 'ReasonForExamProcedureDefs.View', Query: 'ReasonForExamProcedureDefs.Query',
    Create: 'ReasonForExamProcedureDefs.Create', Update: 'ReasonForExamProcedureDefs.Update', Delete: 'ReasonForExamProcedureDefs.Delete'
  },
  /** ReasonForExamBranchDefs işlemleri backend ile aynı yetki kodlarını kullanır. */
  ReasonForExamBranchDefs: {
    View: 'ReasonForExamBranchDefs.View', Query: 'ReasonForExamBranchDefs.Query',
    Create: 'ReasonForExamBranchDefs.Create', Update: 'ReasonForExamBranchDefs.Update', Delete: 'ReasonForExamBranchDefs.Delete'
  },
  /** Procedures işlemleri backend ile aynı yetki kodlarını kullanır. */
  /** Dinamik Tetkik Türü Tanımları ekranının yetkileri. */
  ProcedureTypes: {
    View: 'ProcedureTypes.View', Query: 'ProcedureTypes.Query',
    Create: 'ProcedureTypes.Create', Update: 'ProcedureTypes.Update', Delete: 'ProcedureTypes.Delete'
  },
  /** SYY Tanı Listesi (M/D/F) yetkileri. */
  /** SYY Tanı Sınıflandırma Çizelgesi izinleri. */
  /** ICD10Defs için bağımsız tanım izinleri. */
  ICD10Defs:{View:'ICD10Defs.View',Query:'ICD10Defs.Query',Create:'ICD10Defs.Create',
   Update:'ICD10Defs.Update',Delete:'ICD10Defs.Delete'},
  /** ICD10Diagnostics için bağımsız tanım izinleri. */
  /** SYY tanı karar istatistiği; tanımlama ve salt-okunur sorgulama. */
  SYYDiagnosisDecisions:{
    View:'SYYDiagnosisDecisions.View',Query:'SYYDiagnosisDecisions.Query',
    Create:'SYYDiagnosisDecisions.Create',Update:'SYYDiagnosisDecisions.Update',
    Delete:'SYYDiagnosisDecisions.Delete'
  },
  ICD10Diagnostics:{View:'ICD10Diagnostics.View',Query:'ICD10Diagnostics.Query',Create:'ICD10Diagnostics.Create',
   Update:'ICD10Diagnostics.Update',Delete:'ICD10Diagnostics.Delete'},
  SYYClassifications:{View:'SYYClassifications.View',Query:'SYYClassifications.Query',Create:'SYYClassifications.Create',
   Update:'SYYClassifications.Update',Delete:'SYYClassifications.Delete'},
  /** SYY Gereken Branşlar izinleri. */
  SYYDiagnosisBranches:{View:'SYYDiagnosisBranches.View',Query:'SYYDiagnosisBranches.Query',Create:'SYYDiagnosisBranches.Create',
   Update:'SYYDiagnosisBranches.Update',Delete:'SYYDiagnosisBranches.Delete'},
  /** SYY Gereken Tetkikler izinleri. */
  SYYDiagnosisProcedures:{View:'SYYDiagnosisProcedures.View',Query:'SYYDiagnosisProcedures.Query',Create:'SYYDiagnosisProcedures.Create',
   Update:'SYYDiagnosisProcedures.Update',Delete:'SYYDiagnosisProcedures.Delete'},
  /** SYY Onay Gerektiren Maddeler izinleri. */
  SYYDiagnosisRequireds:{View:'SYYDiagnosisRequireds.View',Query:'SYYDiagnosisRequireds.Query',Create:'SYYDiagnosisRequireds.Create',
   Update:'SYYDiagnosisRequireds.Update',Delete:'SYYDiagnosisRequireds.Delete'},
  SYYDiagnoses:{
    View:'SYYDiagnoses.View',Query:'SYYDiagnoses.Query',Create:'SYYDiagnoses.Create',
    Update:'SYYDiagnoses.Update',Delete:'SYYDiagnoses.Delete'
  },
  /** SYY Tanı Türü Tanımları yetkileri. */
  SYYDiagnosisTypes:{
    View:'SYYDiagnosisTypes.View',Query:'SYYDiagnosisTypes.Query',Create:'SYYDiagnosisTypes.Create',
    Update:'SYYDiagnosisTypes.Update',Delete:'SYYDiagnosisTypes.Delete'
  },
  /** SYY Dilim Tanımları yetkileri. */
  SYYDiagnosisSlices:{
    View:'SYYDiagnosisSlices.View',Query:'SYYDiagnosisSlices.Query',Create:'SYYDiagnosisSlices.Create',
    Update:'SYYDiagnosisSlices.Update',Delete:'SYYDiagnosisSlices.Delete'
  },
  /** Sevk Sebebi Tanımları yetkileri. */
  AnnotationReasons:{
    View:'AnnotationReasons.View',Query:'AnnotationReasons.Query',Create:'AnnotationReasons.Create',
    Update:'AnnotationReasons.Update',Delete:'AnnotationReasons.Delete'
  },
  /** Uçtuğu Uçak Tipi Tanımları yetkileri. */
  PlaneTypes:{
    View:'PlaneTypes.View',Query:'PlaneTypes.Query',Create:'PlaneTypes.Create',
    Update:'PlaneTypes.Update',Delete:'PlaneTypes.Delete'
  },
  /** Uçucu Görev Tanımları yetkileri. */
  FlyingMissions:{
    View:'FlyingMissions.View',Query:'FlyingMissions.Query',Create:'FlyingMissions.Create',
    Update:'FlyingMissions.Update',Delete:'FlyingMissions.Delete'
  },
  /** Eşdeğer Tetkikler CRUD ve sorgulama izinleri. */
  EquivalentProcedures: {
    View:'EquivalentProcedures.View',Query:'EquivalentProcedures.Query',
    Create:'EquivalentProcedures.Create',Update:'EquivalentProcedures.Update',
    Delete:'EquivalentProcedures.Delete'
  },
  Procedures: {
    View: 'Procedures.View', Query: 'Procedures.Query',
    Create: 'Procedures.Create', Update: 'Procedures.Update', Delete: 'Procedures.Delete'
  },
  Hospitals: {
    View: 'Hospitals.View', Query: 'Hospitals.Query',
    Create: 'Hospitals.Create', Update: 'Hospitals.Update', Delete: 'Hospitals.Delete'
  },
  /** Sağlık Tesisi Rapor Türleri tanım izinleri. */
  HospitalReportTypes: {
    View: 'HospitalReportTypes.View', Query: 'HospitalReportTypes.Query',
    Create: 'HospitalReportTypes.Create', Update: 'HospitalReportTypes.Update', Delete: 'HospitalReportTypes.Delete'
  },
  ReportTypes: {
    View: 'ReportTypes.View', Query: 'ReportTypes.Query',
    Create: 'ReportTypes.Create', Update: 'ReportTypes.Update',
    Delete: 'ReportTypes.Delete'
  },
  /** Rütbe Grubu yetki kodları. */
  RankGroups: {
    View: 'RankGroups.View', Query: 'RankGroups.Query',
    Create: 'RankGroups.Create', Update: 'RankGroups.Update',
    Delete: 'RankGroups.Delete'
  },
  /** Rütbe yetki kodları. */
  Ranks: {
    View: 'Ranks.View', Query: 'Ranks.Query',
    Create: 'Ranks.Create', Update: 'Ranks.Update',
    Delete: 'Ranks.Delete'
  },
  Branches: {
    View: 'Branches.View', Query: 'Branches.Query',
    Create: 'Branches.Create', Update: 'Branches.Update',
    Delete: 'Branches.Delete'
  },
  ConnectedDecisions: {
    View: 'ConnectedDecisions.View', Query: 'ConnectedDecisions.Query',
    Create: 'ConnectedDecisions.Create', Update: 'ConnectedDecisions.Update',
    Delete: 'ConnectedDecisions.Delete'
  },
  AdmissionTypes: {
    View: 'AdmissionTypes.View', Query: 'AdmissionTypes.Query',
    Create: 'AdmissionTypes.Create', Update: 'AdmissionTypes.Update',
    Delete: 'AdmissionTypes.Delete'
  },
  MilitaryOffices: {
    View: 'MilitaryOffices.View', Query: 'MilitaryOffices.Query',
    Create: 'MilitaryOffices.Create', Update: 'MilitaryOffices.Update',
    Delete: 'MilitaryOffices.Delete'
  },
  MilitarySchools: {
    View: 'MilitarySchools.View', Query: 'MilitarySchools.Query',
    Create: 'MilitarySchools.Create', Update: 'MilitarySchools.Update',
    Delete: 'MilitarySchools.Delete'
  },
  Towns: {
    View: 'Towns.View', Query: 'Towns.Query', Create: 'Towns.Create',
    Update: 'Towns.Update', Delete: 'Towns.Delete'
  },
  Cities: {
    View: 'Cities.View', Query: 'Cities.Query',
    Create: 'Cities.Create', Update: 'Cities.Update', Delete: 'Cities.Delete'
  },
  MilitaryUnits: {
    View: 'MilitaryUnits.View',
    Query: 'MilitaryUnits.Query',
    Create: 'MilitaryUnits.Create',
    Update: 'MilitaryUnits.Update',
    Delete: 'MilitaryUnits.Delete'
  },
  Reports: {
    View: 'Reports.View',
    Query: 'Reports.Query',
    Export: 'Reports.Export',
    Print: 'Reports.Print'
  },
  Investigations: {
    View: 'Investigations.View',
    Query: 'Investigations.Query',
    Create: 'Investigations.Create',
    Update: 'Investigations.Update',
    Delete: 'Investigations.Delete',
    Approve: 'Investigations.Approve',
    Reject: 'Investigations.Reject',
    Close: 'Investigations.Close',
    Export: 'Investigations.Export',
    Print: 'Investigations.Print'
  }
} as const;
