namespace KLMN.Application.FlyingMissions.Exceptions;
/// <summary>Uçucu Görev Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class FlyingMissionDefConflictException(string message):Exception(message);
