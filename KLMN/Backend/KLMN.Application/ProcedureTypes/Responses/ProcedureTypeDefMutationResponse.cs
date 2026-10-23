using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ProcedureTypes.Responses;
/// <summary>Kaydetme sonrasında ID ve xmin sürümü.</summary>
public sealed record ProcedureTypeDefMutationResponse(Guid Id,uint Version);
