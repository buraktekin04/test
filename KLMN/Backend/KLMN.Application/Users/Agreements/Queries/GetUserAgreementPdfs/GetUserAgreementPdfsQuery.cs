using KLMN.Application.Users.Agreements.Responses;
using MediatR;

namespace KLMN.Application.Users.Agreements.Queries.GetUserAgreementPdfs;

/// <summary>Bir kullanıcıya bağlı PDF metadata listesini döndüren sorgudur.</summary>
/// <param name="UserId">PDF'lerin ait olduğu kullanıcı kimliğidir.</param>
public sealed record GetUserAgreementPdfsQuery(Guid UserId)
    : IRequest<IReadOnlyList<UserAgreementPdfResponse>>;
