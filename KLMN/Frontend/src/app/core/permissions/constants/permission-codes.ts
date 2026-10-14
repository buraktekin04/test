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
