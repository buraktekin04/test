

namespace KLMN.Application.Roles.Exceptions;

/// <summary>Benzersizlik, sistem rolü ve kullanım ilişkisi ihlalleri için 409 yanıtı üretir.</summary>
public sealed class RoleConflictException(string message) : Exception(message);
