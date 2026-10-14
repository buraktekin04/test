namespace KLMN.Application.Users.Exceptions;

/// <summary>Rol atama yetkisi dışındaki işlemler için kullanılır.</summary>
public sealed class UserRoleAssignmentForbiddenException(string message) : Exception(message);
