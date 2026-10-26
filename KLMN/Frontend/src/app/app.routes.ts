import { Routes } from '@angular/router';

import { authGuard } from './core/auth/guards/auth.guard';
import { guestGuard } from './core/auth/guards/guest.guard';
import { PermissionCodes } from './core/permissions/constants/permission-codes';
import { permissionGuard } from './core/permissions/guards/permission.guard';

/**
 * Uygulamanın bütün route'larıdır. Tanımlamalar menüsündeki kullanıcı
 * ekranları '/app/users' ve '/app/roles' adreslerinde açılır;
 * backend authorization API seviyesinde ayrıca uygulanır.
 */
export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/authentication/login/login.component')
        .then(component => component.LoginComponent)
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/authentication/forgot-password/forgot-password.component')
        .then(component => component.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/authentication/reset-password/reset-password.component')
        .then(component => component.ResetPasswordComponent)
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/app-layout/app-layout.component')
        .then(component => component.AppLayoutComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/home/home.component')
            .then(component => component.HomeComponent)
      },
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'Users'
        },
        loadComponent: () =>
          import('./features/users/user-list/user-list.component')
            .then(component => component.UserListComponent)
      },
      {
        path: 'roles',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'Roles'
        },
        loadComponent: () =>
          import('./features/roles/role-list/role-list.component')
            .then(component => component.RoleListComponent)
      },
      {
        path: 'forces-commands',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ForcesCommands' },
        loadComponent: () => import('./features/forces-commands/forces-command-list/forces-command-list.component')
          .then(component => component.ForcesCommandListComponent)
      },
      {
        path: 'definitions/forces-commands', pathMatch: 'full', redirectTo: 'forces-commands'
      },
      {
        path: 'countries',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Countries' },
        loadComponent: () =>
          import('./features/countries/country-list/country-list.component')
            .then(component => component.CountryListComponent)
      },
      {
        path: 'military-class-groups',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClassGroups' },
        loadComponent: () =>
          import('./features/military-class-groups/group-list/group-list.component')
            .then(component => component.MilitaryClassGroupListComponent)
      },
      {
        path: 'military-class-group-relations',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClassGroupRelations' },
        loadComponent: () =>
          import('./features/military-class-group-relations/relation-list/relation-list.component')
            .then(component => component.MilitaryClassGroupRelationListComponent)
      },
      {
        path: 'military-classes',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClasses' },
        loadComponent: () =>
          import('./features/military-classes/military-class-list/military-class-list.component')
            .then(component => component.MilitaryClassListComponent)
      },
      {
        path:'icd10-defs',canActivate:[permissionGuard],
        data:{permissionModule:'ICD10Defs'},
        loadComponent:()=>import('./features/icd10-defs/icd10-def-list/icd10-def-list.component')
         .then(c=>c.Icd10DefListComponent)
      },
      {
        path:'icd10-diagnostics',canActivate:[permissionGuard],
        data:{permissionModule:'ICD10Diagnostics'},
        loadComponent:()=>import('./features/icd10-diagnostics/icd10-diagnostic-list/icd10-diagnostic-list.component')
         .then(c=>c.Icd10DiagnosticListComponent)
      },
      {
        path:'syy-classifications',canActivate:[permissionGuard],
        data:{permissionModule:'SYYClassifications'},
        loadComponent:()=>import('./features/syy-classifications/syy-classification-list/syy-classification-list.component')
         .then(c=>c.SyyClassificationListComponent)
      },
      {
        path:'syy-diagnosis-branches',canActivate:[permissionGuard],
        data:{permissionModule:'SYYDiagnosisBranches'},
        loadComponent:()=>import('./features/syy-diagnosis-branches/syy-diagnosis-branch-list/syy-diagnosis-branch-list.component')
         .then(c=>c.SyyDiagnosisBranchListComponent)
      },
      {
        path:'syy-diagnosis-procedures',canActivate:[permissionGuard],
        data:{permissionModule:'SYYDiagnosisProcedures'},
        loadComponent:()=>import('./features/syy-diagnosis-procedures/syy-diagnosis-procedure-list/syy-diagnosis-procedure-list.component')
         .then(c=>c.SyyDiagnosisProcedureListComponent)
      },
      {
        path:'syy-diagnosis-requireds',canActivate:[permissionGuard],
        data:{permissionModule:'SYYDiagnosisRequireds'},
        loadComponent:()=>import('./features/syy-diagnosis-requireds/syy-diagnosis-required-list/syy-diagnosis-required-list.component')
         .then(c=>c.SyyDiagnosisRequiredListComponent)
      },
      {
        path: 'syy-diagnoses',
        canActivate: [permissionGuard],
        data: { permissionModule: 'SYYDiagnoses' },
        loadComponent: () => import('./features/syy-diagnoses/syy-diagnosis-list/syy-diagnosis-list.component')
          .then(x=>x.SyyDiagnosisListComponent)
      },
      {
        path: 'syy-diagnosis-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'SYYDiagnosisTypes' },
        loadComponent: () => import('./features/syy-diagnosis-types/syy-diagnosis-type-list/syy-diagnosis-type-list.component')
          .then(x=>x.SyyDiagnosisTypeListComponent)
      },
      {
        path: 'syy-diagnosis-slices',
        canActivate: [permissionGuard],
        data: { permissionModule: 'SYYDiagnosisSlices' },
        loadComponent: () => import('./features/syy-diagnosis-slices/syy-diagnosis-slice-list/syy-diagnosis-slice-list.component')
          .then(x=>x.SyyDiagnosisSliceListComponent)
      },
      {
        path: 'annotation-reasons',
        canActivate: [permissionGuard],
        data: { permissionModule: 'AnnotationReasons' },
        loadComponent: () => import('./features/annotation-reasons/annotation-reason-list/annotation-reason-list.component')
          .then(x=>x.AnnotationReasonListComponent)
      },
      {
        path: 'plane-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'PlaneTypes' },
        loadComponent: () => import('./features/plane-types/plane-type-list/plane-type-list.component')
          .then(x=>x.PlaneTypeListComponent)
      },
      {
        path: 'flying-missions',
        canActivate: [permissionGuard],
        data: { permissionModule: 'FlyingMissions' },
        loadComponent: () => import('./features/flying-missions/flying-mission-list/flying-mission-list.component')
          .then(x=>x.FlyingMissionListComponent)
      },
      {
        path:'hc-decisions',canActivate:[permissionGuard],
        data:{permissionModule:'HcDecisions'},
        loadComponent:()=>import('./features/hc-decisions/hc-decision-list/hc-decision-list.component').then(c=>c.HcDecisionListComponent)
      },
      {
        path:'hc-unused-reasons',canActivate:[permissionGuard],
        data:{permissionModule:'HcUnusedReasons'},
        loadComponent:()=>import('./features/hc-unused-reasons/hc-unused-reason-list/hc-unused-reason-list.component').then(c=>c.HcUnusedReasonListComponent)
      },
      {
        path:'hc-unused-admission-types',canActivate:[permissionGuard],
        data:{permissionModule:'HcUnusedAdmissionTypes'},
        loadComponent:()=>import('./features/hc-unused-admission-types/hc-unused-admission-list/hc-unused-admission-list.component').then(c=>c.HcUnusedAdmissionListComponent)
      },
      {
        path:'hc-unused-decisions',canActivate:[permissionGuard],
        data:{permissionModule:'HcUnusedDecisions'},
        loadComponent:()=>import('./features/hc-unused-decisions/hc-unused-decision-list/hc-unused-decision-list.component').then(c=>c.HcUnusedDecisionListComponent)
      },
      {
        path:'hc-decision-connections',canActivate:[permissionGuard],
        data:{permissionModule:'HcDecisionConnections'},
        loadComponent:()=>import('./features/hc-decision-connections/hc-decision-connection-list/hc-decision-connection-list.component').then(c=>c.HcDecisionConnectionListComponent)
      },
      {
        path: 'reasons-for-examination',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReasonsForExaminations' },
        loadComponent: () =>
          import('./features/reasons/reason-list/reason-list.component')
            .then(c => c.ReasonListComponent)
      },
      {
        path: 'procedure-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ProcedureTypes' },
        loadComponent: () =>
          import('./features/procedure-types/type-list/type-list.component')
            .then(c => c.ProcedureTypeListComponent)
      },
      {
        path: 'procedures',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Procedures' },
        loadComponent: () =>
          import('./features/procedures/procedure-list/procedure-list.component')
            .then(c => c.ProcedureListComponent)
      },
      {
        path: 'reason-exam-report-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReasonForExamReportTypes', relationKind: 'reportTypes' },
        loadComponent: () =>
          import('./features/reasons/relation-list/relation-list.component')
            .then(c => c.ReasonRelationListComponent)
      },
      {
        path: 'reason-exam-unused-admission-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReasonForExamUnusedHcAdmissionTypes', relationKind: 'unusedAdmissionTypes' },
        loadComponent: () =>
          import('./features/reasons/relation-list/relation-list.component')
            .then(c => c.ReasonRelationListComponent)
      },
      {
        path: 'reason-exam-procedures',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReasonForExamProcedureDefs', relationKind: 'procedures' },
        loadComponent: () =>
          import('./features/reasons/relation-list/relation-list.component')
            .then(c => c.ReasonRelationListComponent)
      },
      {
        path: 'reason-exam-branches',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReasonForExamBranchDefs', relationKind: 'branches' },
        loadComponent: () =>
          import('./features/reasons/relation-list/relation-list.component')
            .then(c => c.ReasonRelationListComponent)
      },
      {
        path: 'hospitals',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Hospitals' },
        loadComponent: () =>
          import('./features/hospitals/hospital-list/hospital-list.component')
            .then(c => c.HospitalListComponent)
      },
      {
        path: 'hospital-report-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'HospitalReportTypes' },
        loadComponent: () =>
          import('./features/hospital-report-types/hospital-report-type-list/hospital-report-type-list.component')
            .then(c => c.HospitalReportTypeListComponent)
      },
      {
        path: 'report-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ReportTypes' },
        loadComponent: () =>
          import('./features/report-types/report-type-list/report-type-list.component')
            .then(component => component.ReportTypeListComponent)
      },
      {
        path: 'rank-groups',
        canActivate: [permissionGuard],
        data: { permissionModule: 'RankGroups' },
        loadComponent: () =>
          import('./features/rank-groups/rank-group-list/rank-group-list.component')
            .then(component => component.RankGroupListComponent)
      },
      {
        path: 'ranks',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Ranks' },
        loadComponent: () =>
          import('./features/ranks/rank-list/rank-list.component')
            .then(component => component.RankListComponent)
      },
      {
        path: 'branches',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Branches' },
        loadComponent: () =>
          import('./features/branches/branch-list/branch-list.component')
            .then(component => component.BranchListComponent)
      },
      {
        path: 'connected-decisions',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ConnectedDecisions' },
        loadComponent: () =>
          import('./features/connected-decisions/connected-decision-list/connected-decision-list.component')
            .then(component => component.ConnectedDecisionListComponent)
      },
      {
        path: 'admission-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'AdmissionTypes' },
        loadComponent: () =>
          import('./features/admission-types/admission-type-list/admission-type-list.component')
            .then(component => component.AdmissionTypeListComponent)
      },
      {
        path: 'military-offices',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryOffices' },
        loadComponent: () =>
          import('./features/military-offices/military-office-list/military-office-list.component')
            .then(component => component.MilitaryOfficeListComponent)
      },
      {
        path: 'military-schools',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitarySchools' },
        loadComponent: () =>
          import('./features/military-schools/military-school-list/military-school-list.component')
            .then(component => component.MilitarySchoolListComponent)
      },
      {
        path: 'towns',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Towns' },
        loadComponent: () =>
          import('./features/towns/town-list/town-list.component')
            .then(component => component.TownListComponent)
      },
      {
        path: 'cities',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Cities' },
        loadComponent: () =>
          import('./features/cities/city-list/city-list.component')
            .then(component => component.CityListComponent)
      },
      {
        path: 'definitions/countries', pathMatch: 'full', redirectTo: 'countries'
      },
      {
        path: 'definitions/cities', pathMatch: 'full', redirectTo: 'cities'
      },
      {
        path: 'definitions/towns', pathMatch: 'full', redirectTo: 'towns'
      },
      {
        path: 'definitions/hospitals', pathMatch: 'full', redirectTo: 'hospitals'
      },
      {
        path: 'definitions/hospital-report-types', pathMatch: 'full', redirectTo: 'hospital-report-types'
      },
      {
        path: 'definitions/report-types', pathMatch: 'full', redirectTo: 'report-types'
      },
      {
        path: 'definitions/rank-groups', pathMatch: 'full', redirectTo: 'rank-groups'
      },
      {
        path: 'definitions/ranks', pathMatch: 'full', redirectTo: 'ranks'
      },
      {
        path: 'definitions/branches', pathMatch: 'full', redirectTo: 'branches'
      },
      {
        path: 'definitions/connected-decisions', pathMatch: 'full', redirectTo: 'connected-decisions'
      },
      {
        path: 'definitions/admission-types', pathMatch: 'full', redirectTo: 'admission-types'
      },
      {
        path: 'definitions/military-offices', pathMatch: 'full', redirectTo: 'military-offices'
      },
      {
        path: 'definitions/military-schools', pathMatch: 'full', redirectTo: 'military-schools'
      },
      {
        path: 'definitions/military-classes', pathMatch: 'full', redirectTo: 'military-classes'
      },
      {
        path: 'definitions/military-class-groups', pathMatch: 'full', redirectTo: 'military-class-groups'
      },
      {
        path: 'definitions/military-class-group-relations', pathMatch: 'full',
        redirectTo: 'military-class-group-relations'
      },
      {
        path: 'military-units',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'MilitaryUnits'
        },
        loadComponent: () =>
          import('./features/military-units/military-unit-list/military-unit-list.component')
            .then(component => component.MilitaryUnitListComponent)
      },
      {
        path: 'definitions/military-units',
        pathMatch: 'full',
        redirectTo: 'military-units'
      },
      {
        path: 'definitions/roles',
        pathMatch: 'full',
        redirectTo: 'roles'
      },
      {
        // Önceki /app/definitions/users favorilerini yeni adrese yönlendirir.
        path: 'definitions/users',
        pathMatch: 'full',
        redirectTo: 'users'
      },
      {
        path: 'audit-logs',
        canActivate: [permissionGuard],
        data: { permissions: [PermissionCodes.AuditLogs.View] },
        loadComponent: () =>
          import('./features/audit-logs/audit-log-list/audit-log-list.component')
            .then(component => component.AuditLogListComponent)
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./features/authentication/change-password/change-password.component')
            .then(component => component.ChangePasswordComponent)
      }
    ]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'app'
  },
  {
    path: '**',
    redirectTo: 'app'
  }
];
