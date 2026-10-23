using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Exceptions;
using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefById;

/// <summary>Karar detayını seçilen GUID ile sorgular.</summary>
public sealed class GetHcConnectedDecisionDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHcConnectedDecisionDefByIdQuery, HcConnectedDecisionDefResponse>
{
    /// <summary>Silinmiş veya olmayan kararı 404 olarak ele alır.</summary>
    public async Task<HcConnectedDecisionDefResponse> Handle(GetHcConnectedDecisionDefByIdQuery request, CancellationToken ct) =>
        await db.HcConnectedDecisionDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new HcConnectedDecisionDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new ConnectedDecisionNotFoundException();
}
