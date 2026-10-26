using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Commands.Common;


using MediatR;
namespace KLMN.Application.HcUnusedReasons.Commands.DeleteHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için Delete işini veri bütünlüğüyle yürütür.</summary>
public sealed class DeleteHcUnusedReasonForExamCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteHcUnusedReasonForExamCommand>
{
 public async Task Handle(DeleteHcUnusedReasonForExamCommand r,CancellationToken ct)
 {
    var item=await HcUnusedReasonForExamRules.LoadAsync(db,r.Id,r.Version,ct);
    db.HcUnusedReasonForExams.Remove(item);await db.SaveChangesAsync(ct);
 }
}
