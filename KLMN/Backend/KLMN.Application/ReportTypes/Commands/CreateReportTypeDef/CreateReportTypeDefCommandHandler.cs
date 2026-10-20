using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Commands.Common;
using KLMN.Application.ReportTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.CreateReportTypeDef;

/// <summary>Rapor Türü Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateReportTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateReportTypeDefCommand, ReportTypeDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<ReportTypeDefMutationResponse> Handle(CreateReportTypeDefCommand request, CancellationToken ct)
    {
        await ReportTypeDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new ReportTypeDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.ReportTypeDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new ReportTypeDefMutationResponse(item.Id, item.Version);
    }
}
