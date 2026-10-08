using MediatR;

namespace KLMN.Application.Authentication.Commands.LogoutAll;

/// <summary>Kullanıcının bütün cihazlardaki oturumlarını sonlandırır.</summary>
public sealed record LogoutAllCommand : IRequest;
