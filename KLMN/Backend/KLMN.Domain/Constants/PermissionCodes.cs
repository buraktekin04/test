namespace KLMN.Domain.Constants;

/// <summary>
/// Uygulama genelinde kullanılan permission kodlarını merkezi ve
/// type-safe şekilde tanımlar.
/// </summary>
public static class PermissionCodes
{
    /// <summary>Kullanıcı yönetimi permission kodlarını içerir.</summary>
    public static class Users
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Users.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Users.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Users.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Users.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Users.Delete";
        /// <summary>Pasif kaydı tekrar aktif duruma getirme yetkisinin teknik kodudur.</summary>
        public const string Activate = "Users.Activate";
        /// <summary>Kullanıcıya rol atama ve rolünü değiştirme yetkisinin kodudur.</summary>
        public const string AssignRole = "Users.AssignRole";
        /// <summary>Rol veya kullanıcı bazlı yetki yönetimi yapabilme kodudur.</summary>
        public const string ManagePermissions = "Users.ManagePermissions";

        /// <summary>Kullanıcı PDF dokümanlarını listeleme ve indirme yetkisidir.</summary>
        public const string ViewDocuments = "Users.ViewDocuments";

        /// <summary>Kullanıcı PDF dokümanlarını yükleme ve silme yetkisidir.</summary>
        public const string ManageDocuments = "Users.ManageDocuments";
    }

    /// <summary>Rol yönetimi permission kodlarını içerir.</summary>
    public static class Roles
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Roles.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Roles.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Roles.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Roles.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Roles.Delete";
        /// <summary>Rol veya kullanıcı bazlı yetki yönetimi yapabilme kodudur.</summary>
        public const string ManagePermissions = "Roles.ManagePermissions";
    }

    /// <summary>Birlik yönetimi permission kodlarını içerir.</summary>
    public static class ForcesCommands
    {
        public const string View = "ForcesCommands.View";
        public const string Query = "ForcesCommands.Query";
        public const string Create = "ForcesCommands.Create";
        public const string Update = "ForcesCommands.Update";
        public const string Delete = "ForcesCommands.Delete";
    }

    /// <summary>Alım numarası tanımlarına ait eylem izinleri.</summary>
    public static class Countries
    {
        public const string View = "Countries.View";
        public const string Query = "Countries.Query";
        public const string Create = "Countries.Create";
        public const string Update = "Countries.Update";
        public const string Delete = "Countries.Delete";
    }
    public static class Cities
    {
        public const string View = "Cities.View";
        public const string Query = "Cities.Query";
        public const string Create = "Cities.Create";
        public const string Update = "Cities.Update";
        public const string Delete = "Cities.Delete";
    }

    /// <summary>İlçe Tanımları modülünün rol ve kullanıcı izin kodları.</summary>
    public static class Towns
    {
        public const string View = "Towns.View";
        public const string Query = "Towns.Query";
        public const string Create = "Towns.Create";
        public const string Update = "Towns.Update";
        public const string Delete = "Towns.Delete";
    }

    /// <summary>Askerî Okul Tanımları için eylem bazlı permission kodları.</summary>
    /// <summary>Askerî Sınıf Tanımları için eylem bazlı yetki kodları.</summary>
    /// <summary>Kuvvete bağlı askerî sınıf grubu tanım modülü.</summary>
    public static class MilitaryClassGroups
    {
        public const string View = "MilitaryClassGroups.View";
        public const string Query = "MilitaryClassGroups.Query";
        public const string Create = "MilitaryClassGroups.Create";
        public const string Update = "MilitaryClassGroups.Update";
        public const string Delete = "MilitaryClassGroups.Delete";
    }

    /// <summary>Kuvvet-sınıf-grup eşleştirmesi modülü.</summary>
    public static class MilitaryClassGroupRelations
    {
        public const string View = "MilitaryClassGroupRelations.View";
        public const string Query = "MilitaryClassGroupRelations.Query";
        public const string Create = "MilitaryClassGroupRelations.Create";
        public const string Update = "MilitaryClassGroupRelations.Update";
        public const string Delete = "MilitaryClassGroupRelations.Delete";
    }

    public static class MilitaryClasses
    {
        public const string View = "MilitaryClasses.View";
        public const string Query = "MilitaryClasses.Query";
        public const string Create = "MilitaryClasses.Create";
        public const string Update = "MilitaryClasses.Update";
        public const string Delete = "MilitaryClasses.Delete";
    }

    /// <summary>Askerlik şubesi tanımlarının işlem yetkileri.</summary>
    /// <summary>Başvuru Grubu Tanımları izin kodları.</summary>
    /// <summary>Bağlı Kurul Karar Tanımları eylem bazlı yetkileri.</summary>
    /// <summary>Branş Tanımları yetki kodları.</summary>
    /// <summary>Rapor Türü modülü için işlem bazlı yetkiler.</summary>
    /// <summary>Sağlık Tesisi tanımlama ekranı yetkileri.</summary>
    public static class Hospitals
    {
        public const string View = "Hospitals.View";
        public const string Query = "Hospitals.Query";
        public const string Create = "Hospitals.Create";
        public const string Update = "Hospitals.Update";
        public const string Delete = "Hospitals.Delete";
    }

    /// <summary>Sağlık Tesisi Rapor Türleri tanımlama ekranı yetkileri.</summary>
    public static class HospitalReportTypes
    {
        public const string View = "HospitalReportTypes.View";
        public const string Query = "HospitalReportTypes.Query";
        public const string Create = "HospitalReportTypes.Create";
        public const string Update = "HospitalReportTypes.Update";
        public const string Delete = "HospitalReportTypes.Delete";
    }
    public static class ReportTypes
    {
        public const string View = "ReportTypes.View";
        public const string Query = "ReportTypes.Query";
        public const string Create = "ReportTypes.Create";
        public const string Update = "ReportTypes.Update";
        public const string Delete = "ReportTypes.Delete";
    }

    /// <summary>Rütbe Grubu modülü için işlem bazlı yetkiler.</summary>
    public static class RankGroups
    {
        public const string View = "RankGroups.View";
        public const string Query = "RankGroups.Query";
        public const string Create = "RankGroups.Create";
        public const string Update = "RankGroups.Update";
        public const string Delete = "RankGroups.Delete";
    }

    /// <summary>Rütbe modülü için işlem bazlı yetkiler.</summary>
    public static class Ranks
    {
        public const string View = "Ranks.View";
        public const string Query = "Ranks.Query";
        public const string Create = "Ranks.Create";
        public const string Update = "Ranks.Update";
        public const string Delete = "Ranks.Delete";
    }

    public static class Branches
    {
        public const string View = "Branches.View";
        public const string Query = "Branches.Query";
        public const string Create = "Branches.Create";
        public const string Update = "Branches.Update";
        public const string Delete = "Branches.Delete";
    }

    public static class ConnectedDecisions
    {
        public const string View = "ConnectedDecisions.View";
        public const string Query = "ConnectedDecisions.Query";
        public const string Create = "ConnectedDecisions.Create";
        public const string Update = "ConnectedDecisions.Update";
        public const string Delete = "ConnectedDecisions.Delete";
    }

    public static class AdmissionTypes
    {
        public const string View = "AdmissionTypes.View";
        public const string Query = "AdmissionTypes.Query";
        public const string Create = "AdmissionTypes.Create";
        public const string Update = "AdmissionTypes.Update";
        public const string Delete = "AdmissionTypes.Delete";
    }

    public static class MilitaryOffices
    {
        public const string View = "MilitaryOffices.View";
        public const string Query = "MilitaryOffices.Query";
        public const string Create = "MilitaryOffices.Create";
        public const string Update = "MilitaryOffices.Update";
        public const string Delete = "MilitaryOffices.Delete";
    }

    public static class MilitarySchools
    {
        public const string View = "MilitarySchools.View";
        public const string Query = "MilitarySchools.Query";
        public const string Create = "MilitarySchools.Create";
        public const string Update = "MilitarySchools.Update";
        public const string Delete = "MilitarySchools.Delete";
    }

    public static class MilitaryUnits
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "MilitaryUnits.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "MilitaryUnits.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "MilitaryUnits.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "MilitaryUnits.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "MilitaryUnits.Delete";
    }

    /// <summary>Raporlama permission kodlarını içerir.</summary>
    public static class Reports
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Reports.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Reports.Query";
        /// <summary>Kayıtları dışa aktarabilme yetkisinin kodudur.</summary>
        public const string Export = "Reports.Export";
        /// <summary>Kayıtların yazıcıya aktarılabilmesi için gereken yetki kodudur.</summary>
        public const string Print = "Reports.Print";
    }

    /// <summary>Tanımlama geçmişini görüntüleme yetkisidir.</summary>
    public static class AuditLogs
    {
        public const string View = "AuditLogs.View";
    }

    /// <summary>İnceleme ve soruşturma permission kodlarını içerir.</summary>
    public static class Investigations
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Investigations.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Investigations.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Investigations.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Investigations.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Investigations.Delete";
        /// <summary>İnceleme veya soruşturma kaydını onaylama yetkisinin kodudur.</summary>
        public const string Approve = "Investigations.Approve";
        /// <summary>İnceleme veya soruşturma kaydını reddetme yetkisinin kodudur.</summary>
        public const string Reject = "Investigations.Reject";
        /// <summary>İnceleme veya soruşturma sürecini kapatma yetkisinin kodudur.</summary>
        public const string Close = "Investigations.Close";
        /// <summary>Kayıtları dışa aktarabilme yetkisinin kodudur.</summary>
        public const string Export = "Investigations.Export";
        /// <summary>Kayıtların yazıcıya aktarılabilmesi için gereken yetki kodudur.</summary>
        public const string Print = "Investigations.Print";
    }
    /// <summary>Muayene Sebebi ekranının işlem yetkileri.</summary>
    /// <summary>Kurul Karar Tanımları için eylem bazlı izinler.</summary>
    public static class HcDecisions
    {
        public const string View = "HcDecisions.View";
        public const string Query = "HcDecisions.Query";
        public const string Create = "HcDecisions.Create";
        public const string Update = "HcDecisions.Update";
        public const string Delete = "HcDecisions.Delete";
    }

    /// <summary>Kullanılmayacak Muayene Sebepleri için eylem bazlı izinler.</summary>
    public static class HcUnusedReasons
    {
        public const string View = "HcUnusedReasons.View";
        public const string Query = "HcUnusedReasons.Query";
        public const string Create = "HcUnusedReasons.Create";
        public const string Update = "HcUnusedReasons.Update";
        public const string Delete = "HcUnusedReasons.Delete";
    }

    /// <summary>Kullanılmayacak Başvuru Grupları için eylem bazlı izinler.</summary>
    public static class HcUnusedAdmissionTypes
    {
        public const string View = "HcUnusedAdmissionTypes.View";
        public const string Query = "HcUnusedAdmissionTypes.Query";
        public const string Create = "HcUnusedAdmissionTypes.Create";
        public const string Update = "HcUnusedAdmissionTypes.Update";
        public const string Delete = "HcUnusedAdmissionTypes.Delete";
    }

    /// <summary>Birlikte Verilmeyecek Kararlar için eylem bazlı izinler.</summary>
    public static class HcUnusedDecisions
    {
        public const string View = "HcUnusedDecisions.View";
        public const string Query = "HcUnusedDecisions.Query";
        public const string Create = "HcUnusedDecisions.Create";
        public const string Update = "HcUnusedDecisions.Update";
        public const string Delete = "HcUnusedDecisions.Delete";
    }

    /// <summary>Bağlı Kararlar için eylem bazlı izinler.</summary>
    public static class HcDecisionConnections
    {
        public const string View = "HcDecisionConnections.View";
        public const string Query = "HcDecisionConnections.Query";
        public const string Create = "HcDecisionConnections.Create";
        public const string Update = "HcDecisionConnections.Update";
        public const string Delete = "HcDecisionConnections.Delete";
    }
    public static class ReasonsForExaminations
    {
        public const string View = "ReasonsForExaminations.View";
        public const string Query = "ReasonsForExaminations.Query";
        public const string Create = "ReasonsForExaminations.Create";
        public const string Update = "ReasonsForExaminations.Update";
        public const string Delete = "ReasonsForExaminations.Delete";
    }

    /// <summary>Tetkik Türü Tanımları ekranının işlem yetkileri.</summary>
    public static class ProcedureTypes
    {
        public const string View = "ProcedureTypes.View";
        public const string Query = "ProcedureTypes.Query";
        public const string Create = "ProcedureTypes.Create";
        public const string Update = "ProcedureTypes.Update";
        public const string Delete = "ProcedureTypes.Delete";
    }

    /// <summary>Tetkik Tanımları ekranının işlem yetkileri.</summary>
    public static class Procedures
    {
        public const string View = "Procedures.View";
        public const string Query = "Procedures.Query";
        public const string Create = "Procedures.Create";
        public const string Update = "Procedures.Update";
        public const string Delete = "Procedures.Delete";
    }

    /// <summary>Muayene Sebebi Rapor Türleri ekranının işlem yetkileri.</summary>
    public static class ReasonForExamReportTypes
    {
        public const string View = "ReasonForExamReportTypes.View";
        public const string Query = "ReasonForExamReportTypes.Query";
        public const string Create = "ReasonForExamReportTypes.Create";
        public const string Update = "ReasonForExamReportTypes.Update";
        public const string Delete = "ReasonForExamReportTypes.Delete";
    }

    /// <summary>Kullanılmayacak Hasta Grupları ekranının işlem yetkileri.</summary>
    public static class ReasonForExamUnusedHcAdmissionTypes
    {
        public const string View = "ReasonForExamUnusedHcAdmissionTypes.View";
        public const string Query = "ReasonForExamUnusedHcAdmissionTypes.Query";
        public const string Create = "ReasonForExamUnusedHcAdmissionTypes.Create";
        public const string Update = "ReasonForExamUnusedHcAdmissionTypes.Update";
        public const string Delete = "ReasonForExamUnusedHcAdmissionTypes.Delete";
    }

    /// <summary>Gereken Tetkikler ekranının işlem yetkileri.</summary>
    public static class ReasonForExamProcedureDefs
    {
        public const string View = "ReasonForExamProcedureDefs.View";
        public const string Query = "ReasonForExamProcedureDefs.Query";
        public const string Create = "ReasonForExamProcedureDefs.Create";
        public const string Update = "ReasonForExamProcedureDefs.Update";
        public const string Delete = "ReasonForExamProcedureDefs.Delete";
    }

    /// <summary>Muayene Gereken Branşlar ekranının işlem yetkileri.</summary>
    public static class ReasonForExamBranchDefs
    {
        public const string View = "ReasonForExamBranchDefs.View";
        public const string Query = "ReasonForExamBranchDefs.Query";
        public const string Create = "ReasonForExamBranchDefs.Create";
        public const string Update = "ReasonForExamBranchDefs.Update";
        public const string Delete = "ReasonForExamBranchDefs.Delete";
    }

    /// <summary>SYY Tanı Sınıflandırma Çizelgesi ekranının bağımsız permission'ları.</summary>
    public static class SYYClassifications
    {
      public const string View="SYYClassifications.View";
      public const string Query="SYYClassifications.Query";
      public const string Create="SYYClassifications.Create";
      public const string Update="SYYClassifications.Update";
      public const string Delete="SYYClassifications.Delete";
    }

    /// <summary>SYY Gereken Branşlar ekranının bağımsız permission'ları.</summary>
    public static class SYYDiagnosisBranches
    {
      public const string View="SYYDiagnosisBranches.View";
      public const string Query="SYYDiagnosisBranches.Query";
      public const string Create="SYYDiagnosisBranches.Create";
      public const string Update="SYYDiagnosisBranches.Update";
      public const string Delete="SYYDiagnosisBranches.Delete";
    }

    /// <summary>SYY Gereken Tetkikler ekranının bağımsız permission'ları.</summary>
    public static class SYYDiagnosisProcedures
    {
      public const string View="SYYDiagnosisProcedures.View";
      public const string Query="SYYDiagnosisProcedures.Query";
      public const string Create="SYYDiagnosisProcedures.Create";
      public const string Update="SYYDiagnosisProcedures.Update";
      public const string Delete="SYYDiagnosisProcedures.Delete";
    }

    /// <summary>SYY Onay Gerektiren Maddeler ekranının bağımsız permission'ları.</summary>
    public static class SYYDiagnosisRequireds
    {
      public const string View="SYYDiagnosisRequireds.View";
      public const string Query="SYYDiagnosisRequireds.Query";
      public const string Create="SYYDiagnosisRequireds.Create";
      public const string Update="SYYDiagnosisRequireds.Update";
      public const string Delete="SYYDiagnosisRequireds.Delete";
    }

    /// <summary>ICD10 Tanımları için bağımsız izin kodları.</summary>
    public static class ICD10Defs
    {
      public const string View="ICD10Defs.View";
      public const string Query="ICD10Defs.Query";
      public const string Create="ICD10Defs.Create";
      public const string Update="ICD10Defs.Update";
      public const string Delete="ICD10Defs.Delete";
    }

    /// <summary>ICD10 SYY Tanı İstatistik Tanımları için bağımsız izin kodları.</summary>
    public static class ICD10Diagnostics
    {
      public const string View="ICD10Diagnostics.View";
      public const string Query="ICD10Diagnostics.Query";
      public const string Create="ICD10Diagnostics.Create";
      public const string Update="ICD10Diagnostics.Update";
      public const string Delete="ICD10Diagnostics.Delete";
    }

    /// <summary>SYY Tanı Listesi (M/D/F) yetkileri.</summary>
    public static class SYYDiagnoses
    {
       public const string View="SYYDiagnoses.View";
       public const string Query="SYYDiagnoses.Query";
       public const string Create="SYYDiagnoses.Create";
       public const string Update="SYYDiagnoses.Update";
       public const string Delete="SYYDiagnoses.Delete";
    }

    /// <summary>SYY Tanı Türü Tanımları yetkileri.</summary>
    public static class SYYDiagnosisTypes
    {
       public const string View="SYYDiagnosisTypes.View";
       public const string Query="SYYDiagnosisTypes.Query";
       public const string Create="SYYDiagnosisTypes.Create";
       public const string Update="SYYDiagnosisTypes.Update";
       public const string Delete="SYYDiagnosisTypes.Delete";
    }

    /// <summary>SYY Dilim Tanımları yetkileri.</summary>
    public static class SYYDiagnosisSlices
    {
       public const string View="SYYDiagnosisSlices.View";
       public const string Query="SYYDiagnosisSlices.Query";
       public const string Create="SYYDiagnosisSlices.Create";
       public const string Update="SYYDiagnosisSlices.Update";
       public const string Delete="SYYDiagnosisSlices.Delete";
    }

    /// <summary>Sevk Sebebi Tanımları yetkileri.</summary>
    public static class AnnotationReasons
    {
       public const string View="AnnotationReasons.View";
       public const string Query="AnnotationReasons.Query";
       public const string Create="AnnotationReasons.Create";
       public const string Update="AnnotationReasons.Update";
       public const string Delete="AnnotationReasons.Delete";
    }

    /// <summary>Uçtuğu Uçak Tipi Tanımları yetkileri.</summary>
    public static class PlaneTypes
    {
       public const string View="PlaneTypes.View";
       public const string Query="PlaneTypes.Query";
       public const string Create="PlaneTypes.Create";
       public const string Update="PlaneTypes.Update";
       public const string Delete="PlaneTypes.Delete";
    }

    /// <summary>Uçucu Görev Tanımları yetkileri.</summary>
    public static class FlyingMissions
    {
       public const string View="FlyingMissions.View";
       public const string Query="FlyingMissions.Query";
       public const string Create="FlyingMissions.Create";
       public const string Update="FlyingMissions.Update";
       public const string Delete="FlyingMissions.Delete";
    }
}
