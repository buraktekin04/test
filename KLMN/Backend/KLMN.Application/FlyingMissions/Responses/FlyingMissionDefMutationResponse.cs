using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.FlyingMissions.Responses;
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record FlyingMissionDefMutationResponse(Guid Id,uint Version);
