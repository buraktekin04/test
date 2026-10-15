using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Commands.Common;
using KLMN.Application.MilitarySchools.Responses;

using MediatR;

namespace KLMN.Application.MilitarySchools.Commands.UpdateMilitarySchoolDef;

/// <summary>Güncel xmin'i ve yeni FK seçimlerini doğrulayarak askerî okulu günceller.</summary>
public sealed class UpdateMilitarySchoolDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitarySchoolDefCommand, MilitarySchoolDefMutationResponse>
{
    /// <summary>Okulu mevcut concurrency token ile güvenle düzenler.</summary>
    public async Task<MilitarySchoolDefMutationResponse> Handle(UpdateMilitarySchoolDefCommand request, CancellationToken ct)
    {
        var school = await MilitarySchoolDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await MilitarySchoolDefRules.EnsureUniqueCodeAsync(db, request.Code, school.Id, ct);
        await MilitarySchoolDefRules.EnsureReferencesAsync(
            db, request.CityId, request.ForcesCommandDefId, ct, school.CityId, school.ForcesCommandDefId);
        school.Code = request.Code.Trim().ToUpperInvariant();
        school.Name = request.Name.Trim();
        school.CityId = request.CityId;
        school.ForcesCommandDefId = request.ForcesCommandDefId;
        school.ValidFrom = request.ValidFrom;
        school.ValidTo = request.ValidTo;

        await db.SaveChangesAsync(ct);
        return new MilitarySchoolDefMutationResponse(school.Id, school.Version);
    }
}
