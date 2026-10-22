using KLMN.Application.FlyingMissions.Responses;
using MediatR;
namespace KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefById;
/// <summary>Uçucu Görev Tanımları tek kayıt sorgusu.</summary>
public sealed record GetFlyingMissionDefByIdQuery(Guid Id):IRequest<FlyingMissionDefResponse>;
