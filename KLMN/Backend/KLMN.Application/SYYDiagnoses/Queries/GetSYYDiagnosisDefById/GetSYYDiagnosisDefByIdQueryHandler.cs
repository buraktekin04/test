using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnoses.Exceptions;
using KLMN.Application.SYYDiagnoses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetSYYDiagnosisDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisDefByIdQuery,SYYDiagnosisDefResponse>
{
 public async Task<SYYDiagnosisDefResponse> Handle(GetSYYDiagnosisDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(x=>new SYYDiagnosisDefResponse(
 x.Id,x.Code,x.Name,x.SYYDiagnosisType,x.DiagnosisTypeDef.Name,x.AttachedRecNo,x.AttachedDiagnosis!=null?x.AttachedDiagnosis.Name:null,
 x.Eligible,x.Slice,x.SliceDef!=null?x.SliceDef.Name:null,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisDefNotFoundException();
}
