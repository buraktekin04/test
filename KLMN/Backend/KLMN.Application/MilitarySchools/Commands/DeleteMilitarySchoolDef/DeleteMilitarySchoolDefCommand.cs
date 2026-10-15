using MediatR;

namespace KLMN.Application.MilitarySchools.Commands.DeleteMilitarySchoolDef;

/// <summary>Okulu ilişkileri koruyarak soft-delete olarak işaretler.</summary>
public sealed record DeleteMilitarySchoolDefCommand(Guid Id, uint Version)
    : IRequest;
