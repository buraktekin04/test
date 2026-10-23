using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.PlaneTypes.Responses;
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record PlaneTypeDefMutationResponse(Guid Id,uint Version);
