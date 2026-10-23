namespace KLMN.Application.AnnotationReasons.Exceptions;
/// <summary>Sevk Sebebi Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class AnnotationReasonDefConflictException(string message):Exception(message);
