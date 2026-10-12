using MediatR;

namespace KLMN.Application.Users.Agreements.Commands.DeleteUserAgreementPdf;

/// <summary>Kullanıcıya bağlı belgeyi soft delete yaparak diskteki dosyayı temizler.</summary>
/// <param name="UserId">PDF'nin bağlı olduğu kullanıcıdır.</param>
/// <param name="DocumentId">Silinecek PDF kaydıdır.</param>
public sealed record DeleteUserAgreementPdfCommand(Guid UserId, Guid DocumentId) : IRequest;
