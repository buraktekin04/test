using MediatR;
using KLMN.Application.MilitarySchools.Responses;

namespace KLMN.Application.MilitarySchools.Commands.UpdateMilitarySchoolDef;

/// <summary>Var olan askerî okulu güncel xmin ile düzenler.</summary>
public sealed record UpdateMilitarySchoolDefCommand(Guid Id, uint Version, string Code, string Name, Guid? CityId, Guid? ForcesCommandDefId, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitarySchoolDefMutationResponse>;
