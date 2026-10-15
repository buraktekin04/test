namespace KLMN.Application.MilitarySchools.Exceptions;

/// <summary>İstenen askerî okul silinmiş veya mevcut değilse üretilir.</summary>
public sealed class MilitarySchoolNotFoundException()
    : Exception("Askerî okul kaydı bulunamadı.");
