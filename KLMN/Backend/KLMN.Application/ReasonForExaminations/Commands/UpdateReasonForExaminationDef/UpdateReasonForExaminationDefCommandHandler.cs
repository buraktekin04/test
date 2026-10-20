using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Commands.Common;
using KLMN.Application.ReasonForExaminations.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExaminations.Commands.UpdateReasonForExaminationDef;
/// <summary>Update işlemini muayene sebebi kayıtları için yürütür.</summary>
public sealed class UpdateReasonForExaminationDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateReasonForExaminationDefCommand,ReasonForExaminationDefMutationResponse>
{
    public async Task<ReasonForExaminationDefMutationResponse> Handle(UpdateReasonForExaminationDefCommand request,CancellationToken ct)
    {
        var item=await ReasonForExaminationDefRules.LoadAsync(db,request.Id,request.Version,ct);
        await ReasonForExaminationDefRules.EnsureUniqueAsync(db,request.Code,item.Id,ct);
        item.Code=request.Code.Trim().ToUpperInvariant();
        item.Description=request.Description.Trim();
        item.DispatchRequired=request.DispatchRequired;
        item.ClassificationExamination=request.ClassificationExamination;
        item.Objectionable=request.Objectionable;
        item.ValidFrom=request.ValidFrom;
        item.ValidTo=request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
