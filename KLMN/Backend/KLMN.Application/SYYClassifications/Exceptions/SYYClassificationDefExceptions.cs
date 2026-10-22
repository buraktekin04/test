namespace KLMN.Application.SYYClassifications.Exceptions;
public sealed class SYYClassificationDefNotFoundException():Exception("SYY Tanı Sınıflandırma Çizelgesi kaydı bulunamadı.");
public sealed class SYYClassificationDefConflictException(string message):Exception(message);
