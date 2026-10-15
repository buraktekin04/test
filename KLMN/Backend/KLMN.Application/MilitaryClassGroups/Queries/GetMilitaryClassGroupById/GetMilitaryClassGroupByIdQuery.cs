using MediatR;
using KLMN.Application.MilitaryClassGroups.Responses;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupById;

/// <summary>Bir grubun güncel verisini ve xmin sürümünü ister.</summary>
public sealed record GetMilitaryClassGroupByIdQuery(Guid Id) : IRequest<MilitaryClassGroupResponse>;
