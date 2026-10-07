import { InjectionToken } from '@angular/core';

/**
 * Backend API temel adresini temsil eder.
 */
export const API_BASE_URL =
  new InjectionToken<string>('API_BASE_URL');
