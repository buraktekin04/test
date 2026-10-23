using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnoses.Responses;
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record SYYDiagnosisDefMutationResponse(Guid Id,uint Version);
