using MediatR;
using KLMN.Application.HcDecisions.Responses;
namespace KLMN.Application.HcDecisions.Commands.UpdateHcDecisionDef;
/// <summary>Kurul kararı Update CQRS isteği.</summary>
public sealed record UpdateHcDecisionDefCommand(Guid Id,uint Version,int Code,string Name,bool TimeInformationRequired,bool AdditionalDescriptionRequired,bool NegativeDecision,bool ApprovalRequired,bool PertemApprovalRequired,bool DiagnosisRequired,DateTime? ValidFrom,DateTime? ValidTo):IRequest<HcDecisionDefMutationResponse>;
