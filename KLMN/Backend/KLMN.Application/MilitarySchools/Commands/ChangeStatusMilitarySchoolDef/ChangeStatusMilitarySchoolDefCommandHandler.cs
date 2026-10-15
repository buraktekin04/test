using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Commands.Common;
using KLMN.Application.MilitarySchools.Responses;
using MediatR;

namespace KLMN.Application.MilitarySchools.Commands.ChangeStatusMilitarySchoolDef;

/// <summary>Okul etkinleştirilirken bağlı şehir/kuvvetin aktif olduğunu doğrular.</summary>
public sealed class ChangeStatusMilitarySchoolDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusMilitarySchoolDefCommand, MilitarySchoolDefMutationResponse>
{
    /// <summary>Kaydı xmin kontrolünden geçirip güvenli durum değişikliği yapar.</summary>
    public async Task<MilitarySchoolDefMutationResponse> Handle(ChangeStatusMilitarySchoolDefCommand request, CancellationToken ct)
    {
        var school = await MilitarySchoolDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await MilitarySchoolDefRules.EnsureReferencesAsync(db, school.CityId, school.ForcesCommandDefId, ct);
        school.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitarySchoolDefMutationResponse(school.Id, school.Version);
    }
}
