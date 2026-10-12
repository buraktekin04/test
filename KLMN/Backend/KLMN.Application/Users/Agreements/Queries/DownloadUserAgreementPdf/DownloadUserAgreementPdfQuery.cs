using KLMN.Application.Users.Agreements.Responses;
using MediatR;

namespace KLMN.Application.Users.Agreements.Queries.DownloadUserAgreementPdf;

/// <summary>Yetki kontrollü PDF indirme isteğinin kullanıcı ve belge kimliğidir.</summary>
/// <param name="UserId">PDF'nin bağlı olduğu kullanıcı kimliğidir.</param>
/// <param name="DocumentId">İndirilecek doküman kimliğidir.</param>
public sealed record DownloadUserAgreementPdfQuery(Guid UserId, Guid DocumentId)
    : IRequest<UserAgreementPdfDownload>;
