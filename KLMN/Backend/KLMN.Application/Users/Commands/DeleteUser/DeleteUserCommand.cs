using MediatR;

namespace KLMN.Application.Users.Commands.DeleteUser;

/// <summary>Soft delete işlemi için kimlik ve versiyon taşıyan command'dır.</summary>
/// <param name="Id">Silinecek kullanıcı GUID'idir.</param>
/// <param name="Version">Silme öncesi okunmuş xmin versiyonudur.</param>
public sealed record DeleteUserCommand(Guid Id, uint Version) : IRequest;
