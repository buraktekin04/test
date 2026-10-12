import { ValidityPeriod } from '../models/validity-period.model';

/**
 * Tanım ekranlarının datetime-local formu ile API UTC ISO-8601
 * zaman değerleri arasında güvenli ve ortak dönüştürme sınıfıdır.
 */
export class ValidityPeriodUtils {
  /** Sadece statik metotlar için kullanılan sınıfın örneklenmesini engeller. */
  private constructor() {}

  /** UTC ISO zamanını tarayıcının yerel datetime-local girişine dönüştürür. */
  public static toLocalInput(iso: string | null): string {
    if (!iso) return '';
    const date = new Date(iso);
    if (Number.isNaN(date.getTime())) return '';
    const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return local.toISOString().slice(0, 16);
  }

  /** Tarayıcı yerel saat değerini UTC ISO formatına çevirir. */
  public static toUtcIso(localDateTime: string): string | null {
    if (!localDateTime) return null;
    const date = new Date(localDateTime);
    return Number.isNaN(date.getTime()) ? null : date.toISOString();
  }

  /** Boş uçları kabul eder, her iki tarih doluysa başlangıç < bitiş arar. */
  public static hasValidRange(fromLocal: string, toLocal: string): boolean {
    const from = fromLocal ? new Date(fromLocal).getTime() : null;
    const to = toLocal ? new Date(toLocal).getTime() : null;
    if ((from !== null && Number.isNaN(from)) ||
        (to !== null && Number.isNaN(to))) return false;
    return from === null || to === null || from < to;
  }

  /** İki form alanını ortak API dönem DTO'suna dönüştürür. */
  public static fromLocalInputs(fromLocal: string, toLocal: string): ValidityPeriod {
    return {
      validFrom: this.toUtcIso(fromLocal),
      validTo: this.toUtcIso(toLocal)
    };
  }
}
