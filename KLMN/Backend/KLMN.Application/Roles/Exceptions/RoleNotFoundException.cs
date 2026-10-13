

namespace KLMN.Application.Roles.Exceptions;

/// <summary>Var olmayan veya soft-delete edilmiş rol için 404 yanıtı üretir.</summary>
public sealed class RoleNotFoundException : Exception
{
    /// <summary>Güvenli istemci mesajını oluşturur.</summary>
    public RoleNotFoundException() : base("Rol kaydı bulunamadı.") { }
}
