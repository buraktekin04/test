using MediatR;
using KLMN.Application.MilitaryClasses.Responses;

namespace KLMN.Application.MilitaryClasses.Commands.ChangeStatusMilitaryClassDef;

/// <summary>Sistemin IsActive durumunu değiştirir; eski nullable Active alanından ayrıdır.</summary>
public sealed record ChangeStatusMilitaryClassDefCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<MilitaryClassDefMutationResponse>;
