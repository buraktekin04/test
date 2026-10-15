using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Commands.Common;
using KLMN.Application.MilitarySchools.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;

namespace KLMN.Application.MilitarySchools.Commands.CreateMilitarySchoolDef;

/// <summary>Aktif referans kayıtlarını ve tekil kodu doğrulayarak askerî okul oluşturur.</summary>
public sealed class CreateMilitarySchoolDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitarySchoolDefCommand, MilitarySchoolDefMutationResponse>
{
    /// <summary>Okulu doğrulayıp audit interceptor üzerinden kaydeder.</summary>
    public async Task<MilitarySchoolDefMutationResponse> Handle(CreateMilitarySchoolDefCommand request, CancellationToken ct)
    {
        var school = new MilitarySchoolDef();
        await MilitarySchoolDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await MilitarySchoolDefRules.EnsureReferencesAsync(
            db, request.CityId, request.ForcesCommandDefId, ct);
        school.Code = request.Code.Trim().ToUpperInvariant();
        school.Name = request.Name.Trim();
        school.CityId = request.CityId;
        school.ForcesCommandDefId = request.ForcesCommandDefId;
        school.ValidFrom = request.ValidFrom;
        school.ValidTo = request.ValidTo;
        await db.MilitarySchoolDefs.AddAsync(school, ct);
        await db.SaveChangesAsync(ct);
        return new MilitarySchoolDefMutationResponse(school.Id, school.Version);
    }
}
