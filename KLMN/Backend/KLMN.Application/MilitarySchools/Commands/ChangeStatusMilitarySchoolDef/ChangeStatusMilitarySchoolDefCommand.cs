using MediatR;
using KLMN.Application.MilitarySchools.Responses;
namespace KLMN.Application.MilitarySchools.Commands.ChangeStatusMilitarySchoolDef;

/// <summary>Okulun aktiflik durumunu xmin korumasıyla değiştirir.</summary>
public sealed record ChangeStatusMilitarySchoolDefCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<MilitarySchoolDefMutationResponse>;
