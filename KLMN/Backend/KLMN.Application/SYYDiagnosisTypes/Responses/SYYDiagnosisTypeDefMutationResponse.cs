using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisTypes.Responses;
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record SYYDiagnosisTypeDefMutationResponse(Guid Id,uint Version);
