namespace KLMN.Application.MilitarySchools.Exceptions;

/// <summary>Tekil kod veya aktif FK iş kuralı ihlalini temsil eder.</summary>
public sealed class MilitarySchoolConflictException(string message) : Exception(message);
