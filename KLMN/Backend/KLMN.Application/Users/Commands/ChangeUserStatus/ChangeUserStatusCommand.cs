using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Commands.ChangeUserStatus;

/// <summary>Kullanıcıyı pasifleştirme veya aktifleştirme komutudur.</summary>
public sealed record ChangeUserStatusCommand : IRequest<UserMutationResponse>
{
    /// <summary>Durumu değiştirilecek kullanıcı kimliğidir.</summary>
    public Guid Id { get; init; }
    /// <summary>Önceki güncel PostgreSQL xmin concurrency değeridir.</summary>
    public uint Version { get; init; }
    /// <summary>İstenen yeni aktiflik durumudur.</summary>
    public bool IsActive { get; init; }
}
