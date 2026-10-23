using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisSlices.Responses;
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record SYYDiagnosisSliceDefMutationResponse(Guid Id,uint Version);
