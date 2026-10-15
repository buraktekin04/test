namespace KLMN.Application.MilitaryClassGroupRelations.Exceptions;

/// <summary>Eşleştirme için tekrar eden kayıt veya kuvvet-grup tutarsızlığıdır.</summary>
public sealed class MilitaryClassGroupRelationConflictException(string message) : Exception(message);
