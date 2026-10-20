using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Commands.Common;
using KLMN.Application.ReasonForExaminations.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExaminations.Commands.ChangeStatusReasonForExaminationDef;
/// <summary>ChangeStatus işlemini muayene sebebi kayıtları için yürütür.</summary>
public sealed class ChangeStatusReasonForExaminationDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusReasonForExaminationDefCommand,ReasonForExaminationDefMutationResponse>
{
    public async Task<ReasonForExaminationDefMutationResponse> Handle(ChangeStatusReasonForExaminationDefCommand request,CancellationToken ct)
    {
        var item=await ReasonForExaminationDefRules.LoadAsync(db,request.Id,request.Version,ct);
        if(!request.IsActive) await ReasonForExaminationDefRules.EnsureNoActiveLinksAsync(db,item.Id,ct);
        item.IsActive=request.IsActive;
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
