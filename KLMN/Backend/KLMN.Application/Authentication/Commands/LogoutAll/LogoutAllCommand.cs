using MediatR;

namespace KLMN.Application.Authentication.Commands.LogoutAll;

/// <summary>
/// Kullanıcının tüm cihazlardaki session'larını sonlandırır.
/// </summary>
public sealed record LogoutAllCommand : IRequest;
