using MediatR;
using KLMN.Application.HcDecisions.Responses;
namespace KLMN.Application.HcDecisions.Commands.CreateHcDecisionDef;
/// <summary>Kurul kararı Create CQRS isteği.</summary>
public sealed record CreateHcDecisionDefCommand(int Code,string Name,bool TimeInformationRequired,bool AdditionalDescriptionRequired,bool NegativeDecision,bool ApprovalRequired,bool PertemApprovalRequired,bool DiagnosisRequired,DateTime? ValidFrom,DateTime? ValidTo):IRequest<HcDecisionDefMutationResponse>;
