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
            .Select(HcConnectedDecisionDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new ConnectedDecisionNotFoundException();
}
