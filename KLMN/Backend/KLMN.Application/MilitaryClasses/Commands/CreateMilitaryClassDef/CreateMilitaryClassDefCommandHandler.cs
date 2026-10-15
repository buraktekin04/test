using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Commands.Common;
using KLMN.Application.MilitaryClasses.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;

namespace KLMN.Application.MilitaryClasses.Commands.CreateMilitaryClassDef;

/// <summary>Yeni askerî sınıfı kod tekilliği kontrolüyle kaydeder.</summary>
public sealed class CreateMilitaryClassDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitaryClassDefCommand, MilitaryClassDefMutationResponse>
{
    /// <summary>Sınıfı audit/soft-delete konvansiyonlarıyla oluşturur.</summary>
    public async Task<MilitaryClassDefMutationResponse> Handle(CreateMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = new MilitaryClassDef();
        await MilitaryClassDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.MilitaryClassDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new MilitaryClassDefMutationResponse(item.Id, item.Version);
    }
}
