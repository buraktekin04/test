using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Exceptions;
using KLMN.Application.MilitaryClasses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefById;

/// <summary>Silinmemiş sınıf kaydını liste DTO'su biçiminde getirir.</summary>
public sealed class GetMilitaryClassDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassDefByIdQuery, MilitaryClassDefResponse>
{
    /// <summary>Bulunmayan kimliği 404 problem yanıtına dönüştürülmek üzere bildirir.</summary>
    public async Task<MilitaryClassDefResponse> Handle(GetMilitaryClassDefByIdQuery request, CancellationToken ct) =>
        await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(MilitaryClassDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new MilitaryClassNotFoundException();
}
