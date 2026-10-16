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
  PurchaseNumbers: {
    View: 'PurchaseNumbers.View', Query: 'PurchaseNumbers.Query',
    Create: 'PurchaseNumbers.Create', Update: 'PurchaseNumbers.Update', Delete: 'PurchaseNumbers.Delete'
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
