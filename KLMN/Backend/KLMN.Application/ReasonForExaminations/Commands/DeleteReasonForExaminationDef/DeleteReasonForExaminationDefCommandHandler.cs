using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Commands.Common;


using MediatR;
namespace KLMN.Application.ReasonForExaminations.Commands.DeleteReasonForExaminationDef;
/// <summary>Delete işlemini muayene sebebi kayıtları için yürütür.</summary>
public sealed class DeleteReasonForExaminationDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteReasonForExaminationDefCommand>
{
    public async Task Handle(DeleteReasonForExaminationDefCommand request,CancellationToken ct)
    {
        var item=await ReasonForExaminationDefRules.LoadAsync(db,request.Id,request.Version,ct);
        await ReasonForExaminationDefRules.EnsureNoLinksAsync(db,item.Id,ct);
        db.ReasonForExaminationDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
