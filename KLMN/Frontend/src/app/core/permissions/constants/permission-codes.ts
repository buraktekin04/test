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
    ManagePermissions: 'Users.ManagePermissions'
  },
  Roles: {
    View: 'Roles.View',
    Query: 'Roles.Query',
    Create: 'Roles.Create',
    Update: 'Roles.Update',
    Delete: 'Roles.Delete',
    ManagePermissions: 'Roles.ManagePermissions'
  },
  Organizations: {
    View: 'Organizations.View',
    Query: 'Organizations.Query',
    Create: 'Organizations.Create',
    Update: 'Organizations.Update',
    Delete: 'Organizations.Delete'
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
