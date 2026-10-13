using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Commands.Common;
using KLMN.Application.Organizations.Responses;
using MediatR;

namespace KLMN.Application.Organizations.Commands.UpdateOrganizationUnit;

/// <summary>Üst birimde döngüye izin vermeden, xmin ile organizasyon kaydını günceller.</summary>
public sealed class UpdateOrganizationUnitCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateOrganizationUnitCommand, OrganizationUnitMutationResponse>
{
    public async Task<OrganizationUnitMutationResponse> Handle(UpdateOrganizationUnitCommand request, CancellationToken ct)
    {
        var unit = await OrganizationUnitRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await OrganizationUnitRules.EnsureUniqueCodeAsync(db, request.Code, unit.Id, ct);

        if (unit.ParentOrganizationUnitId != request.ParentOrganizationUnitId)
            await OrganizationUnitRules.EnsureValidParentAsync(db, request.ParentOrganizationUnitId, unit.Id, ct);

        unit.Name = request.Name.Trim();
        unit.Code = request.Code.Trim().ToUpperInvariant();
        unit.ParentOrganizationUnitId = request.ParentOrganizationUnitId;

        await db.SaveChangesAsync(ct);
        return new OrganizationUnitMutationResponse(unit.Id, unit.Version);
    }
}
