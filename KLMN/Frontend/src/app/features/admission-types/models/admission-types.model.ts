/** Fotoğraftaki HcAdmissionTypeDef yanıtı; tam 14 zorunluluk boolean'ı taşır. */
export interface AdmissionTypeDefItem {
  id: string;
  code: number;
  name: string;
  forcesCommandRequired: boolean;
  rankRequired: boolean;
  militaryClassRequired: boolean;
  militaryUnitRequired: boolean;
  senderChairRequired: boolean;
  militaryOfficeRequired: boolean;
  registryNumberRequired: boolean;
  militarySchoolRequired: boolean;
  lastNominationDateRequired: boolean;
  heightWeightRequired: boolean;
  nthAdmissionRequired: boolean;
  dispatchDocumentDateRequired: boolean;
  dispatchDocumentNoRequired: boolean;
  pertemApprovalRequired: boolean;
  validFrom: string | null;
  validTo: string | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}

/** Kayıt ekleme ve düzenleme sırasında gönderilen alanlar. */
export interface AdmissionTypeFields {
  code: number;
  name: string;
  forcesCommandRequired: boolean;
  rankRequired: boolean;
  militaryClassRequired: boolean;
  militaryUnitRequired: boolean;
  senderChairRequired: boolean;
  militaryOfficeRequired: boolean;
  registryNumberRequired: boolean;
  militarySchoolRequired: boolean;
  lastNominationDateRequired: boolean;
  heightWeightRequired: boolean;
  nthAdmissionRequired: boolean;
  dispatchDocumentDateRequired: boolean;
  dispatchDocumentNoRequired: boolean;
  pertemApprovalRequired: boolean;
  validFrom: string | null;
  validTo: string | null;
}

/** Diğer tanımların filtreleme/sayfalama sözleşmesiyle aynıdır. */
export interface AdmissionTypeQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateAdmissionTypeRequest extends AdmissionTypeFields { id: string; version: number; }
export interface ChangeAdmissionTypeStatusRequest { id: string; version: number; isActive: boolean; }
export interface AdmissionTypeMutationResponse { id: string; version: number; }

/** 14 onay kutusunun tip güvenli alan isimleri. */
export type AdmissionRequirementField =
    'forcesCommandRequired'
  | 'rankRequired'
  | 'militaryClassRequired'
  | 'militaryUnitRequired'
  | 'senderChairRequired'
  | 'militaryOfficeRequired'
  | 'registryNumberRequired'
  | 'militarySchoolRequired'
  | 'lastNominationDateRequired'
  | 'heightWeightRequired'
  | 'nthAdmissionRequired'
  | 'dispatchDocumentDateRequired'
  | 'dispatchDocumentNoRequired'
  | 'pertemApprovalRequired';
