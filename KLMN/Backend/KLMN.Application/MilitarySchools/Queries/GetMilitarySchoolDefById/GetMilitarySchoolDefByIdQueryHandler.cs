using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Exceptions;
using KLMN.Application.MilitarySchools.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefById;

/// <summary>Silinmemiş kaydı detay DTO'su olarak getirir.</summary>
public sealed class GetMilitarySchoolDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitarySchoolDefByIdQuery, MilitarySchoolDefResponse>
{
    /// <summary>Kimliğe göre okul kaydını döndürür; bulunamazsa 404 üretilir.</summary>
    public async Task<MilitarySchoolDefResponse> Handle(GetMilitarySchoolDefByIdQuery request, CancellationToken ct) =>
        await db.MilitarySchoolDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(MilitarySchoolDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new MilitarySchoolNotFoundException();
}
