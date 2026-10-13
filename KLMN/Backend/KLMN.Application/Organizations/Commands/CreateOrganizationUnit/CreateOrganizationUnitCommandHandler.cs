using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Commands.Common;
using KLMN.Application.Organizations.Responses;
using KLMN.Domain.Entities.Organization;
using MediatR;

namespace KLMN.Application.Organizations.Commands.CreateOrganizationUnit;

/// <summary>Yeni birim kodunu ve üst birim ilişkisinin geçerliliğini doğrulayarak kayıt açar.</summary>
public sealed class CreateOrganizationUnitCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateOrganizationUnitCommand, OrganizationUnitMutationResponse>
{
    public async Task<OrganizationUnitMutationResponse> Handle(CreateOrganizationUnitCommand request, CancellationToken ct)
    {
        await OrganizationUnitRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await OrganizationUnitRules.EnsureValidParentAsync(db, request.ParentOrganizationUnitId, null, ct);

        var unit = new OrganizationUnit
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ParentOrganizationUnitId = request.ParentOrganizationUnitId
        };
        await db.OrganizationUnits.AddAsync(unit, ct);
        await db.SaveChangesAsync(ct);
        return new OrganizationUnitMutationResponse(unit.Id, unit.Version);
    }
}
