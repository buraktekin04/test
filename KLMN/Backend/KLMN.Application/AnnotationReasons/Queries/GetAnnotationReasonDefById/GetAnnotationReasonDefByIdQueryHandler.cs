using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AnnotationReasons.Exceptions;
using KLMN.Application.AnnotationReasons.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetAnnotationReasonDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetAnnotationReasonDefByIdQuery,AnnotationReasonDefResponse>
{
 public async Task<AnnotationReasonDefResponse> Handle(GetAnnotationReasonDefByIdQuery r,CancellationToken ct)=>
 await db.AnnotationReasonDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(x=>new AnnotationReasonDefResponse(x.Id,x.Code,x.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new AnnotationReasonDefNotFoundException();
}
