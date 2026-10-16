using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Exceptions;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefById;

/// <summary>Silinmemiş şubenin detayını salt okunur sorgular.</summary>
public sealed class GetMilitaryOfficeDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryOfficeDefByIdQuery, MilitaryOfficeDefResponse>
{
    /// <summary>Kayıt bulunmazsa 404 istisnası üretir.</summary>
    public async Task<MilitaryOfficeDefResponse> Handle(GetMilitaryOfficeDefByIdQuery request, CancellationToken ct) =>
        await db.MilitaryOfficeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(MilitaryOfficeDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new MilitaryOfficeNotFoundException();
}
