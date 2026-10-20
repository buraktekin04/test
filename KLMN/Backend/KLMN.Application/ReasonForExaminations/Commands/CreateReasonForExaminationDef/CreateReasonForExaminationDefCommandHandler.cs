using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Commands.Common;
using KLMN.Application.ReasonForExaminations.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ReasonForExaminations.Commands.CreateReasonForExaminationDef;
/// <summary>Create işlemini muayene sebebi kayıtları için yürütür.</summary>
public sealed class CreateReasonForExaminationDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateReasonForExaminationDefCommand,ReasonForExaminationDefMutationResponse>
{
    public async Task<ReasonForExaminationDefMutationResponse> Handle(CreateReasonForExaminationDefCommand request,CancellationToken ct)
    {
        await ReasonForExaminationDefRules.EnsureUniqueAsync(db,request.Code,null,ct);
        var item=new ReasonForExaminationDef();
        item.Code=request.Code.Trim().ToUpperInvariant();
        item.Description=request.Description.Trim();
        item.DispatchRequired=request.DispatchRequired;
        item.ClassificationExamination=request.ClassificationExamination;
        item.Objectionable=request.Objectionable;
        item.ValidFrom=request.ValidFrom;
        item.ValidTo=request.ValidTo;
        await db.ReasonForExaminationDefs.AddAsync(item,ct);
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
