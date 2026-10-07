/**
 * Backend PermissionCodes karşılıklarıdır.
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
    Delete: 'Roles.Delete'
  },
  Organizations: {
    View: 'Organizations.View',
    Query: 'Organizations.Query',
    Create: 'Organizations.Create',
    Update: 'Organizations.Update',
    Delete: 'Organizations.Delete'
  },
  Investigations: {
    View: 'Investigations.View',
    Query: 'Investigations.Query',
    Create: 'Investigations.Create',
    Update: 'Investigations.Update',
    Delete: 'Investigations.Delete'
  },
  Reports: {
    View: 'Reports.View',
    Query: 'Reports.Query'
  }
} as const;
