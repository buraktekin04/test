/**
 * Backend ProblemDetails hata modelidir.
 */
export interface ProblemDetails {
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  [key: string]: unknown;
}
