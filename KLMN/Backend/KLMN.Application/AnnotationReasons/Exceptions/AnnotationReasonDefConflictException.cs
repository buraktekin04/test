namespace KLMN.Application.AnnotationReasons.Exceptions;
/// <summary>Zeyil Sebebi Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class AnnotationReasonDefConflictException(string message):Exception(message);
