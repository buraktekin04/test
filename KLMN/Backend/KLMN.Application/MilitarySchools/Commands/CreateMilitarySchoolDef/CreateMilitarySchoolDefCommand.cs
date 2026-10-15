using MediatR;
using KLMN.Application.MilitarySchools.Responses;

namespace KLMN.Application.MilitarySchools.Commands.CreateMilitarySchoolDef;

/// <summary>Yeni askerî okulun bağlı şehir, kuvvet ve UTC geçerlilik alanlarını alır.</summary>
public sealed record CreateMilitarySchoolDefCommand(string Code, string Name, Guid? CityId, Guid? ForcesCommandDefId, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitarySchoolDefMutationResponse>;
