using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Exceptions;
using KLMN.Application.Towns.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Towns.Queries.GetTownDefById;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed class GetTownDefByIdQueryHandler(IKLMNDbContext db) : IRequestHandler<GetTownDefByIdQuery, TownDefResponse>
{
    public async Task<TownDefResponse> Handle(GetTownDefByIdQuery request, CancellationToken ct) =>
        await db.TownDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new TownDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode,
            CityId=x.CityId, CityName=x.City == null ? null : x.City.Name,
            MernisCode=x.MernisCode,
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new TownNotFoundException();
}
