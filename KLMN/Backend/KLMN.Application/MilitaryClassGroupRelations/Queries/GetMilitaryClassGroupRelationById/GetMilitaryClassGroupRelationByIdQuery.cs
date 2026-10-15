using MediatR;
using KLMN.Application.MilitaryClassGroupRelations.Responses;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationById;

/// <summary>Seçili ilişkinin güncel xmin değerini getirir.</summary>
public sealed record GetMilitaryClassGroupRelationByIdQuery(Guid Id) : IRequest<MilitaryClassGroupRelationResponse>;
